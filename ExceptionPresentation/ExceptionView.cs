// created on 20.12.2005 at 1:09
using System;
using System.Threading;
using Gtk;
using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;

namespace GanttMonoTracker.ExceptionPresentation
{
	public class ExceptionView : MessageDialog, IGuiMessageDialog
	{
		public ExceptionView(Exception exception, Window parent) : base (parent, DialogFlags.DestroyWithParent, Gtk.MessageType.Error,Gtk.ButtonsType.YesNo, "Application fail with exception : " + Environment.NewLine +Environment.NewLine+exception.Message + Environment.NewLine +Environment.NewLine+ "Send it to develop team?")
		{
			Modal = true;
		}
		
		protected override void OnResponse(ResponseType responseType)
		{
			Result = responseType;
		}

		public ResponseType Result { get;set; }
		
		public int ShowDialog()
		{
			Show();

			int result = 0;
			for (; true;) 
			{
				result = Run();
				if ((result != ((int)(Gtk.ResponseType.None))))
				{
					break;
				}
				Thread.Sleep(500);
			}
			Result = (Gtk.ResponseType)result; 
			return result;
		}
	}
}