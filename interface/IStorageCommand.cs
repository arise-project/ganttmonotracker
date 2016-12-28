//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.11.2005 at 22:53
namespace TaskManagerInterface
{
    public interface IStorageCommand
    {
        void CheckParams();

        object Contains(object key);

        object Execute();

        object GetParam(object key);

        object[] GetParamKeys();

        void SetParam(object key, object value);
    }
}