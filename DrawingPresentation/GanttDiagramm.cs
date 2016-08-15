//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 15.01.2006 at 15:07
using System;
using System.Data;
using System.Linq;
using Gdk;
using Pango;
using Cairo;

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;
using System.Configuration;
using GanttTracker;
using GanttTracker.TaskManager;

namespace GanttMonoTracker.DrawingPresentation
{
    public class GanttDiagramm : Gtk.DrawingArea, IGuiSource
	{
		#region Constants

		const int fTaskHeight = 14;


		const int fBorderMarginH = 2;


		const int fBorderMarginV = 2;

		#endregion

		public GanttDiagramm() : base()
		{
			DateNowVisible = true;
		}

		#region Public properties.

		public DataSet Source { get;set; }

		public DataSet StaticSource { get;set; }

		public bool DateNowVisible	{ get;set; }


		public bool ReadOnly { get;set; }

		#endregion

		#region Protected methods

		protected override bool OnExposeEvent(EventExpose args)
		{
			var baseResult = base.OnExposeEvent (args);
			if(Source == null && TrackerCore.Instance.TaskManager is EmptyTaskManager)
			{
				return baseResult;
			}

			Source = StaticSource ?? TrackerCore.Instance.TaskManager.GanttSource;

			//ReadGepmetry
			int fX, fY, fWidth,fHeight,fDepth;

			this.GdkWindow.GetGeometry(out fX,out fY,out fWidth,out fHeight,out fDepth);
			fWidth -= 3;
			fHeight -= 3;


			// Insert drawing code here.
			Cairo.Context grw = Gdk.CairoHelper.Create (this.GdkWindow);

			//DrawBorder
			grw.SetSourceRGB(0xff, 0, 0);

			grw.MoveTo(fBorderMarginH, fBorderMarginV);
			grw.LineTo(fWidth - fBorderMarginH, fBorderMarginV);    
			grw.LineTo(fWidth - fBorderMarginH, fHeight - fBorderMarginV);    
			grw.LineTo(fBorderMarginH, fHeight - fBorderMarginV);    
			grw.LineTo(fBorderMarginH, fBorderMarginV);    
			grw.Stroke();

			//DrawTasks
			int deltaActor = (Source.Tables["Actor"].Rows.Count > 0) ? 
			                 (fHeight - 2 * fBorderMarginV) / Source.Tables["Actor"].Rows.Count : 
			                 fHeight - 2 * fBorderMarginV;

			DateTime firstDate = (DateTime)Source.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)Source.Tables["DataRange"].Rows[0]["MaxDate"];
			TimeSpan deltaSpan = lastDate.Date.Subtract(firstDate.Date);

			int deltaTask = fWidth; 
			if (deltaSpan.Days > 1)
				deltaTask = fWidth / deltaSpan.Days;

			var columns = (lastDate - firstDate).Days + 1;
			bool [,] filled = new bool[columns, 100];
			var passedState = ConfigurationManager.AppSettings ["passed_state"];

			foreach(DataRow row in Source.Tables["Task"].Rows)
			{
				if ((int)row["ActorID"] >= 0)
				{
					int actorIndex = 0;
					foreach (DataRow actorRow in Source.Tables["Actor"].Rows)
					{
						if ((int)actorRow["ID"] == (int)row["ActorID"])
							break;
						actorIndex++;
					}

					DateTime startTime = (DateTime)row["StartTime"];
					DateTime endTime = (DateTime)row["EndTime"];
					TimeSpan startSpan = startTime.Subtract(firstDate);
					TimeSpan endSpan = endTime.Subtract(firstDate);	

					// fill availability matrix
					int tasksCount = 0;
					if (columns > (startTime - firstDate).Days && tasksCount  < 99) {
						try
						{
							while (tasksCount < 99 && filled [(startTime - firstDate).Days, tasksCount]) {
								tasksCount++;
								tasksCount++;
							}
						


						var days = (endTime - startTime).Days + 1;
						for (int i = 0; i < days; i++) {
							filled [(startTime - firstDate).Days + i, tasksCount] = true;
						}
						}
						catch(Exception) {
							var c = (startTime - firstDate).Days;
						}
					} 

					Gdk.GC taskGC = new Gdk.GC((Drawable)this.GdkWindow);

					if (Source.Tables["TaskState"].Select("ID = " + row["StateID"]).Length == 0)
						throw new KeyNotFoundException<int>((int)row["StateID"]);

					DataRow stateRow = Source.Tables["TaskState"].Select("ID = " + row["StateID"])[0];

					var stateName = (string)stateRow ["Name"];
					if(passedState.Split(';').Select(s => s.ToLower()).Contains(stateName.ToLower()))
					{
						continue;
					}

					byte colorRed = Convert.ToByte(stateRow["ColorRed"]);
					byte colorGreen = Convert.ToByte(stateRow["ColorGreen"]);
					byte colorBlue = Convert.ToByte(stateRow["ColorBlue"]);


					Gdk.Color foregroundColor = new Gdk.Color(colorRed, colorGreen, colorBlue);
					Colormap colormap = Colormap.System;
					colormap.AllocColor(ref foregroundColor,true,true);
					taskGC.Foreground = foregroundColor;

					if (lastDate.Date > endTime.Date)
						this.GdkWindow.DrawRectangle(
							taskGC, 
							true,
							startSpan.Days * deltaTask + fBorderMarginH, 
							actorIndex * deltaActor + fBorderMarginV + fTaskHeight * (tasksCount+1), 
							(endSpan.Days - startSpan.Days) * deltaTask + deltaTask,
							fTaskHeight);
					else
						if (startTime.Date < lastDate.Date)
							this.GdkWindow.DrawRectangle(
								taskGC, 
								true,
								startSpan.Days * deltaTask + fBorderMarginH,
								actorIndex * deltaActor + fBorderMarginV + fTaskHeight * (tasksCount+1),
								(endSpan.Days - startSpan.Days) * deltaTask + fBorderMarginH,
								fTaskHeight);
					else
							this.GdkWindow.DrawRectangle(
								taskGC,
								true,(startSpan.Days -1) * deltaTask + fBorderMarginH,
								actorIndex * deltaActor + fBorderMarginV + fTaskHeight * (tasksCount+1),
								(endSpan.Days - startSpan.Days + 1) * deltaTask + fBorderMarginH,
								fTaskHeight);

					if (DateNowVisible) {

						Pango.Layout layout = new Pango.Layout (PangoContext) {
							Wrap = Pango.WrapMode.Word,
							FontDescription = FontDescription.FromString ("Tahoma 10")
						};
						layout.SetMarkup (row ["ID"].ToString ());

						this.GdkWindow.DrawLayout (
							GdkPalette.InitColor (this.GdkWindow, ColorEnum.TaskLabel), 
							startSpan.Days * deltaTask + fBorderMarginH,
							actorIndex * deltaActor + fTaskHeight * (tasksCount + 1),
							layout);
					}
					GdkPalette.DestroyColor();
					colormap.Dispose();
					taskGC.Dispose();
				}
			}


			//DrawActorAxis
			Gdk.GC TaskLabelGC = new Gdk.GC (this.GdkWindow);
			if (DateNowVisible) {
				//DrawActorAxis
				int offsetActor = fBorderMarginV; 
				Gdk.Color foregroundColor = new Gdk.Color(0xff, 0, 0);
				Gdk.Color foregroundColor1 = new Gdk.Color(0, 0, 0xff);
				Gdk.GC ActorLabelGC = new Gdk.GC (this.GdkWindow);
				Gdk.GC AxisGC = new Gdk.GC (this.GdkWindow);

				Colormap.System.AllocColor(ref foregroundColor,true,true);
				TaskLabelGC.Foreground = foregroundColor;
				AxisGC.Foreground = foregroundColor1;

				foreach(DataRow row in Source.Tables["Actor"].Rows)
				{
                    Layout layout = new Layout(PangoContext);
					layout.Wrap = WrapMode.Word;
					layout.FontDescription = FontDescription.FromString("Tahoma 10");
					layout.SetMarkup((string)row["Name"]);

					this.GdkWindow.DrawLayout(ActorLabelGC, 
						fBorderMarginH,
						offsetActor - fBorderMarginV,layout);
					this.GdkWindow.DrawLine(AxisGC, 
						fBorderMarginH, 
						offsetActor, 
						fWidth - fBorderMarginH, offsetActor);
					offsetActor += deltaActor; 
				}


				AxisGC.Dispose ();

				//DrawDateAxis

				var labelDate1 = firstDate;
				int offset1 = fBorderMarginH;
				for (int i = 0; i < deltaSpan.Days; i++)
				{
					Layout layout = new Layout(PangoContext);
					layout.Wrap = WrapMode.Word;
					layout.FontDescription = FontDescription.FromString("Tahoma 10");
					layout.SetMarkup(labelDate1.ToString("dd/MM"));

					this.GdkWindow.DrawLayout(ActorLabelGC, 
						offset1 + fBorderMarginH,
						fHeight -2 * fBorderMarginV - fTaskHeight,
						layout);

					this.GdkWindow.DrawLine(AxisGC,
						offset1, 
						fBorderMarginV, 
						offset1, 
						fHeight - fBorderMarginV);
					offset1 += deltaTask;
					labelDate1 = labelDate1.AddDays(1);
				}

				ActorLabelGC.Dispose ();

				//DrawDateNow
				TimeSpan nowSpan = DateTime.Now.Subtract(firstDate);

				int offsetDate = fBorderMarginH + (int)(deltaTask*(double)nowSpan.Ticks / TimeSpan.TicksPerDay);

				grw.SetSourceRGB(0, 0, 0);
				grw.MoveTo(offsetDate, 
					fBorderMarginV);
				grw.LineTo (offsetDate, 
					fHeight - fBorderMarginV);
				grw.RelLineTo (new Distance{ Dx = -3, Dy = 0 });
				grw.RelLineTo (new Distance{ Dx = 3, Dy = -3 });
				grw.RelLineTo (new Distance{ Dx = 3, Dy = 3 });
				grw.RelLineTo (new Distance{ Dx = -3, Dy = 0 });
				grw.Stroke();
			}
			return true;
		}

		#endregion

		// todo : implement refresh
		public void Refresh()
		{
			if (!ReadOnly) {
				//this.GdkWindow.ClearArea(fX,fY,fWidth,fHeight);
				//this.GdkWindow.Show();
			}
		}
	}
}