// created on 04.02.2006 at 18:33

using System;
using System.Collections;
using System.Data;
using Gdk;
using Pango; 

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface; 

namespace GanttMonoTracker.DrawingPresentation
{
	public class AssigmentDiagramm : IGuiAssigment 
	{
		public AssigmentDiagramm()
		{			
		}
		
		private Gdk.Window fDrawingArea;
		public Gdk.Window DrawingArea
		{
			get
			{
				return fDrawingArea;
			}
			set
			{
				fDrawingArea = value;
			}
		}
		
		private Pango.Context fPangoContext;
		public Pango.Context PangoContext
		{
			get
			{
				return fPangoContext;
			}
			set
			{
				fPangoContext = value;
			}			
		}
		
		private Gdk.GC fAxisGC;
		public Gdk.GC AxisGC
		{
			get
			{
				return fAxisGC;
			}
			set
			{
				fAxisGC = value;
			}
		}
		
		private Gdk.GC fActorLabelGC;
		public Gdk.GC ActorLabelGC
		{
			get
			{
				return fActorLabelGC;
			}
			set
			{
				fActorLabelGC = value;
			}
		}
		
		private Gdk.GC fDateLabelGC;
		public Gdk.GC DateLabelGC
		{
			get
			{
				return fDateLabelGC;
			}
			set
			{
				fDateLabelGC = value;
			}
		}
		
		private Gdk.GC fTaskLabelGC;
		public Gdk.GC TaskLabelGC
		{
			get
			{
				return fTaskLabelGC;
			}
			set
			{
				fTaskLabelGC = value;
			}
		}
		
		private Gdk.GC fDateNowGC;
		public Gdk.GC DateNowGC
		{
			get
			{
				return fDateNowGC;
			}
			set
			{
				fDateNowGC = value;
			}
		}			
		
		private int fX;
		public int X
		{
			get
			{
				return fX;
			}
			set
			{
				throw new ImplementationException();
			}
		}
		
		private int fY;
		public int Y
		{
			get
			{
				return fY;
			}
			set
			{
				throw new ImplementationException();
			}
		}
		
		private int fWidth;
		public int Width
		{
			get
			{
				return fWidth;
			}
			set
			{
				throw new ImplementationException();
			}
		}
		
		private int fHeight;
		public int Height
		{
			get
			{
				return fHeight;
			}
			set
			{
				throw new ImplementationException();
			}
		}
		
		private int fDepth;
		public int Depth
		{
			get
			{
				return fDepth;
			}
			set
			{
				throw new ImplementationException();
			}
		}		
		
		public void CreateDiagramm(Gdk.Window drawingArea)
		{			
			fDrawingArea = drawingArea;					
			BindDrawingArea();	
			ResetAxisGC();		
			ResetActorLabelGC();
			ResetDateLabelGC();
			ResetTaskLabelGC();
			ResetDateNowGC();
			BindAssigment();	 
		}	
		
		public void UpdateGepmetry()
		{
			fDrawingArea.GetGeometry(out fX,out fY,out fWidth,out fHeight,out fDepth);
			fWidth -= 3;
			fHeight -= 3;
		}
		
		public void ResetAxisGC()
		{
			fAxisGC = new Gdk.GC((Drawable)fDrawingArea);
			Gdk.Color foregroundColor = new Gdk.Color(0xff, 0, 0);
			Colormap colormap = Colormap.System;
			colormap.AllocColor(ref foregroundColor,true,true);
			fAxisGC.Foreground = foregroundColor;   
		}
		
		public void ResetActorLabelGC()
		{			 
			fActorLabelGC = new Gdk.GC((Drawable)fDrawingArea);
			Gdk.Color foregroundColor = new Gdk.Color(0, 0, 0xff);
			Colormap colormap = Colormap.System;
			colormap.AllocColor(ref foregroundColor,true,true);
			fActorLabelGC.Foreground = foregroundColor;   
		}
		
		public void ResetDateLabelGC()
		{			 
			fDateLabelGC = new Gdk.GC((Drawable)fDrawingArea);
			Gdk.Color foregroundColor = new Gdk.Color(0, 0, 0xff);
			Colormap colormap = Colormap.System;
			colormap.AllocColor(ref foregroundColor,true,true);
			fDateLabelGC.Foreground = foregroundColor;   
		}
		
		public void ResetTaskLabelGC()
		{			 
			fTaskLabelGC = new Gdk.GC((Drawable)fDrawingArea);
			Gdk.Color foregroundColor = new Gdk.Color(0xff, 0xff, 0xff);
			Colormap colormap = Colormap.System;
			colormap.AllocColor(ref foregroundColor,true,true);
			fTaskLabelGC.Foreground = foregroundColor;   
		}
		
		public void ResetDateNowGC()
		{			 
			fDateNowGC = new Gdk.GC((Drawable)fDrawingArea);
			Gdk.Color foregroundColor = new Gdk.Color(0, 0, 0);
			Colormap colormap = Colormap.System;
			colormap.AllocColor(ref foregroundColor,true,true);
			fDateNowGC.Foreground = foregroundColor;   
		}		
		
		public void BindDrawingArea()
		{
			UpdateGepmetry();
		}
		
		#region IGuiAssigment Implementation	
		
		private DataSet fAssigmentSource;
		public DataSet AssigmentSource
		{
			get
			{
				return fAssigmentSource;
			}
			
			set
			{
				fAssigmentSource = value;
			}
		}
		
		public void BindAssigment()
		{
			Clear();
			DrawBorder();
			DrawTasks();
			DrawActorAxis();
			DrawDateAxis();
			DrawDateNow();			
		}
		
		#endregion
		
		private int fBorderMarginH = 2;
		private int fBorderMarginV = 2;
		
		private void DrawBorder()
		{			
			fDrawingArea.DrawRectangle(fAxisGC, false, fBorderMarginH, fBorderMarginV, fWidth - fBorderMarginH, fHeight - fBorderMarginV);			
		}
		
		private void DrawActorAxis()
		{			
			int delta =	(fAssigmentSource.Tables["Actor"].Rows.Count > 0) ? (fHeight - 2 * fBorderMarginV) / fAssigmentSource.Tables["Actor"].Rows.Count : fHeight - 2 * fBorderMarginV;
			int offset = fBorderMarginV; 
			
			foreach(DataRow row in fAssigmentSource.Tables["Actor"].Rows)
			{
				Pango.Layout layout = new Pango.Layout(fPangoContext);
				layout.Wrap = Pango.WrapMode.Word;
				layout.FontDescription = FontDescription.FromString("Tahoma 10");
				layout.SetMarkup((string)row["Name"]);				
				
				fDrawingArea.DrawLayout(fActorLabelGC, fBorderMarginH,offset - fBorderMarginV,layout);
				fDrawingArea.DrawLine(fAxisGC, fBorderMarginH, offset, fWidth - fBorderMarginH, offset);
				offset += delta; 
			}						
		} 
		
		private void DrawDateAxis()
		{

			DateTime firstDate = (DateTime)fAssigmentSource.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)fAssigmentSource.Tables["DataRange"].Rows[0]["MaxDate"];
			TimeSpan deltaSpan = lastDate.Subtract(firstDate); 			 
			int delta = (deltaSpan.Days > 0) ? (fWidth - 2 * fBorderMarginH) / deltaSpan.Days : (fWidth - 2 * fBorderMarginH);  	
			int offset = fBorderMarginH; 
			
			DateTime labelDate = firstDate;
			for (int i = 0; i < deltaSpan.Days; i++)
			{
				Pango.Layout layout = new Pango.Layout(fPangoContext);
				layout.Wrap = Pango.WrapMode.Word;
				layout.FontDescription = FontDescription.FromString("Tahoma 10");
				layout.SetMarkup(labelDate.ToString("dd/MM"));				
				
				fDrawingArea.DrawLayout(fDateLabelGC, offset + fBorderMarginH,fHeight -2 * fBorderMarginV - this.fTaskHeight,layout);
							
				fDrawingArea.DrawLine(fAxisGC,offset, fBorderMarginV, offset, fHeight - fBorderMarginV);				
				offset += delta;
				labelDate = labelDate.AddDays(1);				 
			}			 
		}
		
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
		
		private int fTaskHeight = 14;
		private void DrawTasks()
		{
			int deltaActor = (fAssigmentSource.Tables["Actor"].Rows.Count > 0) ? (fHeight - 2 * fBorderMarginV) / fAssigmentSource.Tables["Actor"].Rows.Count : fHeight - 2 * fBorderMarginV;
			deltaActor -= fTaskHeight; 
			DateTime firstDate = (DateTime)fAssigmentSource.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)fAssigmentSource.Tables["DataRange"].Rows[0]["MaxDate"];
			TimeSpan deltaSpan = lastDate.Subtract(firstDate); 			 
			int delta = (deltaSpan.Days > 0) ? (fWidth - 2 * fBorderMarginH) / deltaSpan.Days : (fWidth - 2 * fBorderMarginH);
			
			int maxTaskCout = 0;
			foreach(DataRow row in this.AssigmentSource.Tables["AssigmentSource"].Rows)
			{
				if (maxTaskCout < (int)row["TaskCount"])
				{
					maxTaskCout = (int)row["TaskCount"];
				}
			}		
			
			int actorIndex = 0;
			foreach(DataRow row in fAssigmentSource.Tables["Actor"].Rows)
			{
				DateTime labelDate = firstDate;
				int offset = fBorderMarginH;				
				for (int i = 0; i < deltaSpan.Days; i++)
				{																				
					int taskCount = 0;
					if (fAssigmentSource.Tables["AssigmentSource"].Select("ActorID = " + row["ID"] + " and Date = '" +labelDate.ToShortDateString() + "'").Length > 0)
						taskCount = (int)(fAssigmentSource.Tables["AssigmentSource"].Select("ActorID = " + row["ID"] + "and Date = '" +labelDate.ToShortDateString() + "'")[0]["TaskCount"]);
					if (taskCount > 0)
					{
						fDrawingArea.DrawRectangle(fAxisGC, true,offset, fBorderMarginV + (int)(deltaActor*(1 - (double)taskCount / maxTaskCout)) + deltaActor * actorIndex,delta,(int)(deltaActor*((double)taskCount / maxTaskCout)) - fBorderMarginV);

						Pango.Layout layout = new Pango.Layout(fPangoContext);
						layout.Wrap = Pango.WrapMode.Word;
						layout.FontDescription = FontDescription.FromString("Tahoma 10");
						layout.SetMarkup(taskCount.ToString());
						
						fDrawingArea.DrawLayout(fTaskLabelGC, offset, (int)(deltaActor*(1 - (double)taskCount / maxTaskCout)) + deltaActor * actorIndex,layout);
					}				
					offset += delta;
					labelDate = labelDate.AddDays(1);				 
				}
				actorIndex++;				
			}
		}
		
		private void DrawDateNow()
		{
			DateTime firstDate = (DateTime)fAssigmentSource.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)fAssigmentSource.Tables["DataRange"].Rows[0]["MaxDate"];
			TimeSpan deltaSpan = lastDate.Subtract(firstDate);
			TimeSpan nowSpan = DateTime.Now.Subtract(firstDate);			
			 			 
			int delta = (deltaSpan.Days > 0) ? (fWidth - 2 * fBorderMarginH) / deltaSpan.Days : (fWidth - 2 * fBorderMarginH);  	
			int offset = fBorderMarginH + (int)(delta*(double)nowSpan.Ticks / TimeSpan.TicksPerDay);
			
			fDrawingArea.DrawLine(fDateNowGC,offset, fBorderMarginV, offset, fHeight - fBorderMarginV);			
			fDrawingArea.DrawPolygon(fDateNowGC,true,new Gdk.Point [] {new Gdk.Point(offset,fHeight - 5), new Gdk.Point(offset + 5,fHeight), new Gdk.Point(offset - 5,fHeight) });
		} 		
		
		public void Clear()
		{
			if (fDrawingArea != null)
			{
				fDrawingArea.ClearArea(fX,fY,fWidth,fHeight);
				fDrawingArea.Show();
			}			
		}
	}
}