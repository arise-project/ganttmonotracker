//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.12.2005 at 23:11

using System;
using System.Data;

namespace TaskManagerInterface
{
	public interface ITaskView
	{
		bool ActorPresent { get;set; }

		
		int ActorID	{ get;set; }

		
		string Description { get;set; }

		
		DateTime StartTime { get;set; }

		
		DateTime EndTime { get;set; }


		int StateID { get;set; }
	}
}