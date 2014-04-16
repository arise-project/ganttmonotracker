//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 29.01.2006 at 15:04

using System.Data;

namespace TaskManagerInterface
{	
	public interface IGuiConnection : IConnectionView
	{
		DataSet TaskStateSource { get;set; }
		
		void BindStateIn();

		void BindStateOut();
	}	
}
