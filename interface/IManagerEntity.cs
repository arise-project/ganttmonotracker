//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.11.2005 at 23:05

namespace TaskManagerInterface
{
	public interface IManagerEntity
	{
		int ID { get;set; }
		
		bool isNew { get; }
		
		bool isUpdated	{ get; }
		
		ITaskManager Parent	{ get;set;	}
		
		void BindData();

		void Save();

		void Delete();
	}
}