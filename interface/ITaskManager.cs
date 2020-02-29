//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.11.2005 at 21:52

namespace TaskManagerInterface
{
	using System;
	using System.Data;
	using System.Threading.Tasks;

	public interface ITaskManager : IStorageManager
    {
        DataSet ActorSource
        {
            get;
            set;
        }

        DataSet AssigmentSource
        {
            get;
        }

        DateTime GanttFirstDate
        {
            get;
            set;
        }

        DateTime GanttLastDate
        {
            get;
            set;
        }

        DataSet GanttSource
        {
            get;
        }

        DataSet TaskSource
        {
            get;
            set;
        }

        DataSet TaskStateConnectionsSource
        {
            get;
            set;
        }

        DataSet TaskStateSource
        {
            get;
            set;
        }

        void BindActor(IManagerEntity actorEntity);

        void BindProject(IManagerEntity taskEntity);

        void BindTask(IManagerEntity taskEntity);

        void BindTaskState(IManagerEntity stateEntity);

        void BindTaskStateConnection(IManagerEntity stateConnectionEntity);

        IManagerEntity CreateActor();

        IManagerEntity CreateTask();

        IManagerEntity CreateTaskState();

        IManagerEntity CreateTaskStateConnection(IManagerEntity stateEntity, IManagerEntity connectedStateEntity);

        void DeleteActor(int id);

        void DeleteTask(int id);

        void DeleteTaskState(int id);

        void DeleteTaskStateConnection(int id);

        IManagerEntity GetActor(int id);

        DataSet GetInitialTaskStateSource();

        IManagerEntity GetTask(int id);

        IManagerEntity GetTaskState(int id);

        IManagerEntity GetTaskStateConnection(int id);

        DataSet GetTaskStateSource(IManagerEntity state);

        bool IsUpdatedActor(IManagerEntity actorEntity);

        bool IsUpdatedTask(IManagerEntity taskEntity);

        bool IsUpdatedTaskState(IManagerEntity stateEntity);

        bool isUpdatedTaskStateConnection(IManagerEntity stateConnectionEntity);

        void UpdateActor(IManagerEntity actorEntity);

        void UpdateTask(IManagerEntity taskEntity);

        //void BindTaskComment(IManagerEntity stateEntity);
        void UpdateTaskState(IManagerEntity stateEntity);

        void UpdateTaskStateConnection(IManagerEntity stateConnectionEntity);

		System.Threading.Tasks.Task SyncronizeAsync();
    }
}