//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 29.11.2005 at 1:12

using TaskManagerInterface;

namespace GanttTracker.TaskManager.TaskStorage
{
	public class CommandFactory : IDealerCruid
	{		
		IStorageDealer daler;


		public CommandFactory()
		{
		}


		public void SetDealer(IStorageDealer daler)
		{
			this.daler = daler;
		}

		
		public IStorageCommand GetSelectCommand()
		{
			return new SelectCommand(daler.Storage, "",null);
		}

				
		public IStorageCommand GetInsertCommand()
		{
			return new InsertCommand(daler.Storage, "",null);
		}

		
		public IStorageCommand GetUpdateCommand()
		{
			return new UpdateCommand(daler.Storage, "",null,null);
		}

		
		public IStorageCommand GetDeleteCommand()
		{
			return new DeleteCommand(daler.Storage, "",null);
		}
	}
}