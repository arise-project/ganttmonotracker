//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.11.2005 at 21:49

using TaskManagerInterface;
using System;
using System.IO;

namespace GanttTracker.TaskManager
{
	public class ManagerFactory : IManagerFactory
	{
		private CoreState state;

		public ManagerFactory(CoreState s)
		{
			state = s;
		}

		public ITaskManager Create(string fileName = null)
		{
			switch (state)
			{
				case CoreState.EmptyProject:
					return CreateEmptyManager();
				case CoreState.CreateProject:
					return CreateNewManager(fileName);
				case CoreState.OpenProject:
					return CreateManager(fileName);
				default:
					throw new NotSupportedException();
			}
		}

		internal ITaskManager CreateEmptyManager()
		{
			return new EmptyTaskManager();
		}

		internal ITaskManager CreateNewManager(string filename)
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

		internal ITaskManager CreateManager(string filename)
		{
			return new XmlTaskManager(filename);
		}
	}
}
