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
			fNew = true;
			Initialize(null);
		}
		
		public State(ITaskManager parent)
		{
			fNew = true;
			Initialize(parent);
		}
		
		public State(ITaskManager parent, int ID)
		{
			fNew = false;
			Initialize(parent);
			fID = ID;
		}
		
		private void Initialize(ITaskManager parent)
		{
			fParent = parent;
			fConnections = new Hashtable();
		}
		
		#region IStateView Implementation
		
		private string fName;
		public string Name
		{
			get
			{
				return fName;
			}
			
			set
			{
				fName = value;
			}
		}
		
		private byte fColorRed;
		public byte ColorRed
		{
			get
			{
				return fColorRed;
			}
			
			set
			{
				fColorRed = value;
			}
		}
		
		private byte fColorGreen;
		public byte ColorGreen
		{
			get
			{
				return fColorGreen;
			}
			
			set
			{
				fColorGreen = value;
			}
		}
		
		private byte fColorBlue;
		public byte ColorBlue
		{
			get
			{
				return fColorBlue;
			}
			
			set
			{
				fColorBlue = value;
			}
		}
		
		private int fMappingID;
		public int MappingID
		{
			get
			{
				if (!fIsMapped)
					throw new NotAllowedException("State not mapped");
				return fMappingID;
			}
			set
			{
				fIsMapped = true;
				fMappingID = value;				 
			}
		}
		
		private bool fIsMapped;
		public bool IsMapped
		{
			get
			{
				return fIsMapped;
			}
			
			set
			{
				fIsMapped = value;
			}
		}
		
		private Hashtable fConnections;
		public Hashtable Connections
		{
			get
			{
				return fConnections;
			}
			
			set
			{
				throw new NotAllowedException("Use Connect, Disconnect methods for change connections");
			}
		}
		
		public void Connect(IManagerEntity stateEntry, string connectionName)
		{		 
			State state = (State)stateEntry;
			if (!fConnections.ContainsKey(state.ID))
			{
				fConnections.Add(state.ID,connectionName);				
			}	
			else
				throw new NotRequiredException("Connection to state with ID "+stateEntry.ID+" already added");
		}
		
		public void Disconnect(IManagerEntity stateEntry)
		{
			if (fConnections.ContainsKey(stateEntry.ID))
			{
				fConnections.Remove(stateEntry.ID);
			}	
			else
				throw new KeyNotFoundException(stateEntry.ID);
		}
		
		public bool IsConnected(IManagerEntity stateEntry)
		{
			return fConnections.ContainsKey(stateEntry.ID);
		}
		
		public void ClearConnections()
		{
			fConnections.Clear();
		}
		
		#endregion
		
		#region IManagerEntity Implementation
		
		public int fID;
		public int ID
		{
			get
			{
				return fID;
			}
			
			set
			{
				throw new NotAllowedException("Can not set ID for managed entity");
			}
		}
		
		private bool fNew;
		public bool isNew
		{
			get
			{
				return fNew;
			}
			
			set
			{
				throw new NotAllowedException("Change state for managed entity not allowed");
			}
		}
		
		public bool isUpdated
		{
			get
			{
				return fParent.isUpdatedTaskState(this);
			}
			
			set
			{
				throw new NotAllowedException("Change state for managed entity not allowed");
			}
		}
		
		private ITaskManager fParent;
		public ITaskManager Parent
		{
			get
			{
				return fParent;
			}
			
			set
			{
				fParent = value;
			}
		}
		
		public void BindData()
		{
			fParent.BindTaskState(this);
		}
				
		public void Save()
		{
			fNew = false;
			fParent.UpdateTaskState(this);
		}
		
		public void Delete()
		{
			fParent.DeleteTaskState(ID);
		}
		
		#endregion
	}
}
