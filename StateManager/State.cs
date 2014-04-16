//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.01.2006 at 20:13

using System;
using System.Collections;

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;

namespace GanttTracker.StateManager
{
	public class State : IStateView, IManagerEntity
	{
		public State()
		{
			isNew = true;
			Initialize(null);
		}
		
		public State(ITaskManager parent)
		{
			isNew = true;
			Initialize(parent);
		}
		
		public State(ITaskManager parent, int ID)
		{
			isNew = false;
			Initialize(parent);
			ID = ID;
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
		
		private int fMappingID;
		public int MappingID
		{
			get
			{
				if (!IsMapped)
					throw new NotAllowedException("State not mapped");
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
			if (!Connections.ContainsKey(state.ID))
			{
				Connections.Add(state.ID,connectionName);				
			}	
			else
				throw new NotRequiredException("Connection to state with ID "+stateEntry.ID+" already added");
		}
		
		public void Disconnect(IManagerEntity stateEntry)
		{
			if (Connections.ContainsKey(stateEntry.ID))
			{
				Connections.Remove(stateEntry.ID);
			}	
			else
				throw new KeyNotFoundException(stateEntry.ID);
		}
		
		public bool IsConnected(IManagerEntity stateEntry)
		{
			return Connections.ContainsKey(stateEntry.ID);
		}
		
		public void ClearConnections()
		{
			Connections.Clear();
		}
		
		#endregion
		
		#region IManagerEntity Implementation
		
		public int ID {	get;set; }
		
		public bool isNew {	get;set; }
		
		public bool isUpdated
		{
			get
			{
				return Parent.isUpdatedTaskState(this);
			}
			
			set
			{
				throw new NotAllowedException("Change state for managed entity not allowed");
			}
		}
		
		public ITaskManager Parent { get;set; }
		
		public void BindData()
		{
			Parent.BindTaskState(this);
		}
				
		public void Save()
		{
			isNew = false;
			Parent.UpdateTaskState(this);
		}
		
		public void Delete()
		{
			Parent.DeleteTaskState(ID);
		}
		
		#endregion
	}
}
