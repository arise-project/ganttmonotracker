//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.12.2005 at 22:41

using System;
using System.Data;
using Gtk;

using GanttTracker.TaskManager.ManagerException;
using GanttTracker.StateManager;
using GanttTracker.TaskManager;
using TaskManagerInterface;

namespace GanttMonoTracker.GuiPresentation
{
	public class GuiFactory
	{
		public GuiFactory()
		{
		}
		
		public IGuiActorView CreateActorView(Window parent)
		{
			ViewActorDialog actorDialog = new ViewActorDialog(parent);
			actorDialog.Title = "Create Actor";
			return actorDialog; 
		}



		public IGuiActorView CreateActorView(Window parent,ITaskManager taskManager, Actor actor)
		{
			ViewActorDialog actorDialog = new ViewActorDialog(parent);
			actorDialog.ActorName = actor.Name;
			actorDialog.ActorEmail = actor.Email;
			actorDialog.Title = "Edit Actor";
			return actorDialog;
		}



		private void ValidateCore(IGuiCore core)
		{
			if (core.TaskManager.ActorSource.Tables["Actor"].Rows.Count == 0)
			{
				throw new NotAllowedException("Create actors before create tasks");
			}
			if (core.TaskManager.TaskStateSource.Tables["TaskState"].Rows.Count == 0)
			{
				throw new NotAllowedException("Create states before create tasks");
			}
		}
		
		public IGuiTaskView CreateTaskView(Window parent, IGuiCore core)
		{
			ValidateCore(core);
			
			ViewTaskDialog taskDialog = new ViewTaskDialog(parent, true);
			taskDialog.Title ="Create Task";
			taskDialog.ActorSource = core.TaskManager.ActorSource;
			taskDialog.StateSource = core.TaskManager.GetInitialTaskStateSource();
			if(taskDialog.StateSource == null) return taskDialog;
			taskDialog.BindActor();
			taskDialog.BindState();
			
			return taskDialog; 
		}
		
		public IGuiTaskView CreateTaskView(Window parent, IGuiCore core, int taskID)
		{
			ValidateCore(core);
			
			Task task = (Task)core.TaskManager.GetTask(taskID);
			ViewTaskDialog taskDialog = new ViewTaskDialog(parent, false);
			if(task == null) return taskDialog;
			if (task.StatePresent)
			{
				State state = (State)core.TaskManager.GetTaskState(task.StateID);
				if(state == null) return taskDialog;
				taskDialog.StateSource = core.TaskManager.GetTaskStateSource(state);
				if(taskDialog.StateSource == null) return taskDialog;
				taskDialog.BindState();
				taskDialog.StateID = task.StateID;
			}
			else
			{
				taskDialog.StateSource = core.TaskManager.GetInitialTaskStateSource();
				if(taskDialog.StateSource == null) return taskDialog;
				taskDialog.BindState();
			}

			taskDialog.Title ="Edit Task";
			taskDialog.ActorSource = core.TaskManager.ActorSource;

			taskDialog.BindActor();
			if (task.ActorPresent)
				taskDialog.ActorID = task.ActorID;
			taskDialog.Description = task.Description;
			taskDialog.EndTime = task.EndTime;
			taskDialog.StartTime = task.StartTime;
			taskDialog.Comment = task.Comment; 
			return taskDialog; 
		}

		
		public ViewTaskAssign CreateTaskAssign(Window parent, IGuiCore core, int taskID)
		{
			ValidateCore(core);
			Task task = (Task)core.TaskManager.GetTask(taskID);
			ViewTaskAssign assignDialog = new ViewTaskAssign(parent);
			if(task == null) return assignDialog;
			assignDialog.ActorSource = core.TaskManager.ActorSource;
			assignDialog.BindActor();
			if (task.ActorPresent)
				assignDialog.ActorID = task.ActorID;
			assignDialog.Title = "Assign Task";
			assignDialog.AssignAction = "Assign task " + task.ID + " to Actor";
			
			return  assignDialog;
		}

		
		public ViewStateDialog CreateStateView (Window parent, IGuiCore guiCore)
		{
			ViewStateDialog stateView = new ViewStateDialog(parent,guiCore);
			stateView.Title = "Edit States";
			return stateView;
		}
		
		public ViewSingleStateDialog CreateSingleStateView (Window parent)
		{
			ViewSingleStateDialog stateView = new ViewSingleStateDialog(parent);
			stateView.Title = "New State";
			return stateView;
		}
		
		public ViewSingleStateDialog CreateSingleStateView (Window parent, State state)
		{
			ViewSingleStateDialog stateView = new ViewSingleStateDialog(parent);
			stateView.Title = "Edit State";
			
			stateView.Name = state.Name;
			
			stateView.ColorRed = state.ColorRed;
			stateView.ColorGreen = state.ColorGreen;
			stateView.ColorBlue = state.ColorBlue;
			stateView.BindStateColor();
			
			if (state.IsMapped)
				stateView.MappingID = state.MappingID;
			else
				stateView.IsMapped = false;
			return stateView;
		}
		
		public ViewConnectionDialog CreateConnectionView (Window parent, DataSet taskStateSource)
		{
			ViewConnectionDialog connectionView = new ViewConnectionDialog(parent,taskStateSource);
			connectionView.Title = "New Connection";
			connectionView.BindStateIn();
			connectionView.BindStateOut();
			return connectionView;
		}
		
		public AboutDialog CreateAboutDialog(Window parent)
		{
			AboutDialog aboutDialog = new AboutDialog(parent);
			return aboutDialog;
		}
	}
}