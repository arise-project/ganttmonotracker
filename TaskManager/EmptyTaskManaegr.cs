//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 27.11.2005 at 21:32

using System;
using System.Data;
using TaskManagerInterface;
using GanttTracker.TaskManager.ManagerException;
using GanttTracker.TaskManager.TaskStorage;

namespace GanttTracker.TaskManager
{
	public class EmptyTaskManager : ITaskManager
	{
		private IStorageDealer fDealer;


		private string fConnectionString;


		public EmptyTaskManager( )
		{
			Initialize();
		}


		public EmptyTaskManager(string connectionString)
		{
			fConnectionString = connectionString;
			Initialize();
		}


		private void Initialize()
		{
			fDealer = new  StorageDealer(fConnectionString, new CommandFactory());
		}
		
	   #region Tasks	
		
		private DataSet fTaskSource;
		public DataSet TaskSource
		{
			get
			{
				if (fTaskSource == null)
				{
					fTaskSource = new DataSet("TaskSource");
					fTaskSource.Tables.Add(fDealer.EmptyStorage.Tables["Task"].Copy());
				}
					
				return fTaskSource;
			}
			set
			{
				
			}
		}
		
		public IManagerEntity GetTask(int id)
		{
			return null;
		}
		
		public IManagerEntity CreateTask()
		{
			return null;
		}
		
		public void BindTask(IManagerEntity taskEntity)
		{

		}
		
		public void UpdateTask(IManagerEntity taskEntity)
		{

		}
		
		public bool isUpdatedTask(IManagerEntity taskEntity)
		{
			return false;
		}
		
		public void DeleteTask(int id)
		{

		}
		
		#endregion
		
		#region Actors
		
		private DataSet fActorSource;
		public DataSet ActorSource
		{
			get
			{
				if (fActorSource == null)
				{
					fActorSource = new DataSet("ActorSource");
					fActorSource.Tables.Add(fDealer.EmptyStorage.Tables["Actor"].Copy());
				}
				return fActorSource;
			}
			
			set
			{
				
			}
			
		}
		
		public IManagerEntity GetActor(int id)
		{
			return null;
		}
		
		public IManagerEntity CreateActor()
		{
			return null;
		}
		
		public void BindActor(IManagerEntity actorEntity)
		{

		}
		
		public void UpdateActor(IManagerEntity actorEntity)
		{

		}
		
		public bool isUpdatedActor(IManagerEntity actorEntity)
		{
			return false;
		}
		
		public void DeleteActor(int id)
		{

		}
		
		#endregion
		
		#region Gantt
				
		private DataSet fGanttSource;

		public DataSet GanttSource { get;set; }
		
		public DateTime GanttFirstDate	{get;set; }
		
		public DateTime GanttLastDate { get; set; }
		
		#endregion
		
		#region Assigment
		
		public DataSet AssigmentSource {get;set; }
		
		#endregion
		
		#region TaskState
		
		public DataSet fTaskStateSource;
		public DataSet TaskStateSource
		{
			get
			{
				if (fTaskStateSource == null)
				{
					fTaskStateSource = new DataSet("TaskStateSource");
					fTaskStateSource.Tables.Add(fDealer.EmptyStorage.Tables["TaskState"].Copy());
				}
				return fTaskStateSource;
			}
			
			set
			{

			}
		}
		
		public DataSet GetInitialTaskStateSource()
		{
			return null;
		}
		
		public DataSet GetTaskStateSource(IManagerEntity state)
		{
			return null;
		}
		
		public IManagerEntity GetTaskState(int id)
		{
			return null;
		}
		
		public IManagerEntity CreateTaskState()
		{
			return null;
		}
		
		public void BindTaskState(IManagerEntity stateEntity)
		{

		}

		public void BindTaskComment(IManagerEntity stateEntity)
		{

		}
				
		public void UpdateTaskState(IManagerEntity stateEntity)
		{

		}
		
		public bool isUpdatedTaskState(IManagerEntity stateEntity)
		{
			return false;
		}
		
		public void DeleteTaskState(int id)
		{

		}
		
		#endregion
		
		#region Task State Connection
		
		public DataSet TaskStateConnectionsSource {get;set;	}
		
		public IManagerEntity GetTaskStateConnection(int id)
		{
			return null;
		}
		
		public IManagerEntity CreateTaskStateConnection(IManagerEntity stateEntity, IManagerEntity connectedStateEntity)
		{
			return null;
		}
		
		public void BindTaskStateConnection(IManagerEntity stateConnectionEntity)
		{

		}
		
		public void UpdateTaskStateConnection(IManagerEntity stateConnectionEntity)
		{

		}
		
		public bool isUpdatedTaskStateConnection(IManagerEntity stateConnectionEntity)
		{
			return false;
		}
		
		public void DeleteTaskStateConnection(int id)
		{

		}
		
		#endregion
		
		public void Save()
		{			
			if (fConnectionString != null)
					fDealer.Create();
		}
		
		public void Update(IStorageDealer updateDealer)
		{

		}
	}
}