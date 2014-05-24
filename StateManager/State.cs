//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.01.2006 at 20:13

using System;
using System.Collections;

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;
using GanttMonoTracker;

namespace GanttTracker.StateManager
{
	public class State : IState, IManagerEntity
	{
		private int fMappingID;

		public State()
		{
			IsNew = true;
			Initialize(null);
		}


		public State(ITaskManager parent)
		{
			IsNew = true;
			Initialize(parent);
		}


		public State(ITaskManager parent, int Id)
		{
			IsNew = false;
			Initialize(parent);
			this.Id = Id;
		}


		private void Initialize(ITaskManager parent)
		{
			Parent = parent;
			Connections = new Hashtable();
		}
		
		#region IStateView Implementation
		
		public string Name { get;set; }


		public byte ColorRed {	get;set; }


		public byte ColorGreen { get;set; }


		public byte ColorBlue {	get;set; }
		

		public int MappingID
		{
			get
			{
				if (!IsMapped)
					throw new ManagementException(ExceptionType.NotAllowed, "State not mapped");
				return fMappingID;
			}
			set
			{
				IsMapped = true;
				fMappingID = value;
			}
		}

		
		public bool IsMapped { get;set; }

		
		public Hashtable Connections { get;set; }

		
		public void Connect(IManagerEntity stateEntry, string connectionName)
		{
			State state = (State)stateEntry;
			if (!Connections.ContainsKey(state.Id))
			{
				Connections.Add(state.Id,connectionName);
			}
			else
				throw new ManagementException(ExceptionType.NotRequired , "Connection to state with ID "+stateEntry.Id+" already added");
		}

		
		public void Disconnect(IManagerEntity stateEntry)
		{
			if (Connections.ContainsKey(stateEntry.Id))
			{
				Connections.Remove(stateEntry.Id);
			}	
			else
				throw new KeyNotFoundException<int>(stateEntry.Id);
		}

		
		public bool IsConnected(IManagerEntity stateEntry)
		{
			return Connections.ContainsKey(stateEntry.Id);
		}

		
		public void ClearConnections()
		{
			Connections.Clear();
		}
		
		#endregion
		
		#region IManagerEntity Implementation
		
		public int Id {	get;set; }


		public bool IsNew {	get;set; }


		public bool isUpdated
		{
			get
			{
				return Parent.IsUpdatedTaskState(this);
			}
			
			set
			{
				throw new ManagementException(ExceptionType.NotAllowed, "Change state for managed entity not allowed");
			}
		}


		public ITaskManager Parent { get;set; }


		public void BindData()
		{
			Parent.BindTaskState(this);
		}

				
		public void Save()
		{
			IsNew = false;
			Parent.UpdateTaskState(this);
		}


		public void Delete()
		{
			Parent.DeleteTaskState(Id);
		}
		
		#endregion
	}
}
