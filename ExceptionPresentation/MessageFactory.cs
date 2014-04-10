// created on 21.12.2005 at 23:37

using System;
using Gtk;
using TaskManagerInterface;

namespace GanttMonoTracker.ExceptionPresentation
{
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