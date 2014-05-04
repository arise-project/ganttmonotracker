//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.11.2005 at 21:52

using System;
using System.Data;
 
namespace TaskManagerInterface
{
	public interface ITaskManager : IStorageManager
	{
		#region Tasks
		
		DataSet TaskSource { get;set; }


		IManagerEntity GetTask(int id);


		IManagerEntity CreateTask();


		void BindTask(IManagerEntity taskEntity);


		void UpdateTask(IManagerEntity taskEntity);


		bool IsUpdatedTask(IManagerEntity taskEntity);


		void DeleteTask(int id);
		
		#endregion
		
		#region Actors
		
		DataSet ActorSource	{ get;set; }


		IManagerEntity GetActor(int id);


		IManagerEntity CreateActor();


		void BindActor(IManagerEntity actorEntity);


		void UpdateActor(IManagerEntity actorEntity);


		bool IsUpdatedActor(IManagerEntity actorEntity);


		void DeleteActor(int id);
		
		#endregion
		
		#region Gantt
		
		DataSet GanttSource	{ get; }


		DateTime GanttFirstDate	{get;set; }


		DateTime GanttLastDate { get;set; }
		
		#endregion
		
		#region Assigment
		
		DataSet AssigmentSource	{ get; }
		
		#endregion	
		
		#region Task State
		
		DataSet TaskStateSource { get;set; }


		IManagerEntity GetTaskState(int id);


		IManagerEntity CreateTaskState();


		void BindTaskState(IManagerEntity stateEntity);


		//void BindTaskComment(IManagerEntity stateEntity);


		void UpdateTaskState(IManagerEntity stateEntity);


		bool IsUpdatedTaskState(IManagerEntity stateEntity);


		void DeleteTaskState(int id);
		
		#endregion
		
		#region Task State Connection
		
		DataSet TaskStateConnectionsSource { get;set; }


		IManagerEntity GetTaskStateConnection(int id);


		IManagerEntity CreateTaskStateConnection(IManagerEntity stateEntity, IManagerEntity connectedStateEntity);


		void BindTaskStateConnection(IManagerEntity stateConnectionEntity);


		void UpdateTaskStateConnection(IManagerEntity stateConnectionEntity);


		bool isUpdatedTaskStateConnection(IManagerEntity stateConnectionEntity);


		void DeleteTaskStateConnection(int id);


		DataSet GetInitialTaskStateSource();


		DataSet GetTaskStateSource(IManagerEntity state);
		
		#endregion
	}
}