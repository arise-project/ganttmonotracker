// created on 18.11.2005 at 22:47

namespace TaskManagerInterface
{
	public interface IManagerFactory
	{
		ITaskManager CreateEmptyManager();
		ITaskManager CreateNewManager(IStorageDealer dealer);
		ITaskManager CreateManager(IStorageDealer dealer);				
	}
}