//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 29.01.2006 at 15:04

namespace TaskManagerInterface
{
    using System.Data;

    public interface IGuiConnection
    {
        int MappingID
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        int StateID
        {
            get;
            set;
        }

        DataSet TaskStateSource
        {
            get;
            set;
        }

        void BindStateIn();

        void BindStateOut();
    }
}