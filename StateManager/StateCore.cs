//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
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
			StateManager = this;
			StorageManager = guiCore.StorageManager;
			TaskManager = guiCore.TaskManager;
			
			GuiFactory = new GuiFactory();
			StateFactory = new StateFactory(TaskManager);
			ControledGui =  controledGui;
		}	
		
		#region IGuiCore Implementation
		
		public CoreState State { get;set; }
		
		public ITaskManager TaskManager { get;set; }
		
		public IStorageManager StorageManager {	get;set; }
		
		public IStateManager StateManager {	get;set; }		
		
		#endregion
		
		public Window MainForm { get;set; }
		
		public IGuiState ControledGui {	get;set; }
		
		public GuiFactory GuiFactory { get;set; }
		
		public StateFactory StateFactory { get;set; }
		
		public void CreateTaskState()
		{			
			ViewSingleStateDialog stateView = GuiFactory.CreateSingleStateView(MainForm);
			if (Gtk.ResponseType.Ok == (Gtk.ResponseType)stateView.ShowDialog())
			{
				State newState = (State)StateFactory.CreateTaskState();
				if(newState == null) return;
				newState.Name = stateView.Name;
					
				newState.ColorRed = stateView.ColorRed;
				newState.ColorGreen = stateView.ColorGreen;
				newState.ColorBlue = stateView.ColorBlue;
							
				newState.Save();
				ControledGui.StateSource =	TaskManager.TaskStateSource;
				ControledGui.BindStates();
			}
		}
		
		public void EditTaskState(int stateID)
		{
			State state = (State)TaskManager.GetTaskState(stateID); 
			if(state == null) return;
			ViewSingleStateDialog stateView = GuiFactory.CreateSingleStateView(MainForm, state);						
			if (Gtk.ResponseType.Ok == (Gtk.ResponseType)stateView.ShowDialog())
			{
				state.Name = stateView.Name;
					
				state.ColorRed = stateView.ColorRed;
				state.ColorGreen = stateView.ColorGreen;
				state.ColorBlue = stateView.ColorBlue;
					
				state.Save();
				ControledGui.StateSource =	TaskManager.TaskStateSource;
				ControledGui.BindStates();
			}
		}
		
		public void DeleteTaskState(int stateID)
		{			
			TaskManager.DeleteTaskState(stateID);
			ControledGui.StateSource =	TaskManager.TaskStateSource;
			ControledGui.BindStates();
		}
		
		public void CreateTaskStateConnection(int stateID)
		{
			State state = (State)TaskManager.GetTaskState(stateID); 
			if(state == null) return;
			ViewConnectionDialog connectionView = GuiFactory.CreateConnectionView(MainForm,TaskManager.TaskStateSource);						
			if (Gtk.ResponseType.Ok == (Gtk.ResponseType)connectionView.ShowDialog())
			{
				State connectedState = (State)TaskManager.GetTaskState(connectionView.StateID);
				if(connectedState == null) return;
				Connection connection = (Connection)TaskManager.CreateTaskStateConnection(state,connectedState);
				connection.Name = connectionView.Name;
				connection.Save();
				ControledGui.BindConnections(state);
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
