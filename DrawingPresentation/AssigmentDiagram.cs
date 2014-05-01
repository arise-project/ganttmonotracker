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
using Cairo; 

using GanttTracker;
using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;

namespace GanttMonoTracker.DrawingPresentation
{
	public class AssigmentDiagramm : Gtk.DrawingArea, IGuiAssigment
	{
		#region Constants.

		const int fBorderMarginH = 2;


		const int fBorderMarginV = 2;


		const int fTaskHeight = 14;

		#endregion

		public AssigmentDiagramm() : base()	{ }

		#region IGuiAssigment Implementation
		
		public DataSet AssigmentSource { get;set; }
				
		#endregion

		#region Protected methods

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

			fWidth -= 3;
			fHeight -= 3;

			int delta = (deltaSpan.Days > 0) ? 
			            (fWidth - 2 * fBorderMarginH) / deltaSpan.Days :
			            (fWidth - 2 * fBorderMarginH);

			base.GdkWindow.ClearArea(fX,fY,fWidth,fHeight);
			base.Show();

			//DrawBorder
			grw.SetSourceRGB(0xff, 0, 0);

			grw.MoveTo(fBorderMarginH, fBorderMarginV);
			grw.LineTo(fWidth - fBorderMarginH, fBorderMarginV);    
			grw.LineTo(fWidth - fBorderMarginH, fHeight - fBorderMarginV);    
			grw.LineTo(fBorderMarginH, fHeight - fBorderMarginV);    
			grw.LineTo(fBorderMarginH, fBorderMarginV);    
			grw.Stroke();

			//DrawTasks
			int deltaActor = (AssigmentSource.Tables["Actor"].Rows.Count > 0) ? 
			                 (fHeight - 2 * fBorderMarginV) / AssigmentSource.Tables["Actor"].Rows.Count : 
			                 fHeight - 2 * fBorderMarginV;
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
					fWidth - fBorderMarginH, offsetActor);
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
					fHeight -2 * fBorderMarginV - fTaskHeight,
					layout);

				this.GdkWindow.DrawLine(AxisGC,
					offset1, 
					fBorderMarginV, 
					offset1, 
					fHeight - fBorderMarginV);
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
				fHeight - fBorderMarginV);
			grw.RelLineTo (new Distance{ Dx = -3, Dy = 0 });
			grw.RelLineTo (new Distance{ Dx = 3, Dy = -3 });
			grw.RelLineTo (new Distance{ Dx = 3, Dy = 3 });
			grw.RelLineTo (new Distance{ Dx = -3, Dy = 0 });
			grw.Stroke();

			return true;
		}

		#endregion
	}
}
