//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.01.2006 at 20:51

using System;
using TaskManagerInterface;
using Arise.Logic;

namespace GanttTracker.TaskManager.ManagerException
{

	public class NotRequiredException : Exception, IManagerException
	{
		public NotRequiredException() : base ("Operation not required")
		{			
		}
		
		public NotRequiredException(string message) : base (message)
		{			
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