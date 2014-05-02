//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 04.12.2005 at 1:11

using System;
using System.Data;
using System.Collections;
using TaskManagerInterface;
using GanttTracker.TaskManager.ManagerException;

namespace GanttTracker.TaskManager.TaskStorage
{
	public class SelectCommand : IStorageCommand
	{
		Hashtable fParams = new Hashtable();


		public SelectCommand(DataSet source,string entityName,Hashtable rules)
		{
			Initialize(source,entityName,rules);
		}
		

		private void Initialize(DataSet source,string entityName,Hashtable rules)
		{
			fParams.Add("Source",source);
			fParams.Add("EntityName",entityName);
			fParams.Add("Rules",rules);
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
			return (object [])keys.ToArray(typeof(object));
		}

		
		public void CheckParams()
		{
			if (!fParams.ContainsKey("Source"))
				throw new KeyNotFoundException("Source");
			if (!fParams.ContainsKey("EntityName"))
				throw new KeyNotFoundException("EntityName");
			if (!fParams.ContainsKey("Rules"))
				throw new KeyNotFoundException("Rules");
		}

						
		public object Execute()
		{
			CheckParams();
			
			DataSet source = (DataSet)fParams["Source"];
			Hashtable rules =	(Hashtable)fParams["Rules"];
			
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
				throw new KeyNotFoundException(string.Format("Table with name {0} Not Found {1}", fParams["EntityName"], fParams["EntityName"]));
			
			string rule = "";
			
			foreach(object key in rules.Keys)
			{
				rule += rules[key].ToString() + " ";
			}
			if (rule.Length > 1)
				rule = rule.Substring(0,rule.Length - 1);
			
			DataSet ds = new DataSet();
			
			if (rules.Count > 0)
			{
				ds.Tables.Add(entityTable.Clone());
				ds.Tables[0].TableName = fParams["EntityName"].ToString();
				DataRow [] selectedRows = entityTable.Select(rule);			  
				if (selectedRows.Length > 0)
				{
					foreach(DataRow selectedRow in selectedRows)
					{
						ds.Tables[0].Rows.Add((object [])selectedRow.ItemArray.Clone());
					} 	
				}
			}
			else
				ds.Tables.Add(entityTable.Copy());
			return ds;
		}
	}
}
