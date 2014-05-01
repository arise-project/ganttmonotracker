//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 22.01.2006 at 1:50

using System.Data;

namespace TaskManagerInterface
{
	public enum CoreState {EmptyProject, CreateProject, OpenProject};
	public interface IGuiCore
	{
		CoreState State { get;set; }


		ITaskManager TaskManager { get;set; }


		IStorageManager StorageManager { get;set; }


		IStateManager StateManager { get;set; }
	}
}