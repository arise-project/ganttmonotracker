// created on 02.12.2005 at 23:18

using System;
using TaskManagerInterface;
using Arise.Logic;

namespace GanttTracker.TaskManager.ManagerException
{

	public class KeyNotFoundException : Exception, IManagerException
	{
		private object fKey;
		
		public object Key
		{
			get
			{
				return fKey;
			}
			set
			{
				fKey = value;
			}
		}
		
		public KeyNotFoundException(object key) : base (key.ToString()+" key not found")
		{
			fKey = key;
		}
		
		public KeyNotFoundException(string message,object key) : base (message)
		{			
			fKey = key;
		}		

		public int ID
		{
			get
			{
				return IdentityManager.Instance.CreateTypeIdentity(this.GetType());
			}
			set
			{
				
			}
		}
	}
}