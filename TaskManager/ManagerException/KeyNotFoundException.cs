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

	public class KeyNotFoundException : Exception, IManagerException
	{
		public object Key { get;set; }
		
		public KeyNotFoundException(object key) : base (key.ToString()+" key not found")
		{
			Key = key;
		}
		
		public KeyNotFoundException(string message,object key) : base (message)
		{			
			Key = key;
		}		

		public int ID
		{
			get
			{
				return IdentityManager.Instance.CreateTypeIdentity(this.GetType());
			}
		}
	}
}