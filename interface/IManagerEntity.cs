//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.11.2005 at 23:05
namespace TaskManagerInterface
{
    public interface IManagerEntity
    {
        int Id
        {
            get;
            set;
        }

        bool IsNew
        {
            get;
        }

        bool isUpdated
        {
            get;
        }

        ITaskManager Parent
        {
            get;
            set;
        }

        void BindData();

        void Delete();

        void Save();
    }
}