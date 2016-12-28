//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 15.01.2006 at 1:25

namespace TaskManagerInterface
{
    using System.Data;

    public interface IGuiSource
    {
        DataSet Source
        {
            get;
            set;
        }
    }
}