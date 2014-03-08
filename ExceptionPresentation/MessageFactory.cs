// created on 21.12.2005 at 23:37

using System;
using Gtk;
using TaskManagerInterface;

namespace GanttMonoTracker.ExceptionPresentation
{
	public class MessageFactory
	{
		private MessageFactory()
		{
		}
		
		private static MessageFactory fInstance;
		public static MessageFactory Instance
		{
			get
			{
				if (fInstance == null)
				{
					fInstance = new MessageFactory();
				}
				return fInstance;
			}
			set
			{
			}
		}
		
		public IGuiMessageDialog CreateErrorDialog(Exception exception, Window parent)
		{
			ExceptionViewDialog dialog = new ExceptionViewDialog(exception, parent);
			dialog.Title = "Gantt Tracker Exception";
			return dialog;
		}
		
		public IGuiMessageDialog CreateWarningDialog(Exception exception, Window parent)
		{			
			MessageViewDialog dialog = new MessageViewDialog(exception.GetType().FullName, exception.Message, exception.StackTrace, parent);
			dialog.Title = "Gantt Tracker Warning";						 
			return dialog;  
		}
		
		public IGuiMessageDialog CreateMessageDialog(string message, Window parent)
		{
			MessageViewDialog dialog = new MessageViewDialog(message,parent);
			dialog.Title = "Gantt Tracker Warning";						 
			return dialog;
		}
	}
	
	
}