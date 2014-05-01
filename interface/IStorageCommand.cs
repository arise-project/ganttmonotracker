//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.11.2005 at 22:53

namespace TaskManagerInterface
{
	public interface IStorageCommand
	{
		void SetParam(object key, object value);


		object GetParam(object key);


		object Contains(object key);


		object [] GetParamKeys();

		
		void CheckParams();


		object Execute();
	}
}