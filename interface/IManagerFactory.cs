//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.11.2005 at 22:47

namespace TaskManagerInterface
{
	public interface IManagerFactory
	{
		ITaskManager CreateEmptyManager();


		ITaskManager CreateNewManager(string filename);


		ITaskManager CreateManager(string filename);
	}
}