//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.11.2005 at 23:29

namespace TaskManagerInterface
{
	public interface IStorageCommandFactory
	{
		IStorageCommand CreateSelectCommand(IStorageDealer daler);

		IStorageCommand CreateInsertCommand(IStorageDealer daler);

		IStorageCommand CreateUpdateCommand(IStorageDealer daler);

		IStorageCommand CreateDeleteCommand(IStorageDealer daler);				
	}	
}

