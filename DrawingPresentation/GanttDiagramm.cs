// created on 15.01.2006 at 15:07

using System;
using System.Collections;
using System.Data;
using Gdk;
using Pango; 

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface; 

namespace GanttMonoTracker.DrawingPresentation
{
	public class GanttDiagramm : IGuiGantt, IDisposable
	{
		private readonly Gdk.Color foregroundColor1;
		private readonly Gdk.Color foregroundColor2;
		private readonly Gdk.Color foregroundColor3;
		private readonly Gdk.Color foregroundColor4;
		private readonly Colormap colormap;
		private int fBorderMarginH = 2;
		private int fBorderMarginV = 2;


		public GanttDiagramm()
		{
			foregroundColor1 = new Gdk.Color(0xff, 0, 0);
			foregroundColor2 = new Gdk.Color(0, 0, 0xff);
			foregroundColor3 = new Gdk.Color(0xff, 0xff, 0xff);
			foregroundColor4 = new Gdk.Color(0, 0, 0);

			colormap = Colormap.System;
			colormap.AllocColor(ref foregroundColor1,true,true);
			colormap.AllocColor(ref foregroundColor2,true,true);
			colormap.AllocColor(ref foregroundColor3,true,true);
			colormap.AllocColor(ref foregroundColor4,true,true);
		}
		

		public Gdk.Window DrawingArea
		{
			get;
			set;
		}
		

		public Pango.Context PangoContext
		{
			get;
			set;
		}
		

		public Gdk.GC AxisGC
		{
			get;
			set;
		}
		

		public Gdk.GC ActorLabelGC
		{
			get;
			set;
		}
		

		public Gdk.GC DateLabelGC
		{
			get;
			set;
		}
		

		public Gdk.GC TaskLabelGC
		{
			get;
			set;
		}
		

		public Gdk.GC DateNowGC
		{
			get;
			set;
		}
		

		public int X
		{
			get;
			set;
		}
		

		public int Y
		{
			get;
			set;
		}
		

		public int Width
		{
			get;
			set;
		}
		
		public int Height
		{
			get;
			set;
		}
		
		public int Depth
		{
			get;
			set;
		}
		
		public void CreateDiagramm(Gdk.Window drawingArea)
		{			
			DrawingArea = drawingArea;
			BindDrawingArea();
			ResetAxisGC();
			ResetActorLabelGC();
			ResetDateLabelGC();
			ResetTaskLabelGC();
			ResetDateNowGC();
			BindGantt();
		}	
		
		public void UpdateGepmetry()
		{
			int fX, fY, fWidth,fHeight,fDepth;

			DrawingArea.GetGeometry(out fX,out fY,out fWidth,out fHeight,out fDepth);
			X = fX;
			Y = fY;
			Width = fWidth - 3;
			Height = fHeight - 3;
			Depth = fDepth;
		}
		
		public void ResetAxisGC()
		{
			if(AxisGC == null)
			{
				AxisGC = new Gdk.GC((Drawable)DrawingArea);
				AxisGC.Foreground = foregroundColor1;
			}
		}
		
		public void ResetActorLabelGC()
		{
			if(ActorLabelGC == null)
			{
				ActorLabelGC = new Gdk.GC((Drawable)DrawingArea);
				ActorLabelGC.Foreground = foregroundColor2;
			}
		}
		
		public void ResetDateLabelGC()
		{
			if(DateLabelGC == null)
			{
				DateLabelGC = new Gdk.GC((Drawable)DrawingArea);
				DateLabelGC.Foreground = foregroundColor2;
			}
		}
		
		public void ResetTaskLabelGC()
		{
			if(TaskLabelGC == null)
			{
				TaskLabelGC = new Gdk.GC((Drawable)DrawingArea);
				TaskLabelGC.Foreground = foregroundColor3;
			}
		}
		
		public void ResetDateNowGC()
		{
			if(DateNowGC == null)
			{
				DateNowGC =  new Gdk.GC((Drawable)DrawingArea);
				DateNowGC.Foreground = foregroundColor4;
			}
		}
		
		public void BindDrawingArea()
		{
			UpdateGepmetry();
		}
		
		#region IGuiGantt Implementation
		
		public DataSet GanttSource
		{
			get;
			set;
		}
		
		public bool DateNowVisible	{ get;	set; }

		public bool ReadOnly { get; set; }
	
		public void BindGantt()
		{
			if(!ReadOnly)
				Clear();
			DrawBorder();
			DrawTasks();
			DrawActorAxis();
			DrawDateAxis();
			if (DateNowVisible)
				DrawDateNow();
		}
		
		#endregion
		

		private void DrawBorder()
		{
			DrawingArea.DrawRectangle(AxisGC, false, fBorderMarginH, fBorderMarginV, Width - fBorderMarginH, Height - fBorderMarginV);
		}
		
		private void DrawActorAxis()
		{
			int delta =	(GanttSource.Tables["Actor"].Rows.Count > 0) ? (Height - 2 * fBorderMarginV) / GanttSource.Tables["Actor"].Rows.Count : Height - 2 * fBorderMarginV;
			int offset = fBorderMarginV; 
			
			foreach(DataRow row in GanttSource.Tables["Actor"].Rows)
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

			DateTime firstDate = (DateTime)GanttSource.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)GanttSource.Tables["DataRange"].Rows[0]["MaxDate"];
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
			int deltaActor = (GanttSource.Tables["Actor"].Rows.Count > 0) ? (Height - 2 * fBorderMarginV) / GanttSource.Tables["Actor"].Rows.Count : Height - 2 * fBorderMarginV;
			
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
					
					Gdk.GC taskGC = new Gdk.GC((Drawable)DrawingArea);
					
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
							DrawingArea.DrawRectangle(taskGC, true, startSpan.Days * deltaTask + fBorderMarginH, actorIndex * deltaActor + fBorderMarginV + fTaskHeight * (tasksCount+1), (endSpan.Days - startSpan.Days) * deltaTask + deltaTask, fTaskHeight);
					else
						if (startTime.Date < lastDate.Date)
							DrawingArea.DrawRectangle(taskGC, true, startSpan.Days * deltaTask + fBorderMarginH, actorIndex * deltaActor + fBorderMarginV + fTaskHeight * (tasksCount+1), (endSpan.Days - startSpan.Days) * deltaTask + fBorderMarginH, fTaskHeight);
						else
							DrawingArea.DrawRectangle(taskGC, true, (startSpan.Days -1) * deltaTask + fBorderMarginH, actorIndex * deltaActor + fBorderMarginV + fTaskHeight * (tasksCount+1), (endSpan.Days - startSpan.Days + 1) * deltaTask + fBorderMarginH, fTaskHeight);
						
						Pango.Layout layout = new Pango.Layout(PangoContext);
						layout.Wrap = Pango.WrapMode.Word;
						layout.FontDescription = FontDescription.FromString("Tahoma 10");
						layout.SetMarkup(row["ID"].ToString());
						
						DrawingArea.DrawLayout(TaskLabelGC, startSpan.Days * deltaTask + fBorderMarginH,actorIndex * deltaActor + fTaskHeight * (tasksCount+1),layout);
				}
			}
		} 		
		
		private void DrawDateNow()
		{
			DateTime firstDate = (DateTime)GanttSource.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)GanttSource.Tables["DataRange"].Rows[0]["MaxDate"];
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

		#region IDisposable implementation

		public void Dispose ()
		{

		}

		#endregion
	}
}