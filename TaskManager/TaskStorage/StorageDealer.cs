﻿//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 28.11.2005 at 0:58

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Data;

using TaskManagerInterface;
using GanttTracker.TaskManager.ManagerException;
using GanttMonoTracker;
using System.Threading.Tasks;

namespace GanttTracker.TaskManager.TaskStorage
{
    public class StorageDealer : IStorageDealer
    {
        DataSet fEmptyStorage;

        public string ConnectionString { get; set; }

        public DataSet Storage { get; set; }

        public IDealerCruid CommandFactory { get; set; }

        public GDriveManager Online { get; set; }

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
                    var tables = new List<DataTable> { taskTable, actorTable, taskStateTable, taskStateConnectionTable, commentTable };

                    actorTable.Columns.Add("ID", typeof(int));
                    actorTable.Columns.Add("Name", typeof(string));
                    actorTable.Columns.Add("Email", typeof(string));

                    taskTable.Columns.Add("ID", typeof(int));
                    taskTable.Columns.Add("ActorID", typeof(int));
                    taskTable.Columns.Add("Description", typeof(string));
                    taskTable.Columns.Add("StartTime", typeof(DateTime));
                    taskTable.Columns.Add("EndTime", typeof(DateTime));
                    taskTable.Columns.Add("StateID", typeof(int));

                    taskStateTable.Columns.Add("ID", typeof(int));
                    taskStateTable.Columns.Add("Name", typeof(string));
                    taskStateTable.Columns.Add("ColorBlue", typeof(byte));
                    taskStateTable.Columns.Add("ColorRed", typeof(byte));
                    taskStateTable.Columns.Add("ColorGreen", typeof(byte));
                    taskStateTable.Columns.Add("MappingID", typeof(int));

                    taskStateConnectionTable.Columns.Add("ID", typeof(int));
                    taskStateConnectionTable.Columns.Add("Name", typeof(string));
                    taskStateConnectionTable.Columns.Add("MappingID", typeof(int));
                    taskStateConnectionTable.Columns.Add("StateID", typeof(int));

                    commentTable.Columns.Add("ID", typeof(int));
                    commentTable.Columns.Add("EntryID", typeof(int));
                    commentTable.Columns.Add("Description", typeof(string));
                    commentTable.Columns.Add("Date", typeof(DateTime));

                    tables.ForEach(fEmptyStorage.Tables.Add);

                    fEmptyStorage.Relations.Add("Relation_Actor_Task_ActorID", actorTable.Columns["ID"], taskTable.Columns["ActorID"]);
                    fEmptyStorage.Relations.Add("Relation_TaskState_Task_ActorID", taskStateTable.Columns["ID"], taskTable.Columns["StateID"]);
                    fEmptyStorage.Relations.Add("Relation_TaskState_TaskStateConnection_ActorID", taskStateTable.Columns["MappingID"], taskStateConnectionTable.Columns["MappingID"]);
                    fEmptyStorage.Relations.Add("Relation_Task_Comment_TaskID", taskTable.Columns["ID"], commentTable.Columns["EntryID"]);
                }

                return fEmptyStorage;
            }
        }

        public StorageDealer(string connectionString, IDealerCruid commandFactory)
        {
            ConnectionString = connectionString;
            CommandFactory = commandFactory;
            CommandFactory.SetDealer(this);
            Online = new GDriveManager();
        }

        public void Create()
        {
            if (File.Exists(ConnectionString))
                throw new ManagementException(ExceptionType.NotAllowed);

            EmptyStorage.WriteXml(ConnectionString, System.Data.XmlWriteMode.WriteSchema);
            EmptyStorage.WriteXmlSchema(string.Format("{0}.xsd", ConnectionString));
        }

        public void Load()
        {
            /*XmlTextReader reader = new XmlTextReader(ConnectionString);
			XmlValidatingReader validator = new XmlValidatingReader(reader);
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

            Storage = new DataSet();
            Storage.ReadXml(ConnectionString);
        }

        private void ValidationHandler(object sender, ValidationEventArgs args)
        {
            throw new ManagementException(ExceptionType.ValidationFailed, string.Format("Validation failed with message {0}", args.Message));
        }

        public void Save()
        {
            Storage.WriteXml(ConnectionString, System.Data.XmlWriteMode.WriteSchema);
            Storage.WriteXmlSchema(string.Format("{0}.xsd", ConnectionString));
        }

        public void Save(string connectionString)
        {
            Storage.WriteXml(connectionString, System.Data.XmlWriteMode.WriteSchema);
            Storage.WriteXmlSchema(string.Format("{0}.xsd", ConnectionString));
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
            return !(ConnectionString == null || !File.Exists(ConnectionString));
        }

        #region Synckronization

        public async Task<bool> BackupAsync(string fileId)
        {
            if (!CheckConnection() || Online == null)
            {
                return false;
            }

            await Online.AuthorizeAsync();
            Save(ConnectionString);
            var raw = File.ReadAllBytes(ConnectionString);
            try
            {
                Online.Uploader.Upload(Online.Credentials, raw, fileId);
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> RestoreAsync(string fileId)
        {
            if (Online == null)
            {
                return false;
            }

            await Online.AuthorizeAsync();
            byte[] raw;
            try
            {
                raw = Online.Downloader.Download(Online.Credentials, fileId);
				if (raw == null)
					return false;
            }
            catch
            {
				//TODO: write good message here
                throw;
            }

            using (Stream s = new MemoryStream(raw))
            {
                var onlineStorage = new DataSet();
                onlineStorage.ReadXml(s, XmlReadMode.Auto);
                if (Storage != null)
                    Storage.Merge(onlineStorage);
                else
                    Storage = onlineStorage;
            }

            return true;
        }

        public async Task<bool> MergeAsync(string fileId, DateTime currentDate, Func<DataSet, DateTime> readDate)
        {
            await Online.AuthorizeAsync();
            if (!CheckConnection())
            {
                return false;
            }

            byte[] raw;
            try
            {
                raw = Online.Downloader.Download(Online.Credentials, fileId);
				if (raw == null)
					return false;
            }
            catch
            {
                return false;
            }

            DataSet update;
            using (Stream s = new MemoryStream(raw))
            {
                update = new DataSet();
                update.ReadXml(s, XmlReadMode.Auto);
            }

            // check modified date.
            var updateDate = readDate(update);
            DataSet src, dest;

            if (updateDate > currentDate)
            {
                src = Storage;
                dest = update;
            }
            else
            {
                src = update;
                dest = Storage;
            }

            src.Merge(dest, true, MissingSchemaAction.AddWithKey);

            Storage = src;
            return true;
        }

        #endregion
    }
}