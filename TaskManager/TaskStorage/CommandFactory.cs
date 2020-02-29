//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 29.11.2005 at 1:12

using TaskManagerInterface;

namespace GanttTracker.TaskManager.TaskStorage
{
	public class CommandFactory : IRepositoryCruid
	{
		IStorageRepository daler;

		public void SetRepository(IStorageRepository daler)
		{
			this.daler = daler;
		}

		public IStorageCommand GetSelectCommand(string entityName)
		{
			return new SelectCommand(daler.Storage, entityName,null);
		}

		public IStorageCommand GetInsertCommand(string entityName)
		{
			return new InsertCommand(daler.Storage, entityName,null);
		}

		public IStorageCommand GetUpdateCommand(string entityName)
		{
			return new UpdateCommand(daler.Storage, entityName,null,null);
		}

		public IStorageCommand GetDeleteCommand(string entityName)
		{
			return new DeleteCommand(daler.Storage, entityName,null);
		}
	}
}