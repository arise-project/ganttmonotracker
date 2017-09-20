//----------------------------------------------------------------------------------------------
// <copyright file="Main.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// project created on 08.11.2005 at 22:11

namespace GanttTracker
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using GanttMonoTracker.ExceptionPresentation;
    using GanttMonoTracker.GuiPresentation;

    using GLib;

    using Gtk;

    using TaskManagerInterface;

    using Process = System.Diagnostics.Process;

    public class GanttTrackerApp
    {
        static MainForm mainForm;

        public static void Init(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
            {
                ShowError(e.ExceptionObject as Exception);
            };

            //check single open
            bool single;
            if (bool.TryParse(ConfigurationManager.AppSettings["single"], out single) && single)
            {
                var procName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
                var procList = Process.GetProcessesByName(procName);
                if (procList.Length > 1)
                {
                    return;
                }
            }

            try
            {
                Application.Init();
                UnhandledExceptionHandler h = new UnhandledExceptionHandler(OnError);
                ExceptionManager.UnhandledException += h;
                mainForm = new MainForm();
                bool autoopen;
                if (bool.TryParse(ConfigurationManager.AppSettings["autoopen"], out autoopen) && autoopen)
                {
                    mainForm.OnRecentProject(mainForm, EventArgs.Empty);
                }

                Application.Run();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        public static void Main(string[] args)
        {
            Init(args);
        }

        static void OnError(UnhandledExceptionEventArgs args)
        {
            var exception = args.ExceptionObject as Exception;
            ShowError(exception ?? new Exception("Unknown error"));
        }

        static void ShowError(Exception ex)
        {
            var silentexceptions = ConfigurationManager.AppSettings["silentexceptions"];
            if (silentexceptions != null && ex != null && silentexceptions.Split(';').Any(s => ex.ToString().IndexOf(s) >= 0))
            {
                return;
            }

            Console.WriteLine("--------------------------Appication Exception-----------------");
            if (ex == null)
            {
                Console.WriteLine("unknown");
                return;
            }

            Console.WriteLine(ex.ToString());

            IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex, mainForm);
            dialog.ShowDialog();
        }
    }
}