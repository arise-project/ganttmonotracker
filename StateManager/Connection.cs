// created on 28.01.2006 at 1:46

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;

namespace GanttTracker.StateManager
{
	public class Connection : IConnectionView, IManagerEntity
	{
		public Connection()
		{
			fNew = true;
			Initialize(null);
		}
		
		public Connection(ITaskManager parent)
		{
			fNew = true;
			Initialize(parent);
		}
		
		public Connection(ITaskManager parent, int ID)
		{
			fNew = false;
			fID = ID;
			Initialize(parent);
		}		
		
		private void Initialize(ITaskManager parent)
		{					
			fParent = parent;
		}	
			
		
		#region IConnectionView Implementation
		
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
		
		private int fMappingID;
		public int MappingID
		{
			get
			{				
				return fMappingID;
			}
			set
			{
				fMappingID = value;								 
			}
		}
		
		private int fStateID;
		public int StateID
		{
			get
			{
				return fStateID;
			}
			
			set
			{
				fStateID = value;
			}			
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
				return fParent.isUpdatedTaskStateConnection(this);
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
			fParent.BindTaskStateConnection(this);
		}
				
		public void Save()
		{
			fNew = false;
			fParent.UpdateTaskStateConnection(this);
		}
		
		public void Delete()
		{
			fParent.DeleteTaskStateConnection(ID);
		}
		
		#endregion
	}
}