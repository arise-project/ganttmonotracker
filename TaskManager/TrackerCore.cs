// created on 10.11.2005 at 1:25

using System;
using System.IO;
using System.Data;

using Gtk;

using GanttTracker.TaskManager;
using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;
using GanttMonoTracker.GuiPresentation;
using GanttMonoTracker.ExceptionPresentation;
using GanttMonoTracker.DrawingPresentation;

 
using AboutDialog = GanttMonoTracker.GuiPresentation.AboutDialog;

namespace GanttTracker
{
	public class TrackerCore : IGuiCore 
	{
		private Window window;

		private TrackerCore()
		{
		}
		
		private static TrackerCore fInstance;
		public static TrackerCore Instance
		{
			get
			{
				if (fInstance == null)
				{					
					fInstance = new TrackerCore(); 
				}				
				return fInstance;
			}			
		}
			
		public IGuiTracker Tracker
		{
			get;
			set;
		}
		
		public GuiFactory GuiSource
		{
			get;
			set;		
		}		
		
		#region IGuiCore Implementation
		
		public CoreState State
		{
			get;
			set;
		}
		
		public ITaskManager TaskManager
		{
			get;
			set;
		}
		
		public IStorageManager StorageManager
		{
			get;
			set;
		}
		
		public IStateManager StateManager
		{
			get;
			set;
		}
		
		#endregion
		public string ProjectFileName
		{
			get;
			set;
		}
		
		public void BindProject()
		{
			switch(State)
			{
				case CoreState.EmptyProject :
					TaskManager = new EmptyTaskManager();
					break;
				case CoreState.CreateProject :
					if (ProjectFileName == null)
						throw new NotAllowedException("Set filename for create project");
					if (File.Exists(ProjectFileName))
					{
						File.Delete(ProjectFileName);
					}
					
					TaskManager = new EmptyTaskManager(ProjectFileName);
					TaskManager.Save();
					
					TaskManager = new XmlTaskManager(ProjectFileName);
					
					break;
				case CoreState.OpenProject :
					if (ProjectFileName == null)
						throw new NotAllowedException("Set filename for create project");
					TaskManager = new XmlTaskManager(ProjectFileName);
					break;
				default :
					throw new ImplementationException("Bind with state " + State.ToString() + " not implemented");
			}
			StorageManager = TaskManager;
			Tracker.TaskSource = TaskManager.TaskSource;
			Tracker.ActorSource = TaskManager.ActorSource;
			Tracker.StateSource = TaskManager.TaskStateSource;
			Tracker.BindTask();
			Tracker.BindActor();
			window = Tracker as Window;
		}
		
		public void SaveProject()
		{
			try
			{
				StorageManager.Save();
			}
			catch(ImplementationException ex)
			{
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateWarningDialog(ex,window);
				dialog.Title = "Not Implemented";
				dialog.ShowDialog();
			}
		}
		
		public void CreateActor()
		{
			try
			{			
				IGuiActorView actorView = GuiSource.CreateActorView(window);
				if (actorView.ShowDialog() == (int)Gtk.ResponseType.Ok)
				{
					Actor newActor = (Actor)TaskManager.CreateActor();
					newActor.Name = actorView.ActorName;
					newActor.Email = actorView.ActorEmail;
					newActor.Save();	
					Tracker.ActorSource = TaskManager.ActorSource;
					Tracker.BindActor();
				}				
			}
			catch(ImplementationException ex)
			{
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateWarningDialog(ex,window);
				dialog.Title = "Not Implemented";
				dialog.ShowDialog();
			}						
		}
		
		public void EditActor(int actorID)
		{	
			try
			{
				Actor actor = (Actor)TaskManager.GetActor(actorID);
				IGuiActorView actorView = GuiSource.CreateActorView(window,TaskManager, actor);
				if (actorView.ShowDialog() == (int)Gtk.ResponseType.Ok)
				{
					actor.Name = actorView.ActorName;
					actor.Email = actorView.ActorEmail;
					actor.Save();	
					Tracker.ActorSource = TaskManager.ActorSource;
					Tracker.BindActor();
				}
			}
			catch(ImplementationException ex)
			{
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateWarningDialog(ex,window);
				dialog.Title = "Not Implemented";
				dialog.ShowDialog();
			}
		}
		
		public void DeleteActor(int actorID)
		{
			try
			{
				Actor actor = (Actor)TaskManager.GetActor(actorID);
				actor.Delete();
				Tracker.ActorSource = TaskManager.ActorSource;
				Tracker.BindActor();
			}
			catch(ImplementationException ex)
			{
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateWarningDialog(ex,window);
				dialog.Title = "Not Implemented";
				dialog.ShowDialog();
			}
		}
		
		public void CreateTask()
		{
			try
			{		
				IGuiTaskView taskView = null;
				try
				{
					taskView = GuiSource.CreateTaskView(window,(IGuiCore)this);
				}
				catch(NotAllowedException ex)
				{
					IGuiMessageDialog dialog = MessageFactory.Instance.CreateMessageDialog(ex.Message,window);
					dialog.Title = "Create Task";
					dialog.ShowDialog();
					return;
				}
				
				Tracker.ActorSource = TaskManager.ActorSource;
				Tracker.BindTask();
				if (taskView.ShowDialog() == (int)Gtk.ResponseType.Ok)
				{
					Task newTask = 	(Task)TaskManager.CreateTask();
					if (taskView.ActorPresent)
						newTask.ActorID = taskView.ActorID;
					else
						newTask.ActorPresent = false;
					newTask.Description = taskView.Description;
					newTask.EndTime = taskView.EndTime;
					newTask.StartTime = taskView.StartTime;
					newTask.StateID = taskView.StateID;

					newTask.Save();
					
					Tracker.TaskSource = TaskManager.TaskSource;
					Tracker.BindTask();
				}
			}
			catch(ImplementationException ex)
			{
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateWarningDialog(ex,window);
				dialog.Title = "Not Implemented";
				dialog.ShowDialog();
			}
		}
		public void AssignTask(int taskID)
		{
			try
			{
				ViewTaskAssign assignView = null;
				try
				{			
					assignView = GuiSource.CreateTaskAssign(window,(IGuiCore)this, taskID);
				}
				catch(NotAllowedException ex)
				{
					IGuiMessageDialog dialog = MessageFactory.Instance.CreateMessageDialog(ex.Message,window);
					dialog.Title = "Task Assign";
					dialog.ShowDialog();
					return;
				}
				
				if (assignView.ShowDialog() == (int)Gtk.ResponseType.Ok)
				{
					Task task =	(Task)TaskManager.GetTask(taskID);
					task.ActorID = assignView.ActorID;
					task.Save();
					Tracker.TaskSource = TaskManager.TaskSource;
					Tracker.BindTask();
				}			
			}
			catch(ImplementationException ex)
			{
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateWarningDialog(ex,window);
				dialog.Title = "Not Implemented";
				dialog.ShowDialog();
			}
		}
		
		public void UpdateTaskState(int taskID)
		{
			try
			{
				IGuiTaskView taskView = null;
				try
				{
					taskView = GuiSource.CreateTaskView(window,(IGuiCore)this, taskID);
				}
				catch(NotAllowedException ex)
				{
					IGuiMessageDialog dialog = MessageFactory.Instance.CreateMessageDialog(ex.Message,window);
					dialog.Title = "Change Task State";
					dialog.ShowDialog();
					return;
				}
				
				if (taskView.ShowDialog() == (int)Gtk.ResponseType.Ok)
				{
					Task task = (Task)TaskManager.GetTask(taskID);
					if (taskView.ActorPresent)
						task.ActorID = taskView.ActorID;
					task.Description = taskView.Description;
					task.EndTime = taskView.EndTime;
					task.StartTime = taskView.StartTime;
					task.StateID = taskView.StateID;
					task.Save();
					Tracker.TaskSource = TaskManager.TaskSource;
					Tracker.BindTask();
				}
			}
			catch(ImplementationException ex)
			{
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateWarningDialog(ex,window);
				dialog.Title = "Not Implemented";
				dialog.ShowDialog();
			}
		}
		
		public void StateEdit()
		{
			ViewStateDialog stateView = null;
			try
			{
				stateView = GuiSource.CreateStateView(window, (IGuiCore)this);
			}
			catch(ImplementationException ex)
			{
				IGuiMessageDialog dialog = MessageFactory.Instance.CreateWarningDialog(ex,window);
				dialog.Title = "Not Implemented";
				dialog.ShowDialog();
				return;
			}
			
			if (Gtk.ResponseType.Ok == (Gtk.ResponseType)stateView.ShowDialog())
			{
				try
				{
					StorageManager.Save();
				}
				catch(ImplementationException ex)
				{
					IGuiMessageDialog dialog = MessageFactory.Instance.CreateWarningDialog(ex,window);
					dialog.Title = "Not Implemented";
					dialog.ShowDialog();
				}
			}	
		}	
		
		public GanttDiagramm GanttPresentation
		{
			get;
			set;
		}
		
		public AssigmentDiagramm AssigmentPresentation
		{
			get;
			set;
		}	
				
		public void DrawGantt(Gtk.DrawingArea drawingarea)
		{
			Gdk.Window parent = drawingarea.GdkWindow;
			if (GanttPresentation == null)
				GanttPresentation = new GanttDiagramm();
			GanttPresentation.PangoContext = drawingarea.PangoContext;
			try
			{
				GanttPresentation.GanttSource = TaskManager.GanttSource;
				GanttPresentation.DateNowVisible = true;
				GanttPresentation.CreateDiagramm(parent);
			}
			catch(ImplementationException)
			{
				GanttPresentation.Clear();
			}
		}
		
		public void DrawAssigment(Gtk.DrawingArea drawingarea)
		{
			Gdk.Window parent = drawingarea.GdkWindow;
			if (AssigmentPresentation == null)
				AssigmentPresentation = new AssigmentDiagramm();
			AssigmentPresentation.PangoContext = drawingarea.PangoContext;
			try
			{
				AssigmentPresentation.AssigmentSource = TaskManager.AssigmentSource;
				AssigmentPresentation.CreateDiagramm(parent);
			}
			catch(ImplementationException)
			{
				AssigmentPresentation.Clear();
			}
		}
		
		public void ShowAboutDialog()
		{
			AboutDialog aboutView = (AboutDialog)GuiSource.CreateAboutDialog(window);
			aboutView.ShowDialog();	
		}
	}
}