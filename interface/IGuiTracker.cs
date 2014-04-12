//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 10.11.2005 at 1:31
using System.Data;

namespace TaskManagerInterface
{
	public interface IGuiTracker
	{
		DataSet TaskSource
		{
			get;
			set;
		}	
		
		DataSet ActorSource
		{
			get;
			set;
		}
		
		DataSet StateSource
		{
			get;
			set;
		}
		
		void BindTask();	
		void BindActor();
		void BindState();
	}	
}