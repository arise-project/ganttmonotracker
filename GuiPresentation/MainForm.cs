// created on 08.11.2005 at 23:25

using System;
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
	public class MainForm : Gtk.Window, IGuiTracker, IGuiGantt
	{		
		// id, description, start time, end time, actor, status
		private TreeStore fTaskStore = new TreeStore(typeof(int), typeof(string),typeof(string), typeof(string),typeof(string),typeof(string));
		private TreeStore fActorStore = new TreeStore(typeof(int),typeof(string),typeof(string));
		private FileSelection fFileSelection;		

		#region Widgets
		
		 [Glade.WidgetAttribute]
		 private Gtk.MenuBar mbMain;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem miCreateProject;		 
		 
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miOpenProject;
		
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miSaveProject;
		
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miCloseProject;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miExit;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miUpdateFromXml;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miActorCreate;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miActorEdit;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miActorDelete;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miTaskCreate;
		
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miChangeTaskState;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miAssignTask;
		
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miStateEdit;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.MenuItem  miAbout;	
		 
		 [Glade.WidgetAttribute]
		 private Gtk.Notebook nbMain; 
		 
		 [Glade.WidgetAttribute]
		 private Gtk.Label lbActorDescription;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.ScrolledWindow swActorTree;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.TreeView tvActorTree;
		 		 		 
		 [Glade.WidgetAttribute]
		 private Gtk.Label lbTaskDescription;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.Label lbGanttDescription;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.Label lbAssigmentDescription;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.VBox vbox2;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.HBox hbox1;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.Button btnCreateTask;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.Button btnAssignTask;	
		 
		 [Glade.WidgetAttribute]
		 private Gtk.Button btnChangeTask;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.Entry entSearchTask;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.Button btnSearchTask;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.ScrolledWindow swTaskTree;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.TreeView tvTaskTree;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.DrawingArea drwGantt;
		 
		 [Glade.WidgetAttribute]
		 private Gtk.DrawingArea drwAssigment;	 
		 
		#endregion

		public MainForm() : base ("Gantt Tracker")
		{
			InitializeComponents();			
			
			LogoForm logoWindow = new LogoForm();
			logoWindow.ShowDialog();
			 
			TrackerCore.Instance.Tracker = this;			 
			
			TrackerCore.Instance.State = CoreState.EmptyProject;
			TrackerCore.Instance.BindProject();
			TrackerCore.Instance.GuiSource = new GuiFactory();			 
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
			else throw new KeyNotFoundException("miCreateProject");			
			miOpenProject.Activated += new EventHandler(OnOpenProject);
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
			
			drwGantt.Realized += new EventHandler(OnGanttRealize);
			drwGantt.ExposeEvent += new Gtk.ExposeEventHandler(OnGanttExpose);
			
			drwAssigment.Realized += new EventHandler(OnAssigmentRealize);
			drwAssigment.ExposeEvent += new Gtk.ExposeEventHandler(OnAssigmentExpose);
				
		}
		
		private void OnWindowDeleteEvent (object sender, DeleteEventArgs a) 
		{				
			a.RetVal = true;
			Application.Quit ();					
		}
		
		private void OnFileSelectionResponce(object sender, ResponseArgs args)
		{
			if (args.ResponseId == ResponseType.Ok)
			{	
				switch (TrackerCore.Instance.State)
				{
					case CoreState.CreateProject :
					case CoreState.OpenProject :
						TrackerCore.Instance.ProjectFileName =((FileSelection)sender).Filename;						
						TrackerCore.Instance.BindProject();			
						break;														
				}
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
				actorId = (int)model.GetValue(iter,0);
			}
			catch(Exception)
			{
				actionRequierd = false;
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateMessageDialog("Select actor to edit" ,this);
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
				actorId = (int)model.GetValue(iter,0);
			}
			catch(Exception)
			{
				actionRequired = false;
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateMessageDialog("Select actor to edit" ,this);
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
				taskID = (int)model.GetValue(iter,0);
			}
			catch(Exception)
			{
				actionRequired = false;
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateMessageDialog("Select Task to edit" ,this);
				dialog.Title = "Assign Task";
				dialog.ShowDialog();				
			}
			if (actionRequired && taskID != -1)
				TrackerCore.Instance.AssignTask(taskID);
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
				taskID = (int)model.GetValue(iter,0);
			}
			catch(Exception)
			{
				actionRequired = false;
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateMessageDialog("Select Task to edit" ,this);
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
			catch(InvalidCastException)
			{
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateMessageDialog("Select actor to edit" ,this);
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
		
		Pango.Layout layout;
		
		
		private void OnGanttRealize(object sender, EventArgs args)
		{
				 
		}		
				
		private void OnGanttExpose(object sender, ExposeEventArgs args)
		{
			TrackerCore.Instance.DrawGantt(drwGantt);
		}
		
		private void OnAssigmentRealize(object sender, EventArgs args)
		{
				 
		}		
				
		private void OnAssigmentExpose(object sender, ExposeEventArgs args)
		{
			//TrackerCore.Instance.DrawAssigment(drwAssigment);
		}
		
		private void OnKeyPress(object sender, Gtk.KeyPressEventArgs args)
		{		
			if (args.Event.Key == Gdk.Key.F2|| args.Event.State == (Gdk.ModifierType.ControlMask | Gdk.ModifierType.Mod2Mask) && args.Event.Key == Gdk.Key.o)
			{
				OnOpenProject(this, new EventArgs());
			}
			if (args.Event.Key == Gdk.Key.F3|| args.Event.State == (Gdk.ModifierType.ControlMask | Gdk.ModifierType.Mod2Mask) && args.Event.Key == Gdk.Key.s)
			{
				OnSaveProject(this, new EventArgs());
			}
			
			if (args.Event.State == (Gdk.ModifierType.ControlMask | Gdk.ModifierType.Mod2Mask) && args.Event.Key == Gdk.Key.q)
			{
				OnExitProgramm(this, new EventArgs());
			}
			
			if (args.Event.State == (Gdk.ModifierType.ControlMask | Gdk.ModifierType.Mod2Mask) && args.Event.Key == Gdk.Key.e)
			{
				OnCloseProject(this, new EventArgs());
			}
								
		}				
		
		#region TaskGui
		
		private DataSet fTaskSource;
		public DataSet TaskSource
		{
			get
			{				
				return fTaskSource;
			}
			set
			{
				fTaskSource = value;
			}
		}	
		
		public void BindTask()
		{				
			fTaskStore.Clear();
			try
			{
			foreach (DataRow row in fTaskSource.Tables["Task"].Rows)
			{
				TreeIter itemNode = fTaskStore.AppendNode();
				fTaskStore.SetValue(itemNode,0,row["Id"]);
				fTaskStore.SetValue(itemNode,1,row["Description"]);
				fTaskStore.SetValue(itemNode,2,((DateTime)row["StartTime"]).ToShortDateString());
				fTaskStore.SetValue(itemNode,3,((DateTime)row["EndTime"]).ToShortDateString());
				fTaskStore.SetValue(itemNode,4,ActorSource.Tables["Actor"].Select("ID = " + (int)row["ActorID"])[0]["Name"]);
				if (StateSource.Tables["TaskState"].Select("ID = " + (int)row["StateID"]).Length > 0)
				{
					var state = StateSource.Tables["TaskState"].Select("ID = " + (int)row["StateID"])[0]["Name"];
					fTaskStore.SetValue(itemNode,5,state);
				}
			}
			tvTaskTree.Model = fTaskStore;
			}
			catch (Exception ex)
			{
				var m = ex.Message;
				//todo: fix ui exception
			}
		}
		
		public DataSet ActorSource
		{
			get;
			set;
		}	
		
		public void BindActor()
		{
			fActorStore.Clear();
			foreach (DataRow row in ActorSource.Tables["Actor"].Rows)
			{
				fActorStore.AppendValues(row["ID"], row["Name"], row["Email"]);
			}
			tvActorTree.Model = fActorStore;
		}
		
		public DataSet StateSource
		{
			get;
			set;
		}
		
		public void BindState()
		{
			throw new ImplementationException();
		}
		
		#endregion
		
		#region IGuiGantt Implementation
		
		public DataSet GanttSource
		{
			get;
			set;
		}
		
		public void BindGantt()
		{
		}
		
		#endregion
	}
}