// created on 04.12.2005 at 1:13

using System.Data;
using System.Collections;
using TaskManagerInterface;
using GanttTracker.TaskManager.ManagerException;

namespace GanttTracker.TaskManager.TaskStorage
{
	public class DeleteCommand : IStorageCommand
	{
		public DeleteCommand(DataSet source,string entityName,Hashtable rules)
		{
			Initialize(source,entityName,rules);
		}		
		
		private 	Hashtable fParams = new Hashtable();
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
				throw new KeyNotFoundException("Table with name " + fParams["EntityName"].ToString() + "Not Found");
			
			string rule = "";
			
			foreach(object key in rules.Keys)
			{
				rule += rules[key].ToString() + ", ";
			}
			if (rule.Length > 1)
				rule = rule.Substring(0,rule.Length - 2);
			
			int count = 0;
			 foreach (DataRow row in entityTable.Select(rule))
			 {
			 	row.Delete();
			 	count++;
			 }
			 entityTable.AcceptChanges();
			 
			 return count;
		}				
	}
}
