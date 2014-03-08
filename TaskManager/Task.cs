// created on 18.11.2005 at 23:03

using TaskManagerInterface;

using System;

using GanttTracker.TaskManager.ManagerException;

namespace GanttTracker.TaskManager
{
	public class Task : IManagerEntity, ITaskView
	{
		private Task()
		{
		}
		
		public Task(ITaskManager parent)
		{
			fParent = parent;
		}
		
		public Task(ITaskManager parent, int id)
		{
			fID = id;
			fParent = parent;
			fParent.BindTask(this);
		}
		
		#region Data
		
		private string fDescription;
		public string Description
		{
			get
			{
				return fDescription;
			}
			
			set
			{
				fDescription = value;
			}			
		}
		
		private bool fActorPresent;
		public bool ActorPresent
		{
			get
			{
				return fActorPresent;
			}
			set
			{
				fActorPresent = value;
			}
		}
		
		private int fActorID;
		public int ActorID
		{
			get
			{
				if (!fActorPresent)
					throw new NotAllowedException("Actor not present");
					
				return fActorID;
			}
			set
			{
				fActorPresent = true;
				fActorID = value;
			}
		}
		
		private bool fStatePresent;
		public bool StatePresent
		{
			get
			{
				return fStatePresent;
			}
			set
			{
				fStatePresent = value;
			}
		}	
		
		private int fStateID;
		public int StateID
		{
			get
			{
				if (!fStatePresent)
					throw new NotAllowedException("State not present");
				return fStateID;
			}
			
			set
			{
				fStatePresent = true;
				fStateID = value;
			}
		}
		
		private DateTime fStartTime;
		public DateTime StartTime
		{
			get
			{
				return fStartTime;
			}
			
			set
			{
				fStartTime = value;
			}
		}
		
		private DateTime fEndTime;
		public DateTime EndTime
		{
			get
			{
				return fEndTime;
			}
			
			set
			{
				fEndTime = value;
			}
		}
		
		private TimeSpan fEstimatedTime;
		public TimeSpan EstimatedTime
		{
			get
			{
				return fEstimatedTime;
			}
			
			set
			{
				fEstimatedTime = value;
			}
		}
		
		#endregion
	
	
		#region IManagerEntity Implementation
		
		private int fID;
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
		
		public bool isNew
		{
			get
			{
				return (fID == 0);
			}
			
			set
			{
				throw new NotAllowedException("Can not set isNew for managed entity");
			}
		}
		
		public bool isUpdated
		{
			get
			{
				return fParent.isUpdatedTask(this);
			}
			set
			{
				throw new NotAllowedException("Can not set isUpdated for managed entity");
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
				throw new NotAllowedException("Can not set Parent for managed entity");
			}
		}
		
		public void BindData()
		{
			fParent.BindTask(this);
		}
			
		public void Save()
		{
			fParent.UpdateTask(this);
		}
		
		public void Delete()
		{
			fParent.DeleteTask(this.ID);
		}
			
		#endregion
		
	}
}