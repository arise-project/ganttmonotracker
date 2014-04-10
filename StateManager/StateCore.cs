// created on 22.01.2006 at 2:18

using System;
using Gtk;

using GanttTracker.TaskManager.ManagerException;
using GanttMonoTracker.ExceptionPresentation;
using GanttMonoTracker.GuiPresentation;
using TaskManagerInterface;

namespace GanttTracker.StateManager
{
	public class StateCore : IGuiCore, IStateManager
	{
		public StateCore(IGuiCore guiCore, IGuiState controledGui)
		{
			Initialize(guiCore, controledGui);
		}
		
		private void Initialize(IGuiCore guiCore, IGuiState controledGui)
		{
			fStateManager = this;
			fStorageManager = guiCore.StorageManager;
			fTaskManager = guiCore.TaskManager;
			
			fGuiFactory = new GuiFactory();
			fStateFactory = new StateFactory(fTaskManager);
			fControledGui =  controledGui;
		}	
		
		#region IGuiCore Implementation
		
		private CoreState fState;
		public CoreState State
		{
			get
			{
				return fState;
			}
			
			set
			{
				fState = value;
			}			
		}
		
		private ITaskManager fTaskManager;
		public ITaskManager TaskManager
		{
			get
			{
				return fTaskManager;
			}
			
			set
			{
				fTaskManager = value;
			}						
		}
		
		private IStorageManager fStorageManager;
		public IStorageManager StorageManager
		{
			get
			{
				return fStorageManager;
			}
			
			set
			{
				fStorageManager = value;
			}						
		}
		
		private IStateManager fStateManager;
		public IStateManager StateManager
		{
			get
			{
				return fStateManager;
			}
			
			set
			{
				fStateManager = value;
			}
		}		
		
		#endregion
		
		private Window fMainForm;
		public Window MainForm
		{
			get
			{
				return fMainForm;
			}
			set
			{
				fMainForm = value;
			}			
		}
		
		private IGuiState fControledGui;
		public IGuiState ControledGui
		{
			get
			{
				return fControledGui;
			}
			
			set
			{
				fControledGui = value;
			}
			
		}
		
		private GuiFactory fGuiFactory;
		public GuiFactory GuiFactory
		{
			get
			{
				return fGuiFactory;
			}
			set
			{
				fGuiFactory = value;
			}
		}
		
		public StateFactory fStateFactory;
		public StateFactory StateFactory
		{
			get
			{
				return fStateFactory;
			}
			
			set
			{
				fStateFactory = value;
			}
		}
		
		public void CreateTaskState()
		{			
			ViewSingleStateDialog stateView = fGuiFactory.CreateSingleStateView(fMainForm);
			if (Gtk.ResponseType.Ok == (Gtk.ResponseType)stateView.ShowDialog())
			{
				State newState = (State)fStateFactory.CreateTaskState();
				if(newState == null) return;
				newState.Name = stateView.Name;
					
				newState.ColorRed = stateView.ColorRed;
				newState.ColorGreen = stateView.ColorGreen;
				newState.ColorBlue = stateView.ColorBlue;
							
				newState.Save();
				fControledGui.StateSource =	TaskManager.TaskStateSource;
				fControledGui.BindStates();
			}
		}
		
		public void EditTaskState(int stateID)
		{
			State state = (State)TaskManager.GetTaskState(stateID); 
			if(state == null) return;
			ViewSingleStateDialog stateView = fGuiFactory.CreateSingleStateView(fMainForm, state);						
			if (Gtk.ResponseType.Ok == (Gtk.ResponseType)stateView.ShowDialog())
			{
				state.Name = stateView.Name;
					
				state.ColorRed = stateView.ColorRed;
				state.ColorGreen = stateView.ColorGreen;
				state.ColorBlue = stateView.ColorBlue;
					
				state.Save();
				fControledGui.StateSource =	TaskManager.TaskStateSource;
				fControledGui.BindStates();
			}
		}
		
		public void DeleteTaskState(int stateID)
		{			
			TaskManager.DeleteTaskState(stateID);
			fControledGui.StateSource =	TaskManager.TaskStateSource;
			fControledGui.BindStates();
		}
		
		public void CreateTaskStateConnection(int stateID)
		{
			State state = (State)TaskManager.GetTaskState(stateID); 
			if(state == null) return;
			ViewConnectionDialog connectionView = fGuiFactory.CreateConnectionView(fMainForm,TaskManager.TaskStateSource);						
			if (Gtk.ResponseType.Ok == (Gtk.ResponseType)connectionView.ShowDialog())
			{
				State connectedState = (State)TaskManager.GetTaskState(connectionView.StateID);
				if(connectedState == null) return;
				Connection connection = (Connection)TaskManager.CreateTaskStateConnection(state,connectedState);
				connection.Name = connectionView.Name;
				connection.Save();
				fControledGui.BindConnections(state);
			}
		}
		
		public void EditTaskStateConnection(int connectionID)
		{
		}
		 
		public void DeleteTaskStateConnection(int connectionID)
		{
		} 
	}
}
