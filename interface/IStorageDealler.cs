//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 19.11.2005 at 21:36

namespace TaskManagerInterface
{
    using System;
    using System.Data;

    public interface IStorageDealer
    {
        IDealerCruid CommandFactory
        {
            get;
            set;
        }

        string ConnectionString
        {
            get;
            set;
        }

        DataSet EmptyStorage
        {
            get;
        }

        DataSet Storage
        {
            get;
            set;
        }

        bool Backup(string fileId);

        bool CheckConnection();

        void Create();

        DataSet ExecuteDataSet(IStorageCommand command);

        void ExecuteNonQuery(IStorageCommand command);

        object ExecuteScalar(IStorageCommand command);

        void Load();

        bool Merge(string fileId, DateTime currentDate, Func<DataSet, DateTime> readDate);

        bool Restore(string fileId);

        void Save();

        void Save(string connectionString);
    }
}