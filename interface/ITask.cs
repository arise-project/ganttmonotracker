//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.12.2005 at 23:11

namespace TaskManagerInterface
{
    using System;

    public interface ITask
    {
        int ActorID
        {
            get;
            set;
        }

        bool ActorPresent
        {
            get;
            set;
        }

        string Description
        {
            get;
            set;
        }

        DateTime EndTime
        {
            get;
            set;
        }

        DateTime StartTime
        {
            get;
            set;
        }

        int StateID
        {
            get;
            set;
        }
    }
}