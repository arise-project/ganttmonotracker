//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 10.02.2006 at 18:13
using System;
using System.Threading;
using System.Data;
using System.IO;
using Gtk;
using GanttTracker.StateManager;
using GanttMonoTracker.ExceptionPresentation;
using TaskManagerInterface;

namespace GanttMonoTracker.GuiPresentation
{	
	public class AboutDialog : IGuiMessageDialog,IDisposable
	{
		private Gtk.Dialog thisDialog;
	
			
		public AboutDialog(Window parent)
		{
		    Initialize(parent);
		}


		private void Initialize(Window parent)
		{
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GMTAboutDialog.glade");
			Glade.XML glade = new Glade.XML(stream, "GMTAboutDialog", null);
			stream.Close();
			glade.Autoconnect(this);
			thisDialog = ((Gtk.Dialog)(glade.GetWidget("GMTAboutDialog")));
			thisDialog.Modal = true;
			thisDialog.TransientFor = parent;
			thisDialog.WindowPosition = WindowPosition.Center;
		}

		
		public int Run()
		{
			thisDialog.ShowAll();
			
			int result = 0;
			for (; true; ) 
			{
				result = thisDialog.Run();
				if ((result != ((int)(Gtk.ResponseType.None))) && result != ((int)(Gtk.ResponseType.Apply)))
				{
					break;
				}
				Thread.Sleep(500);
			}
			thisDialog.Destroy();
			return result;
		}
		
		#region IGuiMessageDialog Implementation
		
		public int ShowDialog()
		{
			return Run();
		}


		public string Title
		{
			get
			{
				return thisDialog.Title;
			}
			set
			{
				thisDialog.Title = value;
			}
		}
		
		#endregion
		
		#region IDisposable Implementation
		
		public void Dispose()
		{
			this.thisDialog.Dispose();
		}
		
		#endregion
	}
}