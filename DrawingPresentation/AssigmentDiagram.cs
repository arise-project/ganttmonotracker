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

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface; 

namespace GanttMonoTracker.DrawingPresentation
{
	// TODO : for run this Pando and Cairo understanding are required.
	// Disabled for now
	public class AssigmentDiagramm : IGuiAssigment, IDisposable
	{
		int Depth;

		private Gdk.Color foregroundColor = new Gdk.Color(0xff, 0, 0);


		private Gdk.Color foregroundColor1 = new Gdk.Color(0, 0, 0xff);


		private Gdk.Color foregroundColor3 = new Gdk.Color(0xff, 0xff, 0xff);


		private Gdk.Color foregroundColor4 = new Gdk.Color(0, 0, 0);


		private int fBorderMarginH = 2;


		private int fBorderMarginV = 2;


		private int fTaskHeight = 14;


		private Gdk.Window DrawingArea;


		public Pango.Context PangoContext {get; set; }


		private Gdk.GC AxisGC;


		private Gdk.GC ActorLabelGC;


		private Gdk.GC DateLabelGC;


		private Gdk.GC TaskLabelGC;


		private Gdk.GC DateNowGC;


		private int X;


		private int Y;


		private int Width;


		private int Height;

		private bool HasLabels
		 {
			get 
			{
				return AxisGC != null 
					&& ActorLabelGC != null 
					&& DateLabelGC != null 
					&& TaskLabelGC != null 
					&& DateNowGC != null;
			}
		}


		public void CreateDiagramm(Gdk.Window drawingArea)
		{
			if(!HasLabels) SetupLabels();
			DrawingArea = drawingArea;
			UpdateGepmetry();
			BindAssigment();
		}


		private void SetupLabels()
		{
			Colormap colormap = Colormap.System;

			AxisGC = new Gdk.GC((Drawable)DrawingArea);
			ActorLabelGC = new Gdk.GC((Drawable)DrawingArea);
			DateLabelGC = new Gdk.GC((Drawable)DrawingArea);
			TaskLabelGC = new Gdk.GC((Drawable)DrawingArea);
			DateNowGC = new Gdk.GC((Drawable)DrawingArea);

			colormap.AllocColor(ref foregroundColor,true,true);
			AxisGC.Foreground = foregroundColor;

			colormap.AllocColor(ref foregroundColor1,true,true);
			ActorLabelGC.Foreground = foregroundColor1;

			colormap.AllocColor(ref foregroundColor1,true,true);
			DateLabelGC.Foreground = foregroundColor1; 

			colormap.AllocColor(ref foregroundColor3,true,true);
			TaskLabelGC.Foreground = foregroundColor3;

			colormap.AllocColor(ref foregroundColor,true,true);
			DateNowGC.Foreground = foregroundColor4;
			//colormap.Dispose();
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

		#region IGuiAssigment Implementation
		
		public DataSet AssigmentSource { get;set; }
		
		public void BindAssigment()
		{
			Clear();
			DrawBorder();
			//DrawTasks();
			//DrawActorAxis();
			//DrawDateAxis();
			//DrawDateNow();
		}
		
		#endregion

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

			var firstDate = (DateTime)AssigmentSource.Tables["DataRange"].Rows[0]["MinDate"];
			var lastDate = (DateTime)AssigmentSource.Tables["DataRange"].Rows[0]["MaxDate"];
			var deltaSpan = lastDate.Subtract(firstDate);
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
			foreach(var row in table.Rows)
			{
				if (searchRow == row) return index;
				index++;
			}
			return -1;
		}


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


		public void Dispose()
		{
			AxisGC.Dispose();
			ActorLabelGC.Dispose();
			DateLabelGC.Dispose();
			TaskLabelGC.Dispose();
			DateNowGC.Dispose();
		}
	}
}