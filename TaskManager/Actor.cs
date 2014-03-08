// created on 18.11.2005 at 23:03
using System;
using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;


namespace GanttTracker.TaskManager
{
	public class Actor : IManagerEntity
	{
		private Actor()
		{
		}		
		
		public Actor(ITaskManager parent)
		{
			fParent = parent;
		}
		
		public Actor(ITaskManager parent, int id)
		{
			fID = id;
			fParent = parent;
		}
		
		#region Data
		
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
		
		private string fEmail;
		public string Email
		{
			get
			{
				return fEmail;
			}
			
			set
			{
				fEmail = value;
			}			
		}	
		
		#endregion
	
		#region IManagerEntity Implementation
	
		private int fID = 0;
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
				throw new InvalidOperationException("Can not set isNew for managed entity");
			}
		}
		
		public bool isUpdated
		{
			get
			{
				return fParent.isUpdatedActor(this);
			}
			set
			{
				throw new InvalidOperationException("Can not set isUpdated for managed entity");
			}
		}
			
		private ITaskManager fParent = null;
		public ITaskManager Parent
		{
			get
			{
				return fParent;			
			}
			
			set
			{
				throw new InvalidOperationException("Can not set Parent for managed entity");
			}
		}
		
		public void BindData()
		{
			fParent.BindActor(this);			
		}
			
		public void Save()
		{
			fParent.UpdateActor(this);
		}
		
		public void Delete()
		{
			fParent.DeleteActor(this.ID);
		}
		
		#endregion		
	}
}