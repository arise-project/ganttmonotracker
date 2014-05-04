//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// Auto-generated by Glade# Code Generator
// http://eric.extremeboredom.net/projects/gladesharpcodegenerator/

using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;
using Gtk;

using GanttMonoTracker.DrawingPresentation;
using TaskManagerInterface;

namespace GanttMonoTracker.GuiPresentation
{	
	public class LogoForm: IGuiMessageDialog
	{
		private DataSet fGanttSource;


		private Gtk.Window thisWindow;


		[Glade.Widget()]
		private Gtk.TextView tvReleaseNews;


		private GanttDiagramm fLogoDiagram;


		/// <summary>
		/// The drwAssigment container.
		/// </summary>
		[Glade.Widget()]
		private Gtk.VBox vbox1;


		private Gtk.DrawingArea dwLogo;


		public LogoForm()
		{
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewLogoForm.glade");
			Glade.XML glade = new Glade.XML(stream, "LogoMindow", null);
			stream.Close();
			glade.Autoconnect(this);
			thisWindow = ((Gtk.Window)(glade.GetWidget("LogoMindow")));
			thisWindow.WindowPosition = WindowPosition.Center;
			thisWindow.SetDefaultSize(480,460);
			thisWindow.KeyReleaseEvent += (o, args) => {
				this.thisWindow.HideAll();
			};
			ReadMe();

			// Assigment
			dwLogo = new GanttDiagramm () { ReadOnly = true, Source = GetLogoSource(), DateNowVisible = false };
			var readme = vbox1.Children [1];
			var readme1 = vbox1.Children [2];
			vbox1.Remove (readme1);
			vbox1.Add (dwLogo);
			vbox1.Add (readme);
			vbox1.Add (readme1);
			dwLogo.Show ();
		}
		

		private void ReadMe()
		{
			var readme =  "Resources".GetPath();
			readme =  Path.Combine(readme,"readme.txt");
			if(File.Exists(readme))
				using(var sr = new StreamReader(readme))
				{
					var text = sr.ReadToEnd().Substring(0,256);
					text = text.Substring(0, text.LastIndexOf("\n"));
					tvReleaseNews.Buffer.Text = text;
					sr.Close();
				}
		}

			
		private DataSet GetLogoSource()
		{
			fGanttSource = GanttDiagramFactory.Create();
			var actors = fGanttSource.Tables["Actor"];
			actors.Rows.Add(new object [] {1, "Eugene Pirogov","pirogov.e@gmail.com"});
			var tasks = fGanttSource.Tables["Task"];
			var taskStates = fGanttSource.Tables["TaskState"];
			taskStates.Rows.Add(new object [] {0,"Open",0x80,0,0xFF,0xFF}); // logo color

			// breaks
			tasks.Rows.Add(new object [] {0,1,"Actors [Name, Email]",DateTime.Now.AddDays(-5),DateTime.Now.AddDays(3),0});
			tasks.Rows.Add(new object [] {1,1,"Tasks [Description, Start Time, End Time, State]",DateTime.Now.AddDays(-3),DateTime.Now.AddDays(2),0});
			tasks.Rows.Add(new object [] {2,1,"Gantt Diagramm",DateTime.Now.AddDays(-7),DateTime.Now.AddDays(4),0});
			tasks.Rows.Add(new object [] {3,1,"Assigment Diagramm",DateTime.Now.AddDays(0),DateTime.Now.AddDays(4),0});


			fGanttSource.Tables.Add("DataRange");
			fGanttSource.Tables["DataRange"].Columns.Add("MinDate",typeof(DateTime));
			fGanttSource.Tables["DataRange"].Columns.Add("MaxDate",typeof(DateTime));
					
			DataRow rangeRow = fGanttSource.Tables["DataRange"].NewRow();
			rangeRow["MinDate"] = DateTime.Now.AddDays(-5);
			rangeRow["MaxDate"] = DateTime.Now.AddDays(4);
					
			fGanttSource.Tables["DataRange"].Rows.Add(rangeRow);
					
			return fGanttSource; 
		}

		 
		public void Destroy()
		{
			thisWindow.Destroy();
		}

		
		public void Show()
		{
			thisWindow.ShowAll();
		}

		
		#region IGuiMessageDialog Implementation
		
		public int ShowDialog()
		{
			Show();
			return 0;
		}

		
		public string Title
		{
			get
			{
				return thisWindow.Title;
			}
			set
			{
				thisWindow.Title = value;
			}
		}
		
		#endregion
		
		#region IDisposable Implementation
		
		public void Dispose()
		{
			fLogoDiagram.Dispose();
			thisWindow.Dispose();
		}
		
		#endregion
	}
}