//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 19.11.2005 at 21:36

using Task = System.Threading.Tasks.Task;

namespace TaskManagerInterface
{
	using System;
	using System.Data;
	using System.Threading.Tasks;

	public interface IStorageRepository
    {
        IRepositoryCruid CommandFactory
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

        Task<bool> BackupAsync(string fileId, bool exists);

        bool CheckConnection();

        void Create();

        DataSet ExecuteDataSet(IStorageCommand command);

        void ExecuteNonQuery(IStorageCommand command);

        object ExecuteScalar(IStorageCommand command);

        void Load();

        Task<bool> MergeAsync(string fileId, DateTime currentDate, Func<DataSet, DateTime> readDate);

        Task<bool> RestoreAsync(string fileId);

        Task Revoke();

        void Save();

        void Save(string connectionString);
    }
}