﻿//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 30.12.2005 at 22:00
namespace TaskManagerInterface
{
    public interface IGuiTask : ITask, IGuiMessageDialog
    {
        int Priority { get; }
    }
}