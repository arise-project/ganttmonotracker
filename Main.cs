// project created on 08.11.2005 at 22:11
using System;
using Gtk;
using GLib;
using Glade;
using GanttMonoTracker;
using GanttMonoTracker.GuiPresentation;
using GanttMonoTracker.ExceptionPresentation;
using TaskManagerInterface;

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
			Console.WriteLine("--------------------------Appication Exception-----------------");
			if(ex == null)
			{
				 Console.WriteLine("unknown");
				return;
			}
			Console.WriteLine("Type : "+ex.GetType().Name );
			Console.WriteLine("Message : ");
			Console.WriteLine(ex.Message);
			Console.WriteLine("Source : ");
			Console.WriteLine(ex.Source);
			if (ex.InnerException != null)
			{
				Console.WriteLine("InnerException Message : ");
				Console.WriteLine(ex.InnerException.Message);
			}
			Console.WriteLine("Help Link : ");
			Console.WriteLine(ex.HelpLink);
			Console.WriteLine("Stack Trace : ");
			Console.WriteLine(ex.StackTrace);

			IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex,mainForm);
			dialog.ShowDialog();
		}

	}
}