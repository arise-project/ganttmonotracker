//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 03.12.2005 at 0:19

using System;
using TaskManagerInterface;
using Arise.Logic;
using GanttMonoTracker;

namespace GanttTracker.TaskManager.ManagerException
{
	public class ManagementException : Exception, IManagerException
	{
		public ManagementException(ExceptionType type) : base ("Operation not allowed") 
		{
			Type = type;
		}


		public ManagementException(ExceptionType type, string message) : base (message) 
		{
			Type = type;
		}


		public int Id
		{
			get
			{
				return IdentityManager.Instance.CreateTypeIdentity(this.GetType());
			}
		}


		public ExceptionType Type { get; private set; }
	}
}