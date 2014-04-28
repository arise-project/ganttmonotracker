//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 15.01.2006 at 15:07
using System;
using System.Collections;
using System.Data;
using Gdk;
using Pango; 

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;
using System.Configuration;
using GanttTracker;
using Cairo; 

namespace GanttMonoTracker.DrawingPresentation
{
	public class GanttDiagramm : Gtk.DrawingArea, IGuiGantt
	{
		public GanttDiagramm() : base()
		{
			DateNowVisible = true;
		}
		int fTaskHeight = 14;


		public DataSet GanttSource { get;set; }


		int fBorderMarginH = 2;


		int fBorderMarginV = 2;


		int X;


		int Y;


		int Width;


		int Height;


		int Depth;

		#region Public properties.

		public bool DateNowVisible	{ get;set; }


		public bool ReadOnly { get;set; }

		protected override bool OnExposeEvent(Gdk.EventExpose args)
		{
			base.OnExposeEvent (args);

			GanttSource = GanttSource ?? TrackerCore.Instance.TaskManager.GanttSource;

			//ReadGepmetry
			int fX, fY, fWidth,fHeight,fDepth;

			this.GdkWindow.GetGeometry(out fX,out fY,out fWidth,out fHeight,out fDepth);
			X = fX;
			Y = fY;
			Width = fWidth - 3;
			Height = fHeight - 3;
			Depth = fDepth;

			// Insert drawing code here.
			Cairo.Context grw = Gdk.CairoHelper.Create (this.GdkWindow);

			//DrawBorder
			grw.SetSourceRGB(0xff, 0, 0);

			grw.MoveTo(fBorderMarginH, fBorderMarginV);
			grw.LineTo(Width - fBorderMarginH, fBorderMarginV);    
			grw.LineTo(Width - fBorderMarginH, Height - fBorderMarginV);    
			grw.LineTo(fBorderMarginH, Height - fBorderMarginV);    
			grw.LineTo(fBorderMarginH, fBorderMarginV);    
			grw.Stroke();

			//DrawTasks
			int deltaActor = (GanttSource.Tables["Actor"].Rows.Count > 0) ? 
				(Height - 2 * fBorderMarginV) / GanttSource.Tables["Actor"].Rows.Count : 
				Height - 2 * fBorderMarginV;

			DateTime firstDate = (DateTime)GanttSource.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)GanttSource.Tables["DataRange"].Rows[0]["MaxDate"];
			TimeSpan deltaSpan = lastDate.Date.Subtract(firstDate.Date);

			int deltaTask = Width; 
			if (deltaSpan.Days > 1)
				deltaTask = Width / deltaSpan.Days;

			Hashtable taskCountHash = new Hashtable(); 
			foreach(DataRow row in GanttSource.Tables["Task"].Rows)
			{
				if ((int)row["ActorID"] >= 0)
				{
					int actorIndex = 0;
					foreach (DataRow actorRow in GanttSource.Tables["Actor"].Rows)
					{
						if ((int)actorRow["ID"] == (int)row["ActorID"])
							break;
						actorIndex++;
					}
					int tasksCount = 0;
					if (!taskCountHash.ContainsKey(actorIndex))
					{
						taskCountHash.Add(actorIndex,0);
					}
					else
					{
						tasksCount = (int)taskCountHash[actorIndex];
						tasksCount++;
						taskCountHash[actorIndex] = tasksCount; 
					}

					DateTime startTime = (DateTime)row["StartTime"];
					DateTime endTime = (DateTime)row["EndTime"];
					TimeSpan startSpan = startTime.Subtract(firstDate);
					TimeSpan endSpan = endTime.Subtract(firstDate);	

					Gdk.GC taskGC = new Gdk.GC((Drawable)this.GdkWindow);

					if (GanttSource.Tables["TaskState"].Select("ID = " + row["StateID"]).Length == 0)
						throw new KeyNotFoundException(row["StateID"]);

					DataRow stateRow = GanttSource.Tables["TaskState"].Select("ID = " + row["StateID"])[0];

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

				foreach(DataRow row in GanttSource.Tables["Actor"].Rows)
				{
					Pango.Layout layout = new Pango.Layout(PangoContext);
					layout.Wrap = Pango.WrapMode.Word;
					layout.FontDescription = FontDescription.FromString("Tahoma 10");
					layout.SetMarkup((string)row["Name"]);

					this.GdkWindow.DrawLayout(ActorLabelGC, 
						fBorderMarginH,
						offsetActor - fBorderMarginV,layout);
					this.GdkWindow.DrawLine(AxisGC, 
						fBorderMarginH, 
						offsetActor, 
						Width - fBorderMarginH, offsetActor);
					offsetActor += deltaActor; 
				}


				AxisGC.Dispose ();

				//DrawDateAxis

				var labelDate1 = firstDate;
				int offset1 = fBorderMarginH;
				for (int i = 0; i < deltaSpan.Days; i++)
				{
					Pango.Layout layout = new Pango.Layout(PangoContext);
					layout.Wrap = Pango.WrapMode.Word;
					layout.FontDescription = FontDescription.FromString("Tahoma 10");
					layout.SetMarkup(labelDate1.ToString("dd/MM"));

					this.GdkWindow.DrawLayout(ActorLabelGC, 
						offset1 + fBorderMarginH,
						Height -2 * fBorderMarginV - this.fTaskHeight,
						layout);

					this.GdkWindow.DrawLine(AxisGC,
						offset1, 
						fBorderMarginV, 
						offset1, 
						Height - fBorderMarginV);
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
					Height - fBorderMarginV);
				grw.RelLineTo (new Distance{ Dx = -3, Dy = 0 });
				grw.RelLineTo (new Distance{ Dx = 3, Dy = -3 });
				grw.RelLineTo (new Distance{ Dx = 3, Dy = 3 });
				grw.RelLineTo (new Distance{ Dx = -3, Dy = 0 });
				grw.Stroke();
			}
			return true;
		}
				
		#endregion

		
		private int GetRowIndex(DataTable table, DataRow searchRow)
		{
			int index = 0;
			foreach(DataRow row in table.Rows)
			{
				if (searchRow == row)
					return index;
				index++;
			}
			return -1;
		}


		public void Refresh()
		{
			if (!ReadOnly) {
				this.GdkWindow.ClearArea(X,Y,Width,Height);
				this.GdkWindow.Show();
			}
		}
	}
}