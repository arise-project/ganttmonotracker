// created on 29.01.2006 at 15:04

using System.Data;

namespace TaskManagerInterface
{	
	public interface IGuiConnection : IConnectionView
	{
		DataSet TaskStateSource
		{
			get;
			set;
		}
		
		void BindStateIn();
		void BindStateOut();
	}	
}
