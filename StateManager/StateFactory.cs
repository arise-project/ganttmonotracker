// created on 21.01.2006 at 20:10

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;

namespace GanttTracker.StateManager
{
	public class StateFactory
	{
		public StateFactory()
		{			
		}
		
		public StateFactory(ITaskManager parent)
		{
			fParent = parent;
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
		
		public IManagerEntity CreateTaskState()
		{
			if (fParent == null)
				throw new ValidationException("Parent no set to instance for factory"); 
			return (State)fParent.CreateTaskState();			 
		}
		
//		public IManagerEntity CreateTaskState(string connectionName, IManagerEntity connectedStateEntity)
//		{
//			if (fParent == null)
//				throw new ValidationException("Parent no set to instance for factory");
//			State state = (State)fParent.CreateTaskState();
//			state.Connect(stateEntity,name);
//			
//			return state; 			 
//		}
//		
//		public IManagerEntity CreateTaskState(string name, IManagerEntity [] stateEntities)
//		{
//			if (fParent == null)
//				throw new ValidationException("Parent no set to instance for factory");
//			State state = (State)fParent.CreateTaskState();
//			foreach(IManagerEntity stateEntity in stateEntities)
//			{			
//				state.Connect(stateEntity,name);
//			}
//			return state; 			 
//		} 
//		
		 
	}  
}