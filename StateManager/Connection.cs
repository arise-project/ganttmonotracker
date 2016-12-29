//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 28.01.2006 at 1:46

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;
using GanttMonoTracker;

namespace GanttTracker.StateManager
{
	public class Connection : IGuiConnection, IManagerEntity
	{
		public Connection()
		{
			IsNew = true;
			Initialize(null);
		}

		public Connection(ITaskManager parent)
		{
			IsNew = true;
			Initialize(parent);
		}

		public Connection(ITaskManager parent, int Id)
		{
			IsNew = false;
			this.Id = Id;
			Initialize(parent);
		}

		void Initialize(ITaskManager parent)
		{
			Parent = parent;
		}
		
		#region IConnectionView Implementation
		
		public string Name { get; set; }

		public int MappingID { get;set; }

		public int StateID { get;set; }
		
		#endregion
		
		#region IManagerEntity Implementation
		
		public int Id {	get;set; }

		public bool IsNew {	get;set; }

		public bool isUpdated
		{
			get
			{
				return Parent.isUpdatedTaskStateConnection(this);
			}
			
			set
			{
				throw new ManagementException(ExceptionType.NotAllowed, "Change state for managed entity not allowed");
			}
		}

		public ITaskManager Parent { get;set; }

		public void BindData()
		{
			Parent.BindTaskStateConnection(this);
		}

		public void Save()
		{
			IsNew = false;
			Parent.UpdateTaskStateConnection(this);
		}

		public void Delete()
		{
			Parent.DeleteTaskStateConnection(Id);
		}

		#endregion

		public void BindStateIn ()
		{
		}

		public void BindStateOut ()
		{
		}

		public System.Data.DataSet TaskStateSource { get; set; }
	}
}