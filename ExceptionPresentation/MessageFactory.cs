//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.12.2005 at 23:37

namespace GanttMonoTracker.ExceptionPresentation
{
    using System;

    using Gtk;

    using TaskManagerInterface;

    public static class MessageFactory
    {
        public static IGuiMessageDialog CreateErrorDialog(Exception exception, Window parent)
        {
            ExceptionViewDialog dialog = new ExceptionViewDialog(exception, parent);
            dialog.Title = "Bug";
            return dialog;
        }

        public static IGuiMessageDialog CreateMessageDialog(string message, Window parent)
        {
            MessageViewDialog dialog = new MessageViewDialog(message,parent);
            dialog.Title = "Gantt Tracker Warning";
            return dialog;
        }
    }
}