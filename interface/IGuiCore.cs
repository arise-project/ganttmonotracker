// created on 22.01.2006 at 1:50

using System.Data;

namespace TaskManagerInterface
{
	public enum CoreState {EmptyProject, CreateProject, OpenProject};
	public interface IGuiCore
	{				
		CoreState State
		{
			get;
			set;			
		}
		
		ITaskManager TaskManager
		{
			get;
			set;
		}
		
		IStorageManager StorageManager
		{
			get;
			set;
		}
		
		IStateManager StateManager
		{
			get;
			set;
		}
	}
}