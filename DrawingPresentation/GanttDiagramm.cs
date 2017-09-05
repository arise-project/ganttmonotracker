//----------------------------------------------------------------------------------------------
// <copyright file="GanttDiagramm.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 15.01.2006 at 15:07

namespace GanttMonoTracker.DrawingPresentation
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Linq;

    using Cairo;

    using GanttTracker;
    using GanttTracker.TaskManager;
    using GanttTracker.TaskManager.ManagerException;

    using Gdk;

    using Pango;

    using TaskManagerInterface;

    public sealed class GanttDiagramm : Gtk.DrawingArea, IGuiSource, IGanttSource
    {
        private const int FBorderMarginH = 2;
        public const int FBorderMarginV = 2;
        public const int FTaskHeight = 14;

		bool IGanttSource.DateNowVisible{get;set;} = true;

        bool IGanttSource.ReadOnly{get;set;}

        public DataSet Source{get;set;}

        DataSet IGanttSource.StaticSource{get;set;}

        // todo : implement refresh
        void IGanttSource.Refresh()
        {
			if (!((IGanttSource)this).ReadOnly)
            {
                //this.GdkWindow.ClearArea(fX,fY,fWidth,fHeight);
                //this.GdkWindow.Show();
            }
        }

        protected override bool OnExposeEvent(EventExpose evnt)
        {
            var baseResult = base.OnExposeEvent (evnt);
            if(Source == null && TrackerCore.Instance.TaskManager is EmptyTaskManager)
            {
                return baseResult;
            }

            Source = ((IGanttSource)this).StaticSource ?? TrackerCore.Instance.TaskManager.GanttSource;

            //ReadGepmetry
            int fX, fY, fWidth,fHeight,fDepth;

            GdkWindow.GetGeometry(out fX,out fY,out fWidth,out fHeight,out fDepth);
            fWidth -= 3;
            fHeight -= 3;

			// Insert drawing code here.
			using (var grw = Gdk.CairoHelper.Create(GdkWindow))
			{

				//DrawBorder
				grw.SetSourceRGB(0xff, 0, 0);

				grw.MoveTo(FBorderMarginH, FBorderMarginV);
				grw.LineTo(fWidth - FBorderMarginH, FBorderMarginV);
				grw.LineTo(fWidth - FBorderMarginH, fHeight - FBorderMarginV);
				grw.LineTo(FBorderMarginH, fHeight - FBorderMarginV);
				grw.LineTo(FBorderMarginH, FBorderMarginV);
				grw.Stroke();

				//DrawTasks
				var deltaActor = (Source.Tables["Actor"].Rows.Count > 0) ?
								 (fHeight - 2 * FBorderMarginV) / Source.Tables["Actor"].Rows.Count :
								 fHeight - 2 * FBorderMarginV;

				var v1 = Source.Tables["DataRange"].Rows[0]["MinDate"];
				var v2 = Source.Tables["DataRange"].Rows[0]["MaxDate"];
				var firstDate = v1 as DateTime? ?? DateTime.Now;
				var lastDate = v2 as DateTime? ?? DateTime.Now;
				var deltaSpan = lastDate.Date.Subtract(firstDate.Date);

				var deltaTask = fWidth;
				if (deltaSpan.Days > 1)
				{
					deltaTask = fWidth / deltaSpan.Days;
				}

				var columns = (lastDate - firstDate).Days + 1;
				var filled = new bool[columns, 100];
				var passedState = ConfigurationManager.AppSettings["passed_state"];

				foreach (DataRow row in Source.Tables["Task"].Rows)
				{
					v1 = row["ActorID"];
					if ((v1 as int? ?? -1) < 0) continue;
					var actorIndex = Source.Tables["Actor"].Rows.Cast<DataRow>().TakeWhile(actorRow => (int)actorRow["ID"] != (int)row["ActorID"]).Count();

					v1 = row["StartTime"];
					v2 = row["EndTime"];
					var startTime = v1 as DateTime? ?? DateTime.Now;
					var endTime = v2 as DateTime? ?? DateTime.Now;
					var startSpan = startTime.Subtract(firstDate);
					var endSpan = endTime.Subtract(firstDate);

					// fill availability matrix
					var tasksCount = 0;
					if (columns > (startTime - firstDate).Days && tasksCount < 99)
					{
						try
						{
							while (tasksCount < 99 && filled[(startTime - firstDate).Days, tasksCount])
							{
								tasksCount++;
								tasksCount++;
							}

							var days = (endTime - startTime).Days + 1;
							for (var i = 0; i < days; i++)
							{
								filled[(startTime - firstDate).Days + i, tasksCount] = true;
							}
						}
						catch (Exception ex)
						{
							Console.Write((startTime - firstDate).Days);
							Console.WriteLine(ex);
						}
					}

					var taskGc = new Gdk.GC(GdkWindow);

					if (Source.Tables["TaskState"].Select("ID = " + row["StateID"]).Length == 0)
					{
						throw new KeyNotFoundException<object>(row["StateID"]);
					}

					var list = Source.Tables["TaskState"].Select("ID = " + row["StateID"]);
					if (list.Length > 0)
					{
						var stateRow = list[0];

						var stateName = (string)stateRow["Name"];
						if (passedState.Split(';').Select(s => s.ToLower()).Contains(stateName.ToLower()))
						{
							continue;
						}

						try
						{
							var colorRed = Convert.ToByte(stateRow["ColorRed"]);
							var colorGreen = Convert.ToByte(stateRow["ColorGreen"]);
							var colorBlue = Convert.ToByte(stateRow["ColorBlue"]);

							var foregroundColor = new Gdk.Color(colorRed, colorGreen, colorBlue);
							var colormap = Colormap.System;
							colormap.AllocColor(ref foregroundColor, true, true);
							taskGc.Foreground = foregroundColor;
							colormap.Dispose();
						}
						catch (Exception e)
						{
							Console.WriteLine(e);
						}
					}

					if (lastDate.Date > endTime.Date)
					{
						GdkWindow.DrawRectangle(
							taskGc,
							true,
							startSpan.Days * deltaTask + FBorderMarginH,
							actorIndex * deltaActor + FBorderMarginV + FTaskHeight * (tasksCount + 1),
							(endSpan.Days - startSpan.Days) * deltaTask + deltaTask,
							FTaskHeight);
					}
					else if (startTime.Date < lastDate.Date)
						GdkWindow.DrawRectangle(
							taskGc,
							true,
							startSpan.Days * deltaTask + FBorderMarginH,
							actorIndex * deltaActor + FBorderMarginV + FTaskHeight * (tasksCount + 1),
							(endSpan.Days - startSpan.Days) * deltaTask + FBorderMarginH,
							FTaskHeight);
					else
						GdkWindow.DrawRectangle(
							taskGc,
							true, (startSpan.Days - 1) * deltaTask + FBorderMarginH,
							actorIndex * deltaActor + FBorderMarginV + FTaskHeight * (tasksCount + 1),
							(endSpan.Days - startSpan.Days + 1) * deltaTask + FBorderMarginH,
							FTaskHeight);

					if (((IGanttSource)this).DateNowVisible)
					{
						var layout = new Layout(PangoContext)
						{
							Wrap = WrapMode.Word,
							FontDescription = FontDescription.FromString("Tahoma 10")
						};
						layout.SetMarkup(row["ID"].ToString());

						GdkWindow.DrawLayout(
							GdkPalette.InitColor(GdkWindow, ColorEnum.TaskLabel),
							startSpan.Days * deltaTask + FBorderMarginH,
							actorIndex * deltaActor + FTaskHeight * (tasksCount + 1),
							layout);
					}

					GdkPalette.DestroyColor();
					taskGc.Dispose();
				}

				//DrawActorAxis
				var taskLabelGc = new Gdk.GC(GdkWindow);
				if (!((IGanttSource)this).DateNowVisible) return true;

				//DrawActorAxis
				var offsetActor = FBorderMarginV;
				var foregroundColor2 = new Gdk.Color(0xff, 0, 0);
				var foregroundColor1 = new Gdk.Color(0, 0, 0xff);
				var actorLabelGc = new Gdk.GC(GdkWindow);
				var axisGc = new Gdk.GC(GdkWindow);

				Colormap.System.AllocColor(ref foregroundColor2, true, true);
				taskLabelGc.Foreground = foregroundColor2;
				axisGc.Foreground = foregroundColor1;

				foreach (DataRow row in Source.Tables["Actor"].Rows)
				{
					var layout = new Layout(PangoContext)
					{
						Wrap = WrapMode.Word,
						FontDescription = FontDescription.FromString("Tahoma 10")
					};
					v1 = row["Name"];
					layout.SetMarkup(v1?.ToString() ?? "<unknown>");

					GdkWindow.DrawLayout(actorLabelGc,
						FBorderMarginH,
						offsetActor - FBorderMarginV, layout);
					GdkWindow.DrawLine(axisGc,
						FBorderMarginH,
						offsetActor,
						fWidth - FBorderMarginH, offsetActor);
					offsetActor += deltaActor;
				}

				axisGc.Dispose();

				//DrawDateAxis
				var labelDate1 = firstDate;
				var offset1 = FBorderMarginH;
				for (var i = 0; i < deltaSpan.Days; i++)
				{
					var layout = new Layout(PangoContext)
					{
						Wrap = WrapMode.Word,
						FontDescription = FontDescription.FromString("Tahoma 10")
					};
					layout.SetMarkup(labelDate1.ToString("dd/MM"));

					GdkWindow.DrawLayout(actorLabelGc,
						offset1 + FBorderMarginH,
						fHeight - 2 * FBorderMarginV - FTaskHeight,
						layout);

					GdkWindow.DrawLine(axisGc,
						offset1,
						FBorderMarginV,
						offset1,
						fHeight - FBorderMarginV);
					offset1 += deltaTask;
					labelDate1 = labelDate1.AddDays(1);
				}

				actorLabelGc.Dispose();

				//DrawDateNow
				var nowSpan = DateTime.Now.Subtract(firstDate);

				var offsetDate = FBorderMarginH + (int)(deltaTask * (double)nowSpan.Ticks / TimeSpan.TicksPerDay);

				grw.SetSourceRGB(0, 0, 0);
				grw.MoveTo(offsetDate,
					FBorderMarginV);
				grw.LineTo(offsetDate,
					fHeight - FBorderMarginV);
				grw.RelLineTo(new Distance { Dx = -3, Dy = 0 });
				grw.RelLineTo(new Distance { Dx = 3, Dy = -3 });
				grw.RelLineTo(new Distance { Dx = 3, Dy = 3 });
				grw.RelLineTo(new Distance { Dx = -3, Dy = 0 });
				grw.Stroke();
			}

			return true;
        }
    }
}