//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 29.11.2005 at 1:12

using TaskManagerInterface;

namespace GanttTracker.TaskManager.TaskStorage
{
	public class CommandFactory : IStorageCommandFactory
	{		
		public CommandFactory()
		{			
		}
		
		public IStorageCommand CreateSelectCommand(IStorageDealer daler)
		{
			return new SelectCommand(daler.Storage, "",null);
		}
				
		public IStorageCommand CreateInsertCommand(IStorageDealer daler)
		{
			return new InsertCommand(daler.Storage, "",null);
		}
		
		public IStorageCommand CreateUpdateCommand(IStorageDealer daler)
		{
			return new UpdateCommand(daler.Storage, "",null,null);
		}
		
		public IStorageCommand CreateDeleteCommand(IStorageDealer daler)
		{
			return new DeleteCommand(daler.Storage, "",null);
		}
	}
}