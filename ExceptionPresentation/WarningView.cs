//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.12.2005 at 23:16

using System;
using System.Threading;
using Gtk;
using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;
 
namespace GanttMonoTracker.ExceptionPresentation
{
	public class  WarningView : MessageDialog, IGuiMessageDialog
	{
		public WarningView(Exception exception, Window parent) 
			: base (parent,  
				DialogFlags.DestroyWithParent,
				Gtk.MessageType.Warning,Gtk.ButtonsType.Ok,
				string.Format("Warning : {0}{1}", Environment.NewLine, Environment.NewLine+exception.Message))
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
			for (; true; ) 
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