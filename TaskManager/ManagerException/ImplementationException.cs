// created on 27.11.2005 at 21:42
using System;
using TaskManagerInterface;
using Arise.Logic;

namespace GanttTracker.TaskManager.ManagerException
{

	public class ImplementationException : Exception, IManagerException
	{
		public ImplementationException() : base ("Not Implemented functionality")
		{			
		}
		
		public ImplementationException(string message) : base (message)
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