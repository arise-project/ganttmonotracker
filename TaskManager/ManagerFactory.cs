//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.11.2005 at 21:49

using TaskManagerInterface;
using System.IO;

namespace GanttTracker.TaskManager
{
	public class ManagerFactory : IManagerFactory
	{
		public ITaskManager CreateEmptyManager()
		{
			return new EmptyTaskManager();
		}


		public ITaskManager CreateNewManager(string filename)
		{
			/* //remove existing
			 if(File.Exists(filename))
			{
				File.Delete(filename);
			}
			*/

			var taskManager = new EmptyTaskManager(filename);
			taskManager.Save();
			return new XmlTaskManager(filename);
		}


		public ITaskManager CreateManager(string filename)
		{
			return new XmlTaskManager(filename);
		}
	}
}