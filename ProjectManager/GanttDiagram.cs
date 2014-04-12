//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
using System;
using System.Data;

namespace GanttMonoTracker
{
	public class GanttDiagramFactory
	{
		public static DataSet Create()
		{
			var fEmptyStorage = new DataSet("Track");
			var taskTable = new DataTable("Task");
			var actorTable = new DataTable("Actor");
			var taskStateTable = new DataTable("TaskState");
			var taskStateConnectionTable = new DataTable("TaskStateConnection");

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
			taskStateTable.Columns.Add("ColorBlue",typeof(int));
			taskStateTable.Columns.Add("ColorRed",typeof(int));
			taskStateTable.Columns.Add("ColorGreen",typeof(int));
			taskStateTable.Columns.Add("MappingID",typeof(int));

			taskStateConnectionTable.Columns.Add("ID",typeof(int));
			taskStateConnectionTable.Columns.Add("Name",typeof(string));
			taskStateConnectionTable.Columns.Add("MappingID",typeof(int));
			taskStateConnectionTable.Columns.Add("StateID",typeof(int));

			fEmptyStorage.Tables.Add(actorTable);
			fEmptyStorage.Tables.Add(taskTable);
			fEmptyStorage.Tables.Add(taskStateTable);
			fEmptyStorage.Tables.Add(taskStateConnectionTable);

			return fEmptyStorage;
		}
	}
}

