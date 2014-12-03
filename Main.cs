//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// project created on 08.11.2005 at 22:11
using System;
using System.Linq;
using Gtk;
using GLib;
using Glade;

using GanttMonoTracker;
using GanttMonoTracker.GuiPresentation;
using GanttMonoTracker.ExceptionPresentation;
using TaskManagerInterface;
using System.Configuration;

namespace GanttTracker
{
	public class GanttTrackerApp
	{
		MainForm mainForm;


		public static void Main (string[] args)
		{
			new GanttTrackerApp (args);
		}


		public GanttTrackerApp (string[] args) 
		{	
			AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>  {
				ShowError(e.ExceptionObject as Exception);
			};

			try
			{	
				Application.Init ();
				UnhandledExceptionHandler h = new UnhandledExceptionHandler(OnError);
				ExceptionManager.UnhandledException += h;
				mainForm = new MainForm();
				bool autoopen;
				if(bool.TryParse( ConfigurationManager.AppSettings["autoopen"], out autoopen) && autoopen)
				{
					mainForm.OnRecentProject(this, EventArgs.Empty);
				}
				Application.Run();
			}
			catch(Exception ex)
			{
				ShowError(ex);
			}
		
		}


		void OnError(UnhandledExceptionEventArgs args)
		{
			var exception = args.ExceptionObject as Exception;
			ShowError(exception ?? new Exception("Unknown error"));
		}

		void ShowError(Exception ex)
		{
			var silentexceptions = ConfigurationManager.AppSettings["silentexceptions"];
			if(silentexceptions != null && ex != null && silentexceptions.Split(';').Any(s => ex.ToString().IndexOf(s) >= 0 ) )
			{
				return;
			}

			Console.WriteLine("--------------------------Appication Exception-----------------");
			if(ex == null)
			{
				 Console.WriteLine("unknown");
				return;
			}
			Console.WriteLine(ex.ToString());

			IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex,mainForm);
			dialog.ShowDialog();
		}
	}
}