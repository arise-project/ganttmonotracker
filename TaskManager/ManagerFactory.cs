// created on 18.11.2005 at 21:49

using TaskManagerInterface;

namespace GanttTracker.TaskManager
{
	public class ManagerFactory : IManagerFactory
	{
		public ITaskManager CreateEmptyManager()
		{
			return new EmptyTaskManager();
		}
		public ITaskManager CreateNewManager(IStorageDealer dealer)
		{
			return null;
		}
		public ITaskManager CreateManager(IStorageDealer dealer)
		{
			return null;
		}			
	}
}

