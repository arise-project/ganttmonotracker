//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 10.11.2005 at 1:31

namespace TaskManagerInterface
{
    using System.Data;

    public interface IGuiTracker
    {
        DataSet ActorSource
        {
            get;
            set;
        }

        DataSet StateSource
        {
            get;
            set;
        }

        DataSet TaskSource
        {
            get;
            set;
        }

        void BindActor();

        void BindState();

        void BindTask();
    }
}