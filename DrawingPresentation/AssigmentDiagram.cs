//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 04.02.2006 at 18:33

using System;
using System.Collections;
using System.Data;
using Gdk;
using Pango; 

using GanttTracker;
using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;
using Cairo; 

namespace GanttMonoTracker.DrawingPresentation
{
	// TODO : for run this Pando and Cairo understanding are required.
	// Disabled for now
	public class AssigmentDiagramm : Gtk.DrawingArea, IGuiAssigment
	{
		public AssigmentDiagramm() : base()
		{
		}


		int Depth;


		private int fBorderMarginH = 2;


		private int fBorderMarginV = 2;


		private int fTaskHeight = 14;


		private int X;


		private int Y;


		private int Width;


		private int Height;

		#region IGuiAssigment Implementation
		
		public DataSet AssigmentSource { get;set; }
		
		protected override bool OnExposeEvent(Gdk.EventExpose args)
		{
			base.OnExposeEvent (args);

			AssigmentSource = TrackerCore.Instance.TaskManager.AssigmentSource;

			DateTime firstDate = (DateTime)AssigmentSource.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)AssigmentSource.Tables["DataRange"].Rows[0]["MaxDate"];
			TimeSpan deltaSpan = lastDate.Subtract(firstDate);


			// Insert drawing code here.
			Cairo.Context grw = Gdk.CairoHelper.Create (this.GdkWindow);

			int fX, fY, fWidth, fHeight, fDepth;

			base.GdkWindow.GetGeometry(out fX,out fY,out fWidth,out fHeight,out fDepth);

			X = fX;
			Y = fY;
			Width = fWidth;
			Height = fHeight;
			Depth = fDepth;
			Width -= 3;
			Height -= 3;

			int delta = (deltaSpan.Days > 0) ? 
				(Width - 2 * fBorderMarginH) / deltaSpan.Days :
				(Width - 2 * fBorderMarginH);

			base.GdkWindow.ClearArea(X,Y,Width,Height);
			base.Show();

			//DrawBorder
			grw.SetSourceRGB(0xff, 0, 0);

			grw.MoveTo(fBorderMarginH, fBorderMarginV);
			grw.LineTo(Width - fBorderMarginH, fBorderMarginV);    
			grw.LineTo(Width - fBorderMarginH, Height - fBorderMarginV);    
			grw.LineTo(fBorderMarginH, Height - fBorderMarginV);    
			grw.LineTo(fBorderMarginH, fBorderMarginV);    
			grw.Stroke();

			//DrawTasks

			int deltaActor = (AssigmentSource.Tables["Actor"].Rows.Count > 0) ? 
				(Height - 2 * fBorderMarginV) / AssigmentSource.Tables["Actor"].Rows.Count : 
				Height - 2 * fBorderMarginV;
			deltaActor -= fTaskHeight; 



			int maxTaskCout = 0;
			foreach(DataRow row in this.AssigmentSource.Tables["AssigmentSource"].Rows)
			{
				if (maxTaskCout < (int)row["TaskCount"])
				{
					maxTaskCout = (int)row["TaskCount"];
				}
			}		

			int actorIndex = 0;
			Gdk.Color foregroundColor3 = new Gdk.Color(0xff, 0xff, 0xff);
			Gdk.GC TaskLabelGC = new Gdk.GC (this.GdkWindow);
			Colormap colormap = Colormap.System;
			colormap.AllocColor(ref foregroundColor3,true,true);
			TaskLabelGC.Foreground = foregroundColor3;

			foreach(DataRow row in AssigmentSource.Tables["Actor"].Rows)
			{
				DateTime labelDate = firstDate;
				int offset = fBorderMarginH;
				for (int i = 0; i < deltaSpan.Days; i++)
				{
					int taskCount = 0;
					if (AssigmentSource.Tables["AssigmentSource"]
						.Select("ActorID = " + row["ID"] + " and Date = '" +labelDate.ToShortDateString() + "'")
						.Length > 0)
						taskCount = (int)(AssigmentSource.Tables["AssigmentSource"]
							.Select("ActorID = " + row["ID"] + "and Date = '" +labelDate.ToShortDateString() + "'")
							[0]["TaskCount"]);
					if (taskCount > 0)
					{
						grw.SetSourceRGB(0xff, 0, 0);
						grw.Rectangle(offset, 
							fBorderMarginV + (int)(deltaActor*(1 - (double)taskCount / maxTaskCout)) 
							+ deltaActor * actorIndex,delta,(int)(deltaActor*((double)taskCount / maxTaskCout)) 
							- fBorderMarginV);
						grw.Clip();
						grw.Paint();
						grw.ResetClip();

						Pango.Layout layout = new Pango.Layout(PangoContext);
						layout.Wrap = Pango.WrapMode.Word;
						layout.FontDescription = FontDescription.FromString("Tahoma 10");
						layout.SetMarkup(taskCount.ToString());


						this.GdkWindow.DrawLayout(TaskLabelGC, 
							offset,
							(int)(deltaActor*(1 - (double)taskCount / maxTaskCout)) + deltaActor * actorIndex,layout);

					}
					offset += delta;
					labelDate = labelDate.AddDays(1);
				}
				actorIndex++;
			}
			TaskLabelGC.Dispose ();


			//DrawActorAxis
			int offsetActor = fBorderMarginV; 
			Gdk.Color foregroundColor = new Gdk.Color(0xff, 0, 0);
			Gdk.Color foregroundColor1 = new Gdk.Color(0, 0, 0xff);
			Gdk.GC ActorLabelGC = new Gdk.GC (this.GdkWindow);
			Gdk.GC AxisGC = new Gdk.GC (this.GdkWindow);
			colormap.AllocColor(ref foregroundColor,true,true);
			TaskLabelGC.Foreground = foregroundColor;
			AxisGC.Foreground = foregroundColor1;

			foreach(DataRow row in AssigmentSource.Tables["Actor"].Rows)
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
				offset1 += delta;
				labelDate1 = labelDate1.AddDays(1);
			}

			ActorLabelGC.Dispose ();

			//DrawDateNow
			TimeSpan nowSpan = DateTime.Now.Subtract(firstDate);

			int offsetDate = fBorderMarginH + (int)(delta*(double)nowSpan.Ticks / TimeSpan.TicksPerDay);

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



			/*
			this.GdkWindow.DrawPolygon(DateNowGC,
				true,
				new Gdk.Point [] 
				{
					new Gdk.Point(offsetDate,Height - 5), 
					new Gdk.Point(offsetDate + 5,Height), 
					new Gdk.Point(offsetDate - 5,Height) 
				});
				*/


			return true;
		}
		
		#endregion



		private int GetRowIndex(DataTable table, DataRow searchRow)
		{
			int index = 0;
			foreach(var row in table.Rows)
			{
				if (searchRow == row) return index;
				index++;
			}
			return -1;
		}


	}
}