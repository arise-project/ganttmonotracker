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
			throw new ImplementationException();
		}
		
		public IManagerEntity CreateTask()
		{
			throw new ImplementationException();
		}
		
		public void BindTask(IManagerEntity taskEntity)
		{
			throw new ImplementationException();
		}
		
		public void UpdateTask(IManagerEntity taskEntity)
		{
			throw new ImplementationException();
		}
		
		public bool isUpdatedTask(IManagerEntity taskEntity)
		{
			throw new ImplementationException();
		}
		
		public void DeleteTask(int id)
		{
			throw new ImplementationException();
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
			throw new ImplementationException();
		}
		
		public IManagerEntity CreateActor()
		{
			throw new ImplementationException();
		}
		
		public void BindActor(IManagerEntity actorEntity)
		{
			throw new ImplementationException();
		}
		
		public void UpdateActor(IManagerEntity actorEntity)
		{
			throw new ImplementationException();
		}
		
		public bool isUpdatedActor(IManagerEntity actorEntity)
		{
			throw new ImplementationException();
		}
		
		public void DeleteActor(int id)
		{
			throw new ImplementationException();
		}		
		
		#endregion
		
		#region Gantt
				
		private DataSet fGanttSource;
		public DataSet GanttSource
		{
			get
			{
				throw new ImplementationException(); 
			}
			
			set
			{
				throw new ImplementationException();
			}		
		}		
		
		public DateTime GanttFirstDate
		{
			get
			{
				throw new ImplementationException();
			}
			
			set
			{
				throw new ImplementationException();
			}
			
		}		
		
		public DateTime GanttLastDate
		{
			get
			{
				throw new ImplementationException(); 
			}
			
			set
			{
				throw new ImplementationException();
			}
		
		}
		
		#endregion
		
		#region Assigment		
		
		public DataSet AssigmentSource
		{
			get
			{
				throw new ImplementationException();
			}
			
			set
			{
				throw new ImplementationException();
			}
			
		}		
		
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
				throw new ImplementationException();
			}
		}
		
		public DataSet GetInitialTaskStateSource()
		{
			throw new ImplementationException();
		}
		
		public DataSet GetTaskStateSource(IManagerEntity state)
		{
			throw new ImplementationException();
		}
		
		public IManagerEntity GetTaskState(int id)
		{
			throw new ImplementationException();
		}
		
		public IManagerEntity CreateTaskState()
		{
			throw new ImplementationException();
		}
		
		public void BindTaskState(IManagerEntity stateEntity)
		{
			throw new ImplementationException();
		}
		
		public void UpdateTaskState(IManagerEntity stateEntity)
		{
			throw new ImplementationException();
		}
		
		public bool isUpdatedTaskState(IManagerEntity stateEntity)
		{
			throw new ImplementationException();
		}
		
		public void DeleteTaskState(int id)
		{
			throw new ImplementationException();
		}
		
		#endregion
		
		#region Task State Connection		
		
		public DataSet TaskStateConnectionsSource
		{
			get
			{
				throw new ImplementationException();
			}
			
			set
			{
				throw new ImplementationException();
			}
		}
		
		public IManagerEntity GetTaskStateConnection(int id)
		{
			throw new ImplementationException();
		}
		
		public IManagerEntity CreateTaskStateConnection(IManagerEntity stateEntity, IManagerEntity connectedStateEntity)
		{
			throw new ImplementationException();
		}
		
		public void BindTaskStateConnection(IManagerEntity stateConnectionEntity)
		{
			throw new ImplementationException();
		}
		
		public void UpdateTaskStateConnection(IManagerEntity stateConnectionEntity)
		{
			throw new ImplementationException();
		}
		
		public bool isUpdatedTaskStateConnection(IManagerEntity stateConnectionEntity)
		{
			throw new ImplementationException();
		}
		
		public void DeleteTaskStateConnection(int id)
		{
			throw new ImplementationException();
		}
		
		#endregion		
		
		public void Save()
		{			
			if (fConnectionString == null)
					throw new ImplementationException();
			fDealer.Create();
		}
		
		public void Update(IStorageDealer updateDealer)
		{
			throw new ImplementationException();
		}
		
		#region Comment
		
		private DataSet fCommentSource;		
		public DataSet CommentSource
		{
			get
			{
				if (fCommentSource == null)
				{
					fCommentSource = new DataSet("CommentSource");					
					fCommentSource.Tables.Add(fDealer.EmptyStorage.Tables["Comment"].Copy());
				}
				return fCommentSource;
			}
			
			set
			{
			}
		}
		
		public IManagerEntity GetComment(int id)
		{
			throw new ImplementationException();
		}
		
		public IManagerEntity CreateComment(IManagerEntity commentedEntity)
		{
			throw new ImplementationException();
		}
				
		public void BindComment(IManagerEntity commentEntity)
		{
			throw new ImplementationException();
		}
		
		public void UpdateComment(IManagerEntity commentEntity)
		{
			throw new ImplementationException();
		}
		
		public bool isUpdatedComment(IManagerEntity commentEntity)
		{
			throw new ImplementationException();
		}
		
		public void DeleteComment(int id)
		{
			throw new ImplementationException();
		}	
		
		#endregion
	}			
}