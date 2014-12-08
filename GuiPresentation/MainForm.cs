//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 08.11.2005 at 23:25

using System;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Data;
using Gtk;
using Glade;
using Gdk;
using Pango; 

using GanttTracker;
using GanttTracker.TaskManager.ManagerException;
using GanttMonoTracker.ExceptionPresentation;
using GanttMonoTracker.DrawingPresentation; 
using TaskManagerInterface;

namespace GanttMonoTracker.GuiPresentation
{
	public class MainForm : Gtk.Window, IGuiTracker
	{
		// id, description, start time, end time, actor, status
		TreeStore fTaskStore = new TreeStore(typeof(int), typeof(string),typeof(string), typeof(string),typeof(string),typeof(string));


		TreeStore fActorStore = new TreeStore(typeof(int),typeof(string),typeof(string));


		FileSelection fFileSelection; // todo : FileChooserWidget

		string selectedFile;

		string searchTask;

		#region Widgets
		
		[Glade.WidgetAttribute]
		Gtk.MenuItem miCreateProject;

		 
		[Glade.WidgetAttribute]
		Gtk.MenuItem  miOpenProject;


		[Glade.WidgetAttribute]
		Gtk.MenuItem  miRecentProject;


		[Glade.WidgetAttribute]
		Gtk.MenuItem  miSaveProject;


		[Glade.WidgetAttribute]
		Gtk.MenuItem  miCloseProject;

		 
		[Glade.WidgetAttribute]
		Gtk.MenuItem  miExit;

		 
		[Glade.WidgetAttribute]
		Gtk.MenuItem  miUpdateFromXml;

		 
		[Glade.WidgetAttribute]
		Gtk.MenuItem  miActorCreate;

		 
		[Glade.WidgetAttribute]
		Gtk.MenuItem  miActorEdit;

		 
		[Glade.WidgetAttribute]
		Gtk.MenuItem  miActorDelete;

		 
		[Glade.WidgetAttribute]
		Gtk.MenuItem  miTaskCreate;


		[Glade.WidgetAttribute]
		Gtk.MenuItem  miChangeTaskState;

		 
		[Glade.WidgetAttribute]
		Gtk.MenuItem  miAssignTask;


		[Glade.WidgetAttribute]
		Gtk.MenuItem  miStateEdit;

		 
		[Glade.WidgetAttribute]
		Gtk.MenuItem  miAbout;	

		 		 
		[Glade.WidgetAttribute]
		Gtk.TreeView tvActorTree;

		 		 		 
		[Glade.WidgetAttribute]
		Gtk.Button btnCreateTask;

		 
		[Glade.WidgetAttribute]
		Gtk.Button btnAssignTask;	

		 
		[Glade.WidgetAttribute]
		Gtk.Button btnChangeTask;


		[Glade.WidgetAttribute]
		Gtk.Button btnSearchTask;


		[Glade.WidgetAttribute]
		Gtk.Entry entSearchTask;


		[Glade.WidgetAttribute]
		Gtk.TreeView tvTaskTree;


		/// <summary>
		/// Gantt.
		/// </summary>
		[Glade.Widget()]
		Gtk.VBox vbox3;


		Gtk.DrawingArea drwGantt;
		 

		/// <summary>
		/// Assigment.
		/// </summary>
		[Glade.Widget()]
		Gtk.VBox vbox4;


		Gtk.DrawingArea drwAssigment;


		bool [] taskSort = new bool[6];
		 
		#endregion

		public MainForm() : base ("Gantt Tracker")
		{
			InitializeComponents();
			
			LogoForm logoWindow = new LogoForm();
			logoWindow.ShowDialog();
			 
			TrackerCore.Instance.Tracker = this;
			
			TrackerCore.Instance.State = CoreState.EmptyProject;
			TrackerCore.Instance.BindProject();
		}

		
		private void InitializeComponents()
		{
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MainForm.glade");
			Glade.XML glade = new Glade.XML(stream, "MainForm", null);
			stream.Close();
			glade.Autoconnect(this);
			this.IconList = new Pixbuf[]{ new Pixbuf(System.Reflection.Assembly.GetExecutingAssembly(), "GMTLogo.bmp")};			
			this.Icon = this.IconList[0];
			
			if (miCreateProject != null)
				miCreateProject.Activated += new EventHandler(OnCreateProject);
			else throw new KeyNotFoundException<string>("miCreateProject");
			miOpenProject.Activated += new EventHandler(OnOpenProject);
			miRecentProject.Activated += new EventHandler(OnRecentProject);
			miSaveProject.Activated += new EventHandler(OnSaveProject);
			miCloseProject.Activated += new EventHandler(OnCloseProject);
			miExit.Activated += new EventHandler(OnExitProgramm);
			
			miActorCreate.Activated += new EventHandler(OnActorCreate);
		 	miActorEdit.Activated += new EventHandler(OnActorEdit);
		 	miActorDelete.Activated += new EventHandler(OnActorDelete);
		 
		 	miTaskCreate.Activated += new EventHandler(OnTaskCreate);
			miChangeTaskState.Activated += new EventHandler(OnChangeTaskState);
			miAssignTask.Activated += new EventHandler(OnTaskAssign);
			
			miStateEdit.Activated += new EventHandler(OnStateEdit);
			
			miUpdateFromXml.Activated += new EventHandler(OnUpdateFromXml);
			
			miAbout.Activated += new EventHandler(OnAbout);
			
			btnCreateTask.Clicked += new EventHandler(OnTaskCreate);
			btnAssignTask.Clicked += new EventHandler(OnTaskAssign);
			btnChangeTask.Clicked += new EventHandler(OnChangeTaskState);
			btnSearchTask.Clicked += new EventHandler(OnSearchTask);

			tvActorTree.HeadersVisible = true;
			tvActorTree.AppendColumn("Name",new CellRendererText(), "text", 1);
			tvActorTree.AppendColumn("Email",new CellRendererText(), "text", 2);
			//tvActorTree.AppendColumn("Id",new CellRendererText(), "text", 2).Visible = false;			
			
			tvTaskTree.HeadersVisible = true;
			tvTaskTree.AppendColumn("Id",new CellRendererText(), "text", 0);
			tvTaskTree.AppendColumn("Description",new CellRendererText(), "text", 1);
			tvTaskTree.AppendColumn("Start Time",new CellRendererText(), "text", 2);
			tvTaskTree.AppendColumn("End Time",new CellRendererText(), "text", 3);
			tvTaskTree.AppendColumn("Actor",new CellRendererText(), "text", 4);
			tvTaskTree.AppendColumn("State",new CellRendererText(), "text", 5);

			tvTaskTree.ButtonPressEvent+= HandleButtonPressEvent;

			// Assigment
			drwAssigment = new AssigmentDiagramm ();
			vbox4.Add (drwAssigment);
			drwAssigment.Show ();

			drwGantt = new GanttDiagramm ();
			vbox3.Add (drwGantt);
			drwGantt.Show ();



			for (int i = 2; i < 5; i++) {

				fTaskStore.SetSortFunc(i, delegate(TreeModel model, TreeIter a, TreeIter b) {
					string s1 = model.GetValue (a, 0).ToString ();
					string s2 = model.GetValue (b, 0).ToString ();
					return String.Compare (s1, s2);
				});
			}

			fTaskStore.SetSortFunc(0, delegate(TreeModel model, TreeIter a, TreeIter b) {
				var s1 = (int)model.GetValue (a, 0);
				var s2 = (int)model.GetValue (b, 0);
				return s1 > s2 ? -1 : s1 == s2 ? 0 : 1;
			});

			tvTaskTree.Model = fTaskStore;
			var index = 0;
			tvTaskTree.Columns.ToList().ForEach(c => {
				c.Clickable = true;
				c.SortColumnId = index++;
				c.Clicked+= (object sender, EventArgs e) => {
					taskSort[c.SortColumnId] = !taskSort[c.SortColumnId];
					fTaskStore.SetSortColumnId(c.SortColumnId, taskSort[c.SortColumnId] ? SortType.Ascending : SortType.Descending);
				};
			});
		}

		[GLib.ConnectBefore]
		void HandleButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if(args.Event.Type == EventType.TwoButtonPress)
			{
				OnChangeTaskState(o, EventArgs.Empty);
			}
		}

		
		private void OnWindowDeleteEvent (object sender, DeleteEventArgs a) 
		{
			a.RetVal = true;
			Application.Quit ();
		}

		
		private void OnFileSelectionResponce(object sender, ResponseArgs args)
		{
			if (args == null || args.ResponseId == ResponseType.Ok)
			{	
				switch (TrackerCore.Instance.State)
				{
					case CoreState.CreateProject :
					case CoreState.OpenProject :
					TrackerCore.Instance.ProjectFileName = args == null ? selectedFile : ((FileSelection)sender).Filename;
					TrackerCore.Instance.BindProject();
					File.WriteAllText("recent.txt".GetPath (), TrackerCore.Instance.ProjectFileName);
					break;
				}
			}
		}


		public void OnRecentProject(object sender, EventArgs args)
		{
			var r = TrackerCore.Instance.Recent;
			if(!string.IsNullOrEmpty(r) && File.Exists(r))
			{
				selectedFile = r;
				TrackerCore.Instance.State = CoreState.OpenProject;
				OnFileSelectionResponce(fFileSelection, null );
			}
		}


		private void OnCreateProject(object sender, EventArgs args)
		{
			TrackerCore.Instance.State = CoreState.CreateProject;
			fFileSelection = new FileSelection("Cerate Project");
			fFileSelection.Response += new ResponseHandler(OnFileSelectionResponce);
			fFileSelection.Run();
			fFileSelection.Hide();
		}	


		private void OnOpenProject(object sender, EventArgs args)
		{
			TrackerCore.Instance.State = CoreState.OpenProject;
			fFileSelection = new FileSelection("Open Project");
			fFileSelection.Response += new ResponseHandler(OnFileSelectionResponce);
			fFileSelection.Run();
			fFileSelection.Hide();

		}


		private void OnSaveProject(object sender, EventArgs args)
		{
			TrackerCore.Instance.SaveProject();
		}

		
		private void OnCloseProject(object sender, EventArgs args)
		{
			TrackerCore.Instance.State = CoreState.EmptyProject;
			TrackerCore.Instance.BindProject();
		}

		
		private void OnExitProgramm(object sender, EventArgs args)
		{
			Application.Quit ();
		}

		
		private void OnActorCreate(object sender, EventArgs args)
		{
			TrackerCore.Instance.CreateActor();
		}

		
		private void OnActorEdit(object sender, EventArgs args)
		{
			TreeModel model;
			TreeIter iter;
						
			((TreeSelection)tvActorTree.Selection).GetSelected (out model, out iter);
							
			int actorId = -1;
			bool actionRequierd = true;
			try
			{
				object res = model.GetValue(iter,0);
				if(res != null)
				{
					actorId = (int)res;
				}
			}
			catch(Exception ex)
			{
				actionRequierd = false;
				IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex ,this);
				dialog.Title = "Actor Edit";
				dialog.ShowDialog();
			}
			if (actionRequierd && actorId != -1)
				TrackerCore.Instance.EditActor(actorId);
		}

		
		private void OnActorDelete(object sender, EventArgs args)
		{
			TreeModel model;
			TreeIter iter;
			((TreeSelection)tvActorTree.Selection).GetSelected (out model, out iter);
			int actorId = -1;
			bool actionRequired = true;
			try
			{
				object res = model.GetValue(iter,0);
				if(res != null)
				{
					actorId = (int)res;
				}
			}
			catch(Exception ex)
			{
				actionRequired = false;
				IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex ,this);
				dialog.Title = "Actor Edit";
				dialog.ShowDialog();
			}
			if (actionRequired && actorId != -1)
				TrackerCore.Instance.DeleteActor(actorId);
		}

		
		private void OnTaskCreate(object sender, EventArgs args)
		{
			TrackerCore.Instance.CreateTask();
		}


		private void OnTaskAssign(object sender, EventArgs args)
		{
			TreeModel model;
			TreeIter iter;
			((TreeSelection)tvTaskTree.Selection).GetSelected (out model, out iter);				

			int taskID = -1;
			bool actionRequired = true;
			try
			{
				object res = model.GetValue(iter,0);
				if(res != null)
				{
					taskID = (int)res;
				}
			}
			catch(Exception ex)
			{
				actionRequired = false;
				IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex ,this);
				dialog.Title = "Assign Task";
				dialog.ShowDialog();
			}
			if (actionRequired && taskID != -1)
				TrackerCore.Instance.AssignTask(taskID);
		}


		private void OnSearchTask(object sender, EventArgs args)
		{
			searchTask = entSearchTask.Text;
			BindTask ();
		}
		

		private void OnChangeTaskState(object sender, EventArgs args)
		{
			TreeModel model;
			TreeIter iter;
			((TreeSelection)tvTaskTree.Selection).GetSelected (out model, out iter);				
			int taskID = -1;
			bool actionRequired = true;
			try
			{
				object res = model.GetValue(iter,0);
				if(res != null)
				{
					taskID = (int)res;
				}
			}
			catch(Exception ex)
			{
				actionRequired = false;
				IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex ,this);
				dialog.Title = "Change Task State";
				dialog.ShowDialog();
			}
			if (actionRequired && taskID != -1)
				TrackerCore.Instance.UpdateTaskState(taskID);
		}

		
		private void OnStateEdit(object sender, EventArgs args)
		{
			try
			{
				TrackerCore.Instance.StateEdit();
			}
			catch(InvalidCastException ex)
			{
				IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex ,this);
				dialog.Title = "Actor Edit";
				dialog.ShowDialog();
			}
		}

		
		private void OnUpdateFromXml(object sender, EventArgs args)
		{
		}

		
		private void OnAbout(object sender, EventArgs args)
		{
			TrackerCore.Instance.ShowAboutDialog();
		}
		

		private void OnGanttExpose(object sender, ExposeEventArgs args)
		{
			TrackerCore.Instance.DrawGantt(drwGantt);
		}

		
		private void OnKeyPress(object sender, Gtk.KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.F2|| 
				args.Event.State == (Gdk.ModifierType.ControlMask | Gdk.ModifierType.Mod2Mask) && 
				args.Event.Key == Gdk.Key.o)
			{
				OnOpenProject(this, new EventArgs());
			}
			if (args.Event.Key == Gdk.Key.F3|| 
				args.Event.State == (Gdk.ModifierType.ControlMask | Gdk.ModifierType.Mod2Mask) && 
				args.Event.Key == Gdk.Key.s)
			{
				OnSaveProject(this, new EventArgs());
			}
			
			if (args.Event.State == (Gdk.ModifierType.ControlMask | Gdk.ModifierType.Mod2Mask) &&
				args.Event.Key == Gdk.Key.q)
			{
				OnExitProgramm(this, new EventArgs());
			}
			
			if (args.Event.State == (Gdk.ModifierType.ControlMask | Gdk.ModifierType.Mod2Mask) && 
				args.Event.Key == Gdk.Key.e)
			{
				OnCloseProject(this, new EventArgs());
			}
		
		}
		
		#region TaskGui

		public DataSet TaskSource {	get;set; }


		public void BindTask()
		{
			fTaskStore.Clear();
			try
			{
				foreach (DataRow row in TaskSource.Tables["Task"].Rows)
				{
					var actorName = ActorSource.Tables["Actor"].Select("ID = " + (int)row["ActorID"])[0]["Name"];
					var stateName = string.Empty;
					if (StateSource.Tables["TaskState"].Select("ID = " + (int)row["StateID"]).Length > 0)
					{
						stateName = (string)StateSource.Tables["TaskState"].Select("ID = " + (int)row["StateID"])[0]["Name"];
					}

					if(!string.IsNullOrEmpty(searchTask))
					{
						string text = string.Join("", row["Id"], row["Description"], row["StartTime"], actorName, stateName);
						if(!text.ToLower().Contains(searchTask.ToLower()))
						{
							continue;
						}
					}

					TreeIter itemNode = fTaskStore.AppendNode();
					fTaskStore.SetValue(itemNode,0,row["Id"]);
					fTaskStore.SetValue(itemNode,1,row["Description"]);
					fTaskStore.SetValue(itemNode,2,((DateTime)row["StartTime"]).ToShortDateString());
					fTaskStore.SetValue(itemNode,3,((DateTime)row["EndTime"]).ToShortDateString());
					fTaskStore.SetValue(itemNode,4,actorName);
					fTaskStore.SetValue(itemNode,5,stateName);				
				}
			}
			catch (Exception ex)
			{
				var m = ex.Message;
				//todo: fix ui exception
			}
		}


		public DataSet ActorSource { get; set; }

		
		public void BindActor()
		{
			fActorStore.Clear();
			foreach (DataRow row in ActorSource.Tables["Actor"].Rows)
			{
				fActorStore.AppendValues(row["ID"], row["Name"], row["Email"]);
			}
			tvActorTree.Model = fActorStore;
		}

		
		public DataSet StateSource { get; set; }

		
		public void BindState()	{ }
		
		#endregion
	}
}