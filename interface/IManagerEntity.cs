// created on 18.11.2005 at 23:05

namespace TaskManagerInterface
{
	public interface IManagerEntity
	{
		int ID
		{
			get;
			set;
		}
		
		bool isNew
		{
			get;
		}
		
		bool isUpdated
		{
			get;
		}
		
		ITaskManager Parent
		{
			get;
			set;
		}
		
		void BindData();
		void Save();
		void Delete();
	}
}