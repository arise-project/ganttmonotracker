// created on 22.01.2006 at 2:49

namespace TaskManagerInterface
{
	public interface IStorageManager
	{
		void Update(IStorageDealer updateDealer);

		void Save();
	}
}