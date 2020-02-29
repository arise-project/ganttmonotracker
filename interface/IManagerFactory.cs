//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.11.2005 at 22:47
namespace TaskManagerInterface
{
    public interface IManagerFactory
    {
		ITaskManager Create(string fileName = null);
    }
}