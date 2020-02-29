//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.12.2005 at 23:57

namespace TaskManagerInterface
{
    using System;

    public interface IGuiMessageDialog : IDisposable
    {
        string Title
        {
            get;
            set;
        }

        int ShowDialog();
    }
}