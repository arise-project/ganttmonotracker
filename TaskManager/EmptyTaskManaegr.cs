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
using GanttMonoTracker;

namespace GanttTracker.TaskManager
{
	public class EmptyTaskManager : ITaskManager
	{
		IStorageDealer fDealer;


		string fConnectionString;


		DataSet fTaskSource;


		DataSet fActorSource;


		DataSet fTaskStateSource;

		public EmptyTaskManager( )
		{
			Initialize();
		}


		public EmptyTaskManager(string connectionString)
		{
			fConnectionString = connectionString;
			Initialize();
		}


		void Initialize()
		{
			fDealer = new  StorageDealer(fConnectionString, new CommandFactory());
		}
		
		#region Tasks

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
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		
		public IManagerEntity CreateTask()
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		
		public void BindTask(IManagerEntity taskEntity)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		
		public void UpdateTask(IManagerEntity taskEntity)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		
		public bool IsUpdatedTask(IManagerEntity taskEntity)
		{
			return false;
		}

		
		public void DeleteTask(int id)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}
		
		#endregion
		
		#region Actors

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
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		
		public IManagerEntity CreateActor()
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		
		public void BindActor(IManagerEntity actorEntity)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		
		public void UpdateActor(IManagerEntity actorEntity)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		
		public bool IsUpdatedActor(IManagerEntity actorEntity)
		{
			return false;
		}

		
		public void DeleteActor(int id)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}
		
		#endregion
		
		#region Gantt
				
		public DataSet GanttSource { get;set; }

		
		public DateTime GanttFirstDate	{get;set; }

		
		public DateTime GanttLastDate { get; set; }
		
		#endregion
		
		#region Assigment
		
		public DataSet AssigmentSource {get;set; }
		
		#endregion
		
		#region TaskState

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
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		
		public DataSet GetTaskStateSource(IManagerEntity state)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		
		public IManagerEntity GetTaskState(int id)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		
		public IManagerEntity CreateTaskState()
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		
		public void BindTaskState(IManagerEntity stateEntity)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}


		public void BindTaskComment(IManagerEntity stateEntity)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}
				
		public void UpdateTaskState(IManagerEntity stateEntity)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}


		public bool IsUpdatedTaskState(IManagerEntity stateEntity)
		{
			return false;
		}


		public void DeleteTaskState(int id)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}
		
		#endregion
		
		#region Task State Connection
		
		public DataSet TaskStateConnectionsSource {get;set;	}


		public IManagerEntity GetTaskStateConnection(int id)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}


		public IManagerEntity CreateTaskStateConnection(IManagerEntity stateEntity, IManagerEntity connectedStateEntity)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}


		public void BindTaskStateConnection(IManagerEntity stateConnectionEntity)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}


		public void UpdateTaskStateConnection(IManagerEntity stateConnectionEntity)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}


		public bool isUpdatedTaskStateConnection(IManagerEntity stateConnectionEntity)
		{
			return false;
		}


		public void DeleteTaskStateConnection(int id)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}
		
		#endregion
		
		public void Save()
		{			
			if (fConnectionString != null)
				fDealer.Create();
		}


		public void Update(IStorageDealer updateDealer)
		{
			throw new ManagementException(ExceptionType.NotAllowed);
		}

		public void BindProject(IManagerEntity taskEntity)
		{
			throw new NotImplementedException();
		}
	}
}