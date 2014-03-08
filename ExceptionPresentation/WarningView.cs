// created on 21.12.2005 at 23:16

using System;
using Gtk;
using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;
 
namespace GanttMonoTracker.ExceptionPresentation
{
	public class  WarningView : MessageDialog, IGuiMessageDialog
	{
		public WarningView(Exception exception, Window parent) : base (parent,  DialogFlags.DestroyWithParent , Gtk.MessageType.Warning,Gtk.ButtonsType.Ok, "Warning : " + Environment.NewLine +Environment.NewLine+exception.Message)
		{
			Modal = true;						
		}		
		
		protected override void OnResponse(ResponseType responseType)
		{
			fResult = responseType;
		}
		
		private ResponseType fResult;
		public ResponseType Result
		{
			get
			{
				return fResult;
			}
			
			set
			{
				fResult = value;
			}
		}
		
		public int ShowDialog()
		{
			Show();
			
			int result = 0;
			for (
			; true; 
			) 
			{
				result = Run();
				if ((result != ((int)(Gtk.ResponseType.None))))
				{
					break;
				}
			}
			fResult = (Gtk.ResponseType)result; 
			return result;
		}		
	}
}