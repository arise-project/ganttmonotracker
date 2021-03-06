﻿//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.11.2005 at 23:03
using TaskManagerInterface;

namespace GanttTracker.TaskManager
{
    public class Actor : IManagerEntity
	{
		public Actor(ITaskManager parent)
		{
			Parent = parent;
		}

		public Actor(ITaskManager parent, int id)
		{
			Id = id;
			Parent = parent;
		}
		
		#region Data
		
		public string Name { get;set; }

		public string Email { get; set; }
		
		#endregion
	
		#region IManagerEntity Implementation
	
		public int Id {	get;set; }

		public bool IsNew
		{
			get
			{
				return (Id == 0);
			}
		}

		public bool isUpdated
		{
			get
			{
				return Parent.IsUpdatedActor(this);
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
			Parent.DeleteActor(this.Id);
		}
		
		#endregion
	}
}