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
		
		public Gdk.Window DrawingArea  { get;set; }
		
		public Pango.Context PangoContext {	get;set; }
		
		public Gdk.GC AxisGC { get;set; }
		
		public Gdk.GC ActorLabelGC { get;set; }
		
		public Gdk.GC DateLabelGC { get;set; }
		
		public Gdk.GC TaskLabelGC { get;set; }
		
		public Gdk.GC DateNowGC { get;set; }
		
		public int X { get;private set; }
		
		public int Y { get;private set; }
		
		public int Width { get;private set; }
		
		public int Height { get;private set; }
		
		public int Depth { get;private set; }
		
		public void CreateDiagramm(Gdk.Window drawingArea)
		{
			DrawingArea = drawingArea;
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
			int fX, fY, fWidth, fHeight, fDepth;
			DrawingArea.GetGeometry(out fX,out fY,out fWidth,out fHeight,out fDepth);
			X = fX;
			Y = fY;
			Width = fWidth;
			Height = fHeight;
			Depth = fDepth;
			Width -= 3;
			Height -= 3;
		}
		
		public void ResetAxisGC()
		{
			AxisGC = new Gdk.GC((Drawable)DrawingArea);
			Gdk.Color foregroundColor = new Gdk.Color(0xff, 0, 0);
			Colormap colormap = Colormap.System;
			colormap.AllocColor(ref foregroundColor,true,true);
			AxisGC.Foreground = foregroundColor;
		}
		
		public void ResetActorLabelGC()
		{
			ActorLabelGC = new Gdk.GC((Drawable)DrawingArea);
			Gdk.Color foregroundColor = new Gdk.Color(0, 0, 0xff);
			Colormap colormap = Colormap.System;
			colormap.AllocColor(ref foregroundColor,true,true);
			ActorLabelGC.Foreground = foregroundColor;
		}
		
		public void ResetDateLabelGC()
		{
			DateLabelGC = new Gdk.GC((Drawable)DrawingArea);
			Gdk.Color foregroundColor = new Gdk.Color(0, 0, 0xff);
			Colormap colormap = Colormap.System;
			colormap.AllocColor(ref foregroundColor,true,true);
			DateLabelGC.Foreground = foregroundColor;   
		}
		
		public void ResetTaskLabelGC()
		{
			TaskLabelGC = new Gdk.GC((Drawable)DrawingArea);
			Gdk.Color foregroundColor = new Gdk.Color(0xff, 0xff, 0xff);
			Colormap colormap = Colormap.System;
			colormap.AllocColor(ref foregroundColor,true,true);
			TaskLabelGC.Foreground = foregroundColor;
		}
		
		public void ResetDateNowGC()
		{
			DateNowGC = new Gdk.GC((Drawable)DrawingArea);
			Gdk.Color foregroundColor = new Gdk.Color(0, 0, 0);
			Colormap colormap = Colormap.System;
			colormap.AllocColor(ref foregroundColor,true,true);
			DateNowGC.Foreground = foregroundColor;
		}
		
		public void BindDrawingArea()
		{
			UpdateGepmetry();
		}
		
		#region IGuiAssigment Implementation
		
		public DataSet AssigmentSource { get;set; }
		
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
			DrawingArea.DrawRectangle(AxisGC, false, fBorderMarginH, fBorderMarginV, Width - fBorderMarginH, Height - fBorderMarginV);
		}
		
		private void DrawActorAxis()
		{
			int delta =	(AssigmentSource.Tables["Actor"].Rows.Count > 0) ? (Height - 2 * fBorderMarginV) / AssigmentSource.Tables["Actor"].Rows.Count : Height - 2 * fBorderMarginV;
			int offset = fBorderMarginV; 
			
			foreach(DataRow row in AssigmentSource.Tables["Actor"].Rows)
			{
				Pango.Layout layout = new Pango.Layout(PangoContext);
				layout.Wrap = Pango.WrapMode.Word;
				layout.FontDescription = FontDescription.FromString("Tahoma 10");
				layout.SetMarkup((string)row["Name"]);
				
				DrawingArea.DrawLayout(ActorLabelGC, fBorderMarginH,offset - fBorderMarginV,layout);
				DrawingArea.DrawLine(AxisGC, fBorderMarginH, offset, Width - fBorderMarginH, offset);
				offset += delta; 
			}
		} 
		
		private void DrawDateAxis()
		{

			DateTime firstDate = (DateTime)AssigmentSource.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)AssigmentSource.Tables["DataRange"].Rows[0]["MaxDate"];
			TimeSpan deltaSpan = lastDate.Subtract(firstDate);
			int delta = (deltaSpan.Days > 0) ? (Width - 2 * fBorderMarginH) / deltaSpan.Days : (Width - 2 * fBorderMarginH);
			int offset = fBorderMarginH; 
			
			DateTime labelDate = firstDate;
			for (int i = 0; i < deltaSpan.Days; i++)
			{
				Pango.Layout layout = new Pango.Layout(PangoContext);
				layout.Wrap = Pango.WrapMode.Word;
				layout.FontDescription = FontDescription.FromString("Tahoma 10");
				layout.SetMarkup(labelDate.ToString("dd/MM"));
				
				DrawingArea.DrawLayout(DateLabelGC, offset + fBorderMarginH,Height -2 * fBorderMarginV - this.fTaskHeight,layout);
							
				DrawingArea.DrawLine(AxisGC,offset, fBorderMarginV, offset, Height - fBorderMarginV);
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
			int deltaActor = (AssigmentSource.Tables["Actor"].Rows.Count > 0) ? (Height - 2 * fBorderMarginV) / AssigmentSource.Tables["Actor"].Rows.Count : Height - 2 * fBorderMarginV;
			deltaActor -= fTaskHeight; 
			DateTime firstDate = (DateTime)AssigmentSource.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)AssigmentSource.Tables["DataRange"].Rows[0]["MaxDate"];
			TimeSpan deltaSpan = lastDate.Subtract(firstDate);
			int delta = (deltaSpan.Days > 0) ? (Width - 2 * fBorderMarginH) / deltaSpan.Days : (Width - 2 * fBorderMarginH);
			
			int maxTaskCout = 0;
			foreach(DataRow row in this.AssigmentSource.Tables["AssigmentSource"].Rows)
			{
				if (maxTaskCout < (int)row["TaskCount"])
				{
					maxTaskCout = (int)row["TaskCount"];
				}
			}		
			
			int actorIndex = 0;
			foreach(DataRow row in AssigmentSource.Tables["Actor"].Rows)
			{
				DateTime labelDate = firstDate;
				int offset = fBorderMarginH;
				for (int i = 0; i < deltaSpan.Days; i++)
				{
					int taskCount = 0;
					if (AssigmentSource.Tables["AssigmentSource"].Select("ActorID = " + row["ID"] + " and Date = '" +labelDate.ToShortDateString() + "'").Length > 0)
						taskCount = (int)(AssigmentSource.Tables["AssigmentSource"].Select("ActorID = " + row["ID"] + "and Date = '" +labelDate.ToShortDateString() + "'")[0]["TaskCount"]);
					if (taskCount > 0)
					{
						DrawingArea.DrawRectangle(AxisGC, true,offset, fBorderMarginV + (int)(deltaActor*(1 - (double)taskCount / maxTaskCout)) + deltaActor * actorIndex,delta,(int)(deltaActor*((double)taskCount / maxTaskCout)) - fBorderMarginV);

						Pango.Layout layout = new Pango.Layout(PangoContext);
						layout.Wrap = Pango.WrapMode.Word;
						layout.FontDescription = FontDescription.FromString("Tahoma 10");
						layout.SetMarkup(taskCount.ToString());
						
						DrawingArea.DrawLayout(TaskLabelGC, offset, (int)(deltaActor*(1 - (double)taskCount / maxTaskCout)) + deltaActor * actorIndex,layout);
					}
					offset += delta;
					labelDate = labelDate.AddDays(1);
				}
				actorIndex++;
			}
		}
		
		private void DrawDateNow()
		{
			DateTime firstDate = (DateTime)AssigmentSource.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)AssigmentSource.Tables["DataRange"].Rows[0]["MaxDate"];
			TimeSpan deltaSpan = lastDate.Subtract(firstDate);
			TimeSpan nowSpan = DateTime.Now.Subtract(firstDate);
			 			 
			int delta = (deltaSpan.Days > 0) ? (Width - 2 * fBorderMarginH) / deltaSpan.Days : (Width - 2 * fBorderMarginH);  	
			int offset = fBorderMarginH + (int)(delta*(double)nowSpan.Ticks / TimeSpan.TicksPerDay);
			
			DrawingArea.DrawLine(DateNowGC,offset, fBorderMarginV, offset, Height - fBorderMarginV);
			DrawingArea.DrawPolygon(DateNowGC,true,new Gdk.Point [] {new Gdk.Point(offset,Height - 5), new Gdk.Point(offset + 5,Height), new Gdk.Point(offset - 5,Height) });
		}
		
		public void Clear()
		{
			if (DrawingArea != null)
			{
				DrawingArea.ClearArea(X,Y,Width,Height);
				DrawingArea.Show();
			}
		}
	}
}