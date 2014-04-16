//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 28.01.2006 at 1:41

namespace TaskManagerInterface
{
	public interface IConnectionView
	{
		string Name { get;set;	}
		
		int MappingID {	get;set; }
		
		int StateID	{ get;set; }									
	}
}
