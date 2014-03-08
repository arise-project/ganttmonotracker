// created on 19.11.2005 at 21:36

using System.Data;

namespace TaskManagerInterface
{
	public interface IStorageDealer
	{
		string ConnectionString
		{
			get;
			set;
		}
		
		DataSet Storage
		{
			get;
			set;		
		}
		
		DataSet EmptyStorage
		{
			get;
			set;		
		}
		
		void Create();
		void Load();
		void Save();
		void Save(string connectionString);
		
		DataSet ExecuteDataSet(IStorageCommand command);
		object ExecuteScalar(IStorageCommand command);
		void ExecuteNonQuery(IStorageCommand command);
		
		bool CheckConnection();
		
		IStorageCommandFactory CommandFactory
		{
			get;
			set;
		}
						
	}
}