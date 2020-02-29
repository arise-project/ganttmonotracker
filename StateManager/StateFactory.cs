//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.01.2006 at 20:10

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;
using GanttMonoTracker;

namespace GanttTracker.StateManager
{
	public static class StateFactory
	{
		public static ITaskManager Parent { get; set; }

		public static IManagerEntity CreateTaskState()
		{
			if (Parent == null)
				throw new ManagementException(ExceptionType.ValidationFailed, "Parent no set to instance for factory"); 
			return (State)Parent.CreateTaskState();
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
	}
}