//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 10.02.2006 at 12:05

namespace TaskManagerInterface
{
    using System;

    public interface IGuiComment
    {
        int CommentedEntryID
        {
            get;
            set;
        }

        bool CommentedEntryPresent
        {
            get;
            set;
        }

        DateTime Date
        {
            get;
            set;
        }

        string Description
        {
            get;
            set;
        }
    }
}