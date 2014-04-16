//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 28.01.2006 at 1:46

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;

namespace GanttTracker.StateManager
{
	public class Connection : IConnectionView, IManagerEntity
	{
		public Connection()
		{
			isNew = true;
			Initialize(null);
		}
		
		public Connection(ITaskManager parent)
		{
			isNew = true;
			Initialize(parent);
		}
		
		public Connection(ITaskManager parent, int ID)
		{
			isNew = false;
			ID = ID;
			Initialize(parent);
		}		
		
		private void Initialize(ITaskManager parent)
		{				
			Parent = parent;
		}	
			
		
		#region IConnectionView Implementation
		
		public string Name { get; set; }
		
		public int MappingID { get;set; }
		
		public int StateID { get;set; }
		
		#endregion				
		
		#region IManagerEntity Implementation
		
		public int ID {	get;set; }
		
		public bool isNew {	get;set; }
		
		public bool isUpdated
		{
			get
			{
				return Parent.isUpdatedTaskStateConnection(this);
			}
			
			set
			{
				throw new NotAllowedException("Change state for managed entity not allowed");
			}
		}
		
		public ITaskManager Parent { get;set; }
		
		public void BindData()
		{
			Parent.BindTaskStateConnection(this);
		}
				
		public void Save()
		{
			isNew = false;
			Parent.UpdateTaskStateConnection(this);
		}
		
		public void Delete()
		{
			Parent.DeleteTaskStateConnection(ID);
		}
		
		#endregion
	}
}