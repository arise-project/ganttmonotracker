// created on 18.12.2005 at 23:11

using System;
using System.Data;

namespace TaskManagerInterface
{
	public interface ITaskView
	{
		bool ActorPresent
		{
			get;
			set;
		}
		
		int ActorID
		{
			get;
			set;
		}
		
		string Description 
		{
			get;
			set;
		}
		
		DateTime StartTime  
		{
			get;
			set;
		}
		
		DateTime EndTime  
		{
			get;
			set;
		}

		int StateID  
		{
			get;
			set;
		}		 
	}
}