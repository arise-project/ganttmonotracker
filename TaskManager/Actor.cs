//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
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
			Parent = parent;
		}
		
		public Actor(ITaskManager parent, int id)
		{
			ID = id;
			Parent = parent;
		}
		
		#region Data
		
		public string Name { get;set; }
		
		public string Email { get; set; }	
		
		#endregion
	
		#region IManagerEntity Implementation
	
		public int ID {	get;set; }	
		
		public bool isNew
		{
			get
			{
				return (ID == 0);
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
				return Parent.isUpdatedActor(this);
			}
			set
			{
				throw new InvalidOperationException("Can not set isUpdated for managed entity");
			}
		}
			
		public ITaskManager Parent { get;set; }
		
		public void BindData()
		{
			Parent.BindActor(this);			
		}
			
		public void Save()
		{
			Parent.UpdateActor(this);
		}
		
		public void Delete()
		{
			Parent.DeleteActor(this.ID);
		}
		
		#endregion		
	}
}