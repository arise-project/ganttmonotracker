//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 19.11.2005 at 21:36

using System.Data;
using System;

namespace TaskManagerInterface
{
	public interface IStorageDealer
	{
		string ConnectionString	{ get;set;	}


		DataSet Storage { get;set; }


		DataSet EmptyStorage { get; }


		void Create();


		void Load();


		void Save();


		void Save(string connectionString);


		DataSet ExecuteDataSet(IStorageCommand command);


		object ExecuteScalar(IStorageCommand command);


		void ExecuteNonQuery(IStorageCommand command);


		bool CheckConnection();


		IDealerCruid CommandFactory { get;set; }

        bool Backup(string fileId);

        bool Restore(string fileId);

        bool Merge(string fileId, DateTime currentDate, Func<DataSet, DateTime> readDate);
	}
}