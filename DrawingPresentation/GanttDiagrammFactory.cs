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

namespace GanttMonoTracker.DrawingPresentation
{
	public class GanttDiagramm : IGuiGantt, IDisposable
	{
		int fTaskHeight = 14;


		public DataSet GanttSource { get;set; }


		int fBorderMarginH = 2;


		int fBorderMarginV = 2;


		Gdk.Window drawingArea;


		int X;


		int Y;


		int Width;


		int Height;


		int Depth;


		Pango.Context pangoContext;

		#region Public properties.

		public bool DateNowVisible	{ get;set; }


		public bool ReadOnly { get;set; }


		/// <summary>
		/// Gets or sets the pango context.
		/// </summary>
		/// <value>The pango context.</value>
		/// Dispose context after swithc to other context, tasks tab for example.
		public Pango.Context PangoContext 
		{
			private get
			{
				return pangoContext;
			}
			set
			{
				if(pangoContext != null) 
				{
					pangoContext.Dispose();
				}
				pangoContext = value;
			}
		}


		public Gdk.Window DrawingArea 
		{
			private get
			{
				return drawingArea;
			}
			set
			{
				drawingArea = value;
				ReadGepmetry();
			} 
		}

		#endregion

		public void CreateDiagramm(Gdk.Window drawingArea)
		{
			DrawingArea = drawingArea;
			BindGantt();
		}

		/// <summary>
		/// Read drawig area size.
		/// </summary>
		public void ReadGepmetry()
		{
			int fX, fY, fWidth,fHeight,fDepth;

			DrawingArea.GetGeometry(out fX,out fY,out fWidth,out fHeight,out fDepth);
			X = fX;
			Y = fY;
			Width = fWidth - 3;
			Height = fHeight - 3;
			Depth = fDepth;
		}

		
		#region IGuiGantt Implementation
		
		public void BindGantt()
		{
			if(!ReadOnly) Clear();
			DrawBorder();
			DrawTasks();
			DrawActorAxis();
			DrawDateAxis();
			if (DateNowVisible)	DrawDateNow();
		}
		
		#endregion
		

		private void DrawBorder()
		{
			DrawingArea.DrawRectangle(
				GdkPalette.InitColor(DrawingArea, ColorEnum.Axis), 
				false, 
				fBorderMarginH, 
				fBorderMarginV, 
				Width - fBorderMarginH, 
				Height - fBorderMarginV);

			GdkPalette.DestroyColor();
		}

		
		private void DrawActorAxis()
		{
			int delta =	(GanttSource.Tables["Actor"].Rows.Count > 0) ? 
			            (Height - 2 * fBorderMarginV) / GanttSource.Tables["Actor"].Rows.Count : 
			            Height - 2 * fBorderMarginV;

			int offset = fBorderMarginV; 
			
			foreach(DataRow row in GanttSource.Tables["Actor"].Rows)
			{
				Pango.Layout layout = new Pango.Layout(PangoContext){
					Wrap = Pango.WrapMode.Word,
					FontDescription = FontDescription.FromString("Tahoma 10")
				};

				layout.SetMarkup((string)row["Name"]);
				
				DrawingArea.DrawLayout(
					GdkPalette.InitColor(DrawingArea, ColorEnum.ActorLabel), 
					fBorderMarginH,offset - fBorderMarginV,
					layout);

				GdkPalette.DestroyColor();

				DrawingArea.DrawLine(
					GdkPalette.InitColor(DrawingArea, ColorEnum.Axis), 
					fBorderMarginH,
					offset,
					Width - fBorderMarginH, 
					offset);

				GdkPalette.DestroyColor();
				offset += delta;
			}
		} 
		
		private void DrawDateAxis()
		{

			DateTime firstDate = (DateTime)GanttSource.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)GanttSource.Tables["DataRange"].Rows[0]["MaxDate"];
			TimeSpan deltaSpan = lastDate.Subtract(firstDate);

			int delta = (deltaSpan.Days > 0) ? 
			            (Width - 2 * fBorderMarginH) / deltaSpan.Days :
			            (Width - 2 * fBorderMarginH);
			int offset = fBorderMarginH; 
			
			DateTime labelDate = firstDate;
			for (int i = 0; i < deltaSpan.Days; i++)
			{
				Pango.Layout layout = new Pango.Layout(PangoContext){
					Wrap = Pango.WrapMode.Word,
					FontDescription = FontDescription.FromString("Tahoma 10")
				};
				layout.SetMarkup(labelDate.ToString("dd/MM"));
				
				DrawingArea.DrawLayout(
					GdkPalette.InitColor(DrawingArea, ColorEnum.DateLabel), 
					offset + fBorderMarginH,
					Height -2 * fBorderMarginV - this.fTaskHeight,
					layout);
				GdkPalette.DestroyColor();

				DrawingArea.DrawLine(
					GdkPalette.InitColor(DrawingArea, ColorEnum.Axis),
					offset, 
					fBorderMarginV, 
					offset, 
					Height - fBorderMarginV);
				GdkPalette.DestroyColor();

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
		
		private void DrawTasks()
		{
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
							DrawingArea.DrawRectangle(
							taskGC, 
							true,
							startSpan.Days * deltaTask + fBorderMarginH, 
							actorIndex * deltaActor + fBorderMarginV + fTaskHeight * (tasksCount+1), 
							(endSpan.Days - startSpan.Days) * deltaTask + deltaTask,
							fTaskHeight);
					else
						if (startTime.Date < lastDate.Date)
							DrawingArea.DrawRectangle(
								taskGC, 
								true,
								startSpan.Days * deltaTask + fBorderMarginH,
								actorIndex * deltaActor + fBorderMarginV + fTaskHeight * (tasksCount+1),
								(endSpan.Days - startSpan.Days) * deltaTask + fBorderMarginH,
								fTaskHeight);
						else
							DrawingArea.DrawRectangle(
								taskGC,
								true,(startSpan.Days -1) * deltaTask + fBorderMarginH,
								actorIndex * deltaActor + fBorderMarginV + fTaskHeight * (tasksCount+1),
								(endSpan.Days - startSpan.Days + 1) * deltaTask + fBorderMarginH,
								fTaskHeight);
						
					Pango.Layout layout = new Pango.Layout(PangoContext){
						Wrap = Pango.WrapMode.Word,
						FontDescription = FontDescription.FromString("Tahoma 10")
					};
					layout.SetMarkup(row["ID"].ToString());
						
					DrawingArea.DrawLayout(
						GdkPalette.InitColor(DrawingArea, ColorEnum.TaskLabel), 
						startSpan.Days * deltaTask + fBorderMarginH,
						actorIndex * deltaActor + fTaskHeight * (tasksCount+1),
						layout);

					GdkPalette.DestroyColor();
					colormap.Dispose();
					taskGC.Dispose();
				}
			}
		} 		
		
		private void DrawDateNow()
		{
			DateTime firstDate = (DateTime)GanttSource.Tables["DataRange"].Rows[0]["MinDate"];
			DateTime lastDate = (DateTime)GanttSource.Tables["DataRange"].Rows[0]["MaxDate"];
			TimeSpan deltaSpan = lastDate.Subtract(firstDate);
			TimeSpan nowSpan = DateTime.Now.Subtract(firstDate);
			 			 
			int delta = (deltaSpan.Days > 0) ?
			            (Width - 2 * fBorderMarginH) / deltaSpan.Days : 
			            (Width - 2 * fBorderMarginH);
			int offset = fBorderMarginH + (int)(delta*(double)nowSpan.Ticks / TimeSpan.TicksPerDay);
			
			DrawingArea.DrawLine(
				GdkPalette.InitColor(DrawingArea, ColorEnum.DateNow),
				offset,
				fBorderMarginV,
				offset,
				Height - fBorderMarginV);
			GdkPalette.DestroyColor();
			DrawingArea.DrawPolygon(
				GdkPalette.InitColor(DrawingArea, ColorEnum.DateNow),
				true,
				new Gdk.Point [] 
				{
					new Gdk.Point(offset,Height - 5), 
					new Gdk.Point(offset + 5,Height), 
					new Gdk.Point(offset - 5,Height) 
				});
			GdkPalette.DestroyColor();
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
			DrawingArea.Dispose();
			PangoContext.Dispose();
		}

		#endregion
	}
}