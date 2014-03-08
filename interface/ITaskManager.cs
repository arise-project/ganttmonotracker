// created on 18.11.2005 at 21:52

using System;
using System.Data;
 
namespace TaskManagerInterface
{
	public interface ITaskManager : IStorageManager
	{
		
		#region Tasks
		
		DataSet TaskSource
		{
			get;
			set;			
		}
		
		IManagerEntity GetTask(int id);
		IManagerEntity CreateTask();
		void BindTask(IManagerEntity taskEntity);
		void UpdateTask(IManagerEntity taskEntity);
		bool isUpdatedTask(IManagerEntity taskEntity);
		void DeleteTask(int id);			
		
		#endregion
		
		#region Actors
		
		DataSet ActorSource
		{
			get;
			set;
		}
		
		IManagerEntity GetActor(int id);
		IManagerEntity CreateActor();
		
		void BindActor(IManagerEntity actorEntity);
		void UpdateActor(IManagerEntity actorEntity);
		bool isUpdatedActor(IManagerEntity actorEntity);
		void DeleteActor(int id);		
		
		#endregion
		
		#region Gantt
		
		DataSet GanttSource
		{
			get;
			set;
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
		
		#endregion
		
		#region Assigment
		
		DataSet AssigmentSource
		{
			get;
			set;
		}		
		
		#endregion	
		
		#region Task State
		
		DataSet TaskStateSource
		{
			get;
			set;
		}
		
		IManagerEntity GetTaskState(int id);
		IManagerEntity CreateTaskState();
		
		void BindTaskState(IManagerEntity stateEntity);
		void UpdateTaskState(IManagerEntity stateEntity);
		bool isUpdatedTaskState(IManagerEntity stateEntity);
		void DeleteTaskState(int id);
		
		#endregion
		
		#region Task State Connection		
		
		DataSet TaskStateConnectionsSource
		{
			get;
			set;
		}
		
		IManagerEntity GetTaskStateConnection(int id);
		IManagerEntity CreateTaskStateConnection(IManagerEntity stateEntity, IManagerEntity connectedStateEntity);
		
		void BindTaskStateConnection(IManagerEntity stateConnectionEntity);
		void UpdateTaskStateConnection(IManagerEntity stateConnectionEntity);
		bool isUpdatedTaskStateConnection(IManagerEntity stateConnectionEntity);
		void DeleteTaskStateConnection(int id);
		
		DataSet GetInitialTaskStateSource();		
		DataSet GetTaskStateSource(IManagerEntity state);		
		
		#endregion
		
		#region Comment
		
		DataSet CommentSource
		{
			get;
			set;
		}
		
		IManagerEntity GetComment(int id);
		IManagerEntity CreateComment(IManagerEntity commentedEntity);		
		void BindComment(IManagerEntity commentEntity);
		void UpdateComment(IManagerEntity commentEntity);
		bool isUpdatedComment(IManagerEntity commentEntity);
		void DeleteComment(int id);	
		
		#endregion	
			 
	}
}