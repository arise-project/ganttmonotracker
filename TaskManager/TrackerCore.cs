//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 10.11.2005 at 1:25

using System;
using System.IO;
using System.Data;
using Gtk;

using GanttTracker.TaskManager;
using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;
using GanttMonoTracker;
using GanttMonoTracker.GuiPresentation;
using GanttMonoTracker.ExceptionPresentation;
using GanttMonoTracker.DrawingPresentation;
using AboutDialog = GanttMonoTracker.GuiPresentation.AboutDialog;

namespace GanttTracker
{
	public class TrackerCore : IGuiCore 
	{
		private Window window;


		private static TrackerCore fInstance;


		private TrackerCore()
		{
		}
		


		public static TrackerCore Instance
		{
			get
			{
				fInstance = fInstance == null ? new TrackerCore() : fInstance; 
				return fInstance;
			}
		}

			
		public IGuiTracker Tracker { get;set; }


		public string Recent 
		{
			 get
			{
				return File.ReadAllText("recent.txt".GetPath ()); 
			}
		 }
		
		#region IGuiCore Implementation
		
		public CoreState State { get;set; }


		public ITaskManager TaskManager	{ get;set; }


		public IStorageManager StorageManager { get;set; }


		public IStateManager StateManager {	get;set; }
		
		#endregion

		public string ProjectFileName {	get;set; }


		public void BindProject()
		{
			var mgr = new ManagerFactory();
			if (State != CoreState.EmptyProject && ProjectFileName == null)
				throw new ManagementException(ExceptionType.NotAllowed, "Set filename for create project");
			switch(State)
			{
				case CoreState.EmptyProject :
				TaskManager = mgr.CreateEmptyManager();
				break;
				case CoreState.CreateProject :
				TaskManager = mgr.CreateNewManager(ProjectFileName);
				break;
				case CoreState.OpenProject :
				mgr.CreateManager(ProjectFileName);
				break;
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
			StorageManager.Save();
		}

		
		public void CreateActor()
		{
			IGuiActorView actorView = GuiFactory.CreateActorView(window);
			if (actorView.ShowDialog() == (int)Gtk.ResponseType.Ok)
			{
				Actor newActor = (Actor)TaskManager.CreateActor();
				if(newActor == null) return;
				newActor.Name = actorView.ActorName;
				newActor.Email = actorView.ActorEmail;
				newActor.Save();
				Tracker.ActorSource = TaskManager.ActorSource;
				Tracker.BindActor();
			}
		}

		
		public void EditActor(int actorID)
		{	
			Actor actor = (Actor)TaskManager.GetActor(actorID);
			if(actor == null) return;
			IGuiActorView actorView = GuiFactory.CreateActorView(window,TaskManager, actor);
			if (actorView.ShowDialog() == (int)Gtk.ResponseType.Ok)
			{
				actor.Name = actorView.ActorName;
				actor.Email = actorView.ActorEmail;
				actor.Save();
				Tracker.ActorSource = TaskManager.ActorSource;
				Tracker.BindActor();
			}
		}

		
		public void DeleteActor(int actorID)
		{
			Actor actor = (Actor)TaskManager.GetActor(actorID);
			if(actor == null) return;
			actor.Delete();
			Tracker.ActorSource = TaskManager.ActorSource;
			Tracker.BindActor();
		}

		
		public void CreateTask()
		{
			IGuiTask taskView = null;
				try
				{
				taskView = GuiFactory.CreateTaskView(window,(IGuiCore)this);
					if(taskView == null) return;
				}
			catch(ManagementException ex)
				{
					IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex,window);
					dialog.Title = "Create Task";
					dialog.ShowDialog();
					return;
				}
				
				Tracker.ActorSource = TaskManager.ActorSource;
				Tracker.BindTask();
				if (taskView.ShowDialog() == (int)Gtk.ResponseType.Ok)
				{
					Task newTask = 	(Task)TaskManager.CreateTask();
					if(newTask == null) return;
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


		public void AssignTask(int taskID)
		{
			ViewTaskAssign assignView = null;
			try
			{
				assignView = GuiFactory.CreateTaskAssign(window,(IGuiCore)this, taskID);
			}
			catch(ManagementException ex)
			{
					IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex,window);
					dialog.Title = "Task Assign";
					dialog.ShowDialog();
					return;
			}
				
			if (assignView.ShowDialog() == (int)Gtk.ResponseType.Ok)
			{
				Task task =	(Task)TaskManager.GetTask(taskID);
				if(task == null) return;
				task.ActorID = assignView.ActorID;
				task.Save();
				Tracker.TaskSource = TaskManager.TaskSource;
				Tracker.BindTask();
			}
		}

		
		public void UpdateTaskState(int taskID)
		{
			IGuiTask taskView = null;
			try
			{
				taskView = GuiFactory.CreateTaskView(window,(IGuiCore)this, taskID);
			}
			catch(ManagementException ex)
			{
				IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex,window);
				dialog.Title = "Change Task State";
				dialog.ShowDialog();
				return;
			}
				
			if (taskView.ShowDialog() == (int)Gtk.ResponseType.Ok)
			{
				Task task = (Task)TaskManager.GetTask(taskID);
				if(task == null) return;
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

		
		public void StateEdit()
		{
			ViewStateDialog stateView = null;
			stateView = GuiFactory.CreateStateView(window, (IGuiCore)this);

			if (Gtk.ResponseType.Ok == (Gtk.ResponseType)stateView.ShowDialog())
			{
				StorageManager.Save();
			}
		}

		
		public GanttDiagramm GanttPresentation { get;set; }


		public AssigmentDiagramm AssigmentPresentation { get;set; }	

				
		public void DrawGantt(Gtk.DrawingArea drawingarea)
		{
			Gdk.Window parent = drawingarea.GdkWindow;
			if (GanttPresentation == null)
				GanttPresentation = new GanttDiagramm();
			GanttPresentation.DateNowVisible = true;
		}

		
		public void ShowAboutDialog()
		{
			AboutDialog aboutView = (AboutDialog)GuiFactory.CreateAboutDialog(window);
			aboutView.ShowDialog();	
		}
	}
}