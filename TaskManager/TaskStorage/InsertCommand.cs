//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 29.11.2005 at 1:21

using System;
using System.Data;
using System.Collections;
using TaskManagerInterface;
using GanttTracker.TaskManager.ManagerException;
using Arise.Logic;

namespace GanttTracker.TaskManager.TaskStorage
{
	public class InsertCommand : IStorageCommand
	{
		Hashtable fParams = new Hashtable();


		public InsertCommand(DataSet source,string entityName,Hashtable values)
		{
			Initialize(source,entityName,values);
		}		
		

		private void Initialize(DataSet source,string entityName,Hashtable values)
		{
			fParams.Add("Source",source);
			fParams.Add("EntityName",entityName);
			fParams.Add("Values",values);
		}

		
		public void SetParam(object key, object value)
		{
			if (fParams.ContainsKey(key))
				fParams[key] = value;
			else
				fParams.Add(key,value);
		}

		
		public object GetParam(object key)
		{
			if (fParams.ContainsKey(key))
				return fParams[key];
			else
				throw new KeyNotFoundException(key);
		}

		
		public object Contains(object key)
		{
			return fParams.ContainsKey(key);
		}

		
		public object [] GetParamKeys()
		{
			ArrayList keys = new ArrayList();
			foreach (object key in fParams.Keys)
			{
				keys.Add(key);
			} 
			return (object [])keys.ToArray(typeof(object)) ;
		}

		
		public void CheckParams()
		{
			if (!fParams.ContainsKey("Source"))
				throw new KeyNotFoundException("Source");
			if (!fParams.ContainsKey("EntityName"))
				throw new KeyNotFoundException("EntityName");
			if (!fParams.ContainsKey("Values"))
				throw new KeyNotFoundException("Values");
		}

						
		public object Execute()
		{
			CheckParams();
			
			DataSet source = (DataSet)fParams["Source"];
			Hashtable values =	(Hashtable)fParams["Values"];
			
			DataTable entityTable = null;
			foreach(DataTable table in source.Tables)
			{
				if (table.TableName == fParams["EntityName"].ToString() )
				{
					entityTable = table;
					break;
				}
			}
			
			if (entityTable == null)
				throw new KeyNotFoundException(string.Format("Table with name {0} Not Found", fParams["EntityName"]));
			 
			 foreach (object coumn in values.Keys)
			 {
			 	if (!entityTable.Columns.Contains(coumn.ToString()	) )
					throw new KeyNotFoundException(string.Format("Column with name {0} Not Found for table {1}", coumn, entityTable.TableName ));			 	
			 }
			 
			 DataRow entity = entityTable.NewRow();
			 
			 foreach (object coumn in values.Keys)
			 {
			 	entity[coumn.ToString()] = values[coumn];
			 }			 
			 			 
			int id = -1;
			foreach (DataRow row in entityTable.Rows)
			{
				if ((int)row["ID"] > id) id = (int)row["ID"]; 
			}
			
			 entity["ID"] = ++id;
			 entityTable.Rows.Add(entity);
			 return id;
		}
	}
}
