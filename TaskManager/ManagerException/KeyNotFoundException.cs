//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 02.12.2005 at 23:18

using System;

using TaskManagerInterface;
using Arise.Logic;

namespace GanttTracker.TaskManager.ManagerException
{
	public class KeyNotFoundException<T> : Exception
	{
		public T Key { get;set; }


		public KeyNotFoundException(T key) : base (key.ToString()+" key not found")
		{
			Key = key;
		}


		public KeyNotFoundException(string message,T key) : base (message)
		{
			Key = key;
		}
	}
}