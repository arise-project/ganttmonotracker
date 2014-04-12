//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 28.11.2005 at 0:58

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Data;
using TaskManagerInterface;
using GanttTracker.TaskManager.ManagerException;

namespace GanttTracker.TaskManager.TaskStorage
{
	public class StorageDealer : IStorageDealer
	{
		private string fConnectionString;
		public string ConnectionString
		{
			get
			{
				return fConnectionString;
			}
			
			set
			{
				fConnectionString = value;
			}
		}	
		
		private DataSet fStorage;
		public DataSet Storage
		{
			get
			{
				return fStorage;
			}
			set
			{
				fStorage = value;
			}
		}
		
		private DataSet fEmptyStorage;
		public DataSet EmptyStorage
		{
			get
			{
				if (fEmptyStorage == null)
				{
					fEmptyStorage = new DataSet("Track");
					DataTable taskTable = new DataTable("Task");
					DataTable actorTable = new DataTable("Actor");
					DataTable taskStateTable = new DataTable("TaskState");
					DataTable taskStateConnectionTable = new DataTable("TaskStateConnection");
					DataTable commentTable = new DataTable("Comment");
					
					actorTable.Columns.Add("ID",typeof(int));								
					actorTable.Columns.Add("Name",typeof(string));
					actorTable.Columns.Add("Email",typeof(string));
						
					taskTable.Columns.Add("ID",typeof(int));
					taskTable.Columns.Add("ActorID",typeof(int));
					taskTable.Columns.Add("Description",typeof(string));					
					taskTable.Columns.Add("StartTime",typeof(DateTime));
					taskTable.Columns.Add("EndTime",typeof(DateTime));
					taskTable.Columns.Add("StateID",typeof(int));
					
					taskStateTable.Columns.Add("ID",typeof(int));					
					taskStateTable.Columns.Add("Name",typeof(string));
					taskStateTable.Columns.Add("ColorBlue",typeof(byte));
					taskStateTable.Columns.Add("ColorRed",typeof(byte));
					taskStateTable.Columns.Add("ColorGreen",typeof(byte));
					taskStateTable.Columns.Add("MappingID",typeof(int));
					
					taskStateConnectionTable.Columns.Add("ID",typeof(int));
					taskStateConnectionTable.Columns.Add("Name",typeof(string));			
					taskStateConnectionTable.Columns.Add("MappingID",typeof(int));
					taskStateConnectionTable.Columns.Add("StateID",typeof(int));
					
					commentTable.Columns.Add("ID",typeof(int));
					commentTable.Columns.Add("EntryID",typeof(int));
					commentTable.Columns.Add("Description",typeof(string));
					commentTable.Columns.Add("Date",typeof(DateTime));					
					
					fEmptyStorage.Tables.Add(actorTable);
					fEmptyStorage.Tables.Add(taskTable);
					fEmptyStorage.Tables.Add(taskStateTable);
					fEmptyStorage.Tables.Add(taskStateConnectionTable);
					fEmptyStorage.Tables.Add(commentTable);
					
					fEmptyStorage.Relations.Add("Relation_Actor_Task_ActorID", actorTable.Columns["ID"],taskTable.Columns["ActorID"]);
					fEmptyStorage.Relations.Add("Relation_TaskState_Task_ActorID",taskStateTable.Columns["ID"],taskTable.Columns["StateID"]);
					fEmptyStorage.Relations.Add("Relation_TaskState_TaskStateConnection_ActorID",taskStateTable.Columns["MappingID"],taskStateConnectionTable.Columns["MappingID"]);
					fEmptyStorage.Relations.Add("Relation_Task_Comment_TaskID",taskTable.Columns["ID"],commentTable.Columns["EntryID"]);				
					
				}
				return fEmptyStorage;			
			}
			
			set
			{
				
			}			
		}
		
		public StorageDealer(string connectionString,IStorageCommandFactory commandFactory)
		{
			fConnectionString = connectionString;
			fCommandFactory = commandFactory;
		}
		
		public void Create()
		{
			if (File.Exists(fConnectionString))
				throw new NotAllowedException();	
			
			EmptyStorage.WriteXml(fConnectionString, System.Data.XmlWriteMode.WriteSchema);
			EmptyStorage.WriteXmlSchema(fConnectionString + ".xsd");		 			 
		}	
		
		public void Load()
		{
			XmlTextReader reader = new XmlTextReader(fConnectionString);
			/*XmlValidatingReader validator = new XmlValidatingReader(reader);
			validator.ValidationType = ValidationType.Schema; 
			validator.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);
			while(validator.Read())
			{
				if(validator.NodeType == XmlNodeType.Element)
				{
					while (validator.MoveToNextAttribute ())
					{
					}
				}
			}
			validator.Close();
			*/
			fStorage = new DataSet();
			fStorage.ReadXml(fConnectionString);		
		}
		
		private void ValidationHandler(object sender, ValidationEventArgs args)
		{			 
			throw new ValidationException("Validation failed with message " + args.Message);
		}
		
		public void Save()
		{
			fStorage.WriteXml(fConnectionString, System.Data.XmlWriteMode.WriteSchema);
			fStorage.WriteXmlSchema(fConnectionString + ".xsd");		
		}
		
		public void Save(string connectionString)
		{			
			fStorage.WriteXml(connectionString, System.Data.XmlWriteMode.WriteSchema);
			fStorage.WriteXmlSchema(connectionString + ".xsd");		
		}
				
		public DataSet ExecuteDataSet(IStorageCommand command)
		{			
			return (DataSet)command.Execute();
		}
		
		public object ExecuteScalar(IStorageCommand command)
		{
			return command.Execute();
		}
		
		public void ExecuteNonQuery(IStorageCommand command)
		{
			command.Execute();			
		}
		
		public bool CheckConnection()
		{
			return !(fConnectionString == null || !File.Exists(fConnectionString));
		}
		
		private IStorageCommandFactory fCommandFactory;
		public IStorageCommandFactory CommandFactory
		{
			get
			{
				return fCommandFactory;				
			}
			
			set
			{
				fCommandFactory = value;
			}
		}
	}
}