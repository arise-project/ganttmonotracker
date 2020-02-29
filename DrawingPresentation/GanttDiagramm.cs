//----------------------------------------------------------------------------------------------
// <copyright file="GanttDiagramm.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 15.01.2006 at 15:07

namespace GanttMonoTracker.DrawingPresentation
{
	using System;
	using System.Collections.Generic;
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

	public sealed class GanttDiagramm : Gtk.DrawingArea, IGuiSource, IGanttSource, IDisposable
	{
		private Cairo.Context grw;
		private Gdk.GC taskGc;

		private const int FBorderMarginH = 2;
		public const int FBorderMarginV = 2;
		public const int FTaskHeight = 14;

		bool IGanttSource.DateNowVisible { get; set; } = true;

		bool IGanttSource.ReadOnly { get; set; }

		public DataSet Source { get; set; }

		DataSet IGanttSource.StaticSource { get; set; }

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
			var baseResult = base.OnExposeEvent(evnt);
			if (Source == null && TrackerCore.Instance.TaskManager is EmptyTaskManager)
			{
				return baseResult;
			}

			Source = ((IGanttSource)this).StaticSource ?? TrackerCore.Instance.TaskManager.GanttSource;

			//ReadGepmetry
			int fX, fY, fWidth, fHeight, fDepth;

			GdkWindow.GetGeometry(out fX, out fY, out fWidth, out fHeight, out fDepth);
			fWidth -= 3;
			fHeight -= 3;

			// Insert drawing code here.

			if (grw == null) grw = Gdk.CairoHelper.Create(GdkWindow);
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

			var stateColors = new Dictionary<int, Gdk.Color>();
			var statePresence = new Dictionary<int, Dictionary<int, int>>();
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


				if (taskGc == null) taskGc = new Gdk.GC(GdkWindow);

				int stateId = (int)row["StateID"];
				if (Source.Tables["TaskState"].Select("ID = " + stateId).Length == 0)
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

						if (!stateColors.ContainsKey(stateId))
						{
							var foregroundColor = new Gdk.Color(colorRed, colorGreen, colorBlue);
							stateColors.Add(stateId, foregroundColor);
						}
					}

					catch (Exception e)
					{
						Console.WriteLine(e);
					}
				}

				int offset;
				if (lastDate.Date > endTime.Date)
					offset = startSpan.Days * deltaTask + FBorderMarginH;
				else if (startTime.Date < lastDate.Date)
					offset = startSpan.Days * deltaTask + FBorderMarginH;
				else
					offset = (startSpan.Days - 1) * deltaTask + FBorderMarginH;

				if (!statePresence.ContainsKey(offset))
					statePresence.Add(offset, new Dictionary<int, int>());
				if (!statePresence[offset].ContainsKey(stateId))
					statePresence[offset].Add(stateId, 1);
				else
					statePresence[offset][stateId]++;
			}

			//TODO: for now the diagram was disabled because it not represent any gantt presentation itself
			//draw tasks
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

				if (taskGc == null) taskGc = new Gdk.GC(GdkWindow);

				int stateId = (int)row["StateID"];
				var foregroundColor = stateColors[stateId];
				var colormap = Colormap.System;
				colormap.AllocColor(ref foregroundColor, true, true);
				taskGc.Foreground = foregroundColor;
				colormap.Dispose();

				int offset;
				if (lastDate.Date > endTime.Date)
				{
					offset = startSpan.Days * deltaTask + FBorderMarginH;
					if (statePresence.ContainsKey(offset) && statePresence[offset].ContainsKey(stateId) && statePresence[offset][stateId] > 0)
					{
						int sum = 0;
						int start = 0;
						bool found = false;
						foreach (var s in statePresence[offset].Keys)
						{
							sum += Math.Abs(statePresence[offset][s]);
						}

						foreach (var s in statePresence[offset].Keys)
						{
							found = s == stateId;
							if (!found)
								start += deltaActor* (Math.Abs(statePresence[offset][stateId]) - 1) / sum;
							else break;
						}

						GdkWindow.DrawRectangle(
							taskGc,
							true,
							startSpan.Days * deltaTask + FBorderMarginH,
							actorIndex * deltaActor + start,
							deltaTask,
							deltaActor * (statePresence[offset][stateId]) / sum);
					}
				}
				else if (startTime.Date < lastDate.Date)
				{
					offset = startSpan.Days * deltaTask + FBorderMarginH;


					if (statePresence.ContainsKey(offset) && statePresence[offset].ContainsKey(stateId) && statePresence[offset][stateId] > 0)
					{
						int sum = 0;
						int start = 0;
						bool found = false;
						foreach (var s in statePresence[offset].Keys)
						{
							sum += Math.Abs(statePresence[offset][s]);
						}

						foreach (var s in statePresence[offset].Keys)
						{
							found = s == stateId;
							if (!found)
								start += deltaActor* (Math.Abs(statePresence[offset][stateId]) - 1) / sum;
							else break;
						}

						GdkWindow.DrawRectangle(
														taskGc,
														true,
														startSpan.Days * deltaTask + FBorderMarginH,
														actorIndex * deltaActor + FBorderMarginV + start,
														deltaTask + FBorderMarginH,
														deltaActor * (statePresence[offset][stateId]) / sum);
					}
				}
				else
				{
					offset = (startSpan.Days - 1) * deltaTask + FBorderMarginH;
					if (statePresence.ContainsKey(offset) && statePresence[offset].ContainsKey(stateId) && statePresence[offset][stateId] > 0)
					{
						int sum = 0;
						int start = 0;
						bool found = false;
						foreach (var s in statePresence[offset].Keys)
						{
							sum += Math.Abs(statePresence[offset][s]);
						}

						foreach (var s in statePresence[offset].Keys)
						{
							found = s == stateId;
							if (!found)
								start += deltaActor * (Math.Abs(statePresence[offset][stateId]) - 1) / sum;
							else break;
						}

						GdkWindow.DrawRectangle(
							taskGc,
							true, (startSpan.Days - 1) * deltaTask + FBorderMarginH,
							actorIndex * deltaActor + FBorderMarginV + start,
							deltaTask + FBorderMarginH,
							deltaActor * (statePresence[offset][stateId]) / sum);
					}
				}
				statePresence[offset][stateId] = -Math.Abs(statePresence[offset][stateId]);
				GdkPalette.DestroyColor();
			}


			//DrawActorAxis

			//TODO: reuse gc
			var taskLabelGc = new Gdk.GC(GdkWindow);
			if (!((IGanttSource)this).DateNowVisible) return true;

			//DrawActorAxis
			var offsetActor = FBorderMarginV;
			var foregroundColor2 = new Gdk.Color(0xff, 0, 0);
			var foregroundColor1 = new Gdk.Color(0, 0, 0xff);
			//TODO: reuse gc
			var actorLabelGc = new Gdk.GC(GdkWindow);
			//TODO: reuse gc
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


			return true;
		}

		public override void Dispose()
		{
			base.Dispose();
			if (grw != null) grw.Dispose();
			if (taskGc != null) taskGc.Dispose();
		}
	}
}