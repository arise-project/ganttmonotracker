//----------------------------------------------------------------------------------------------
// <copyright file="AboutDialog.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 10.02.2006 at 18:13

namespace GanttMonoTracker.GuiPresentation
{
    using System;
    using System.IO;
    using System.Threading;

    using Gtk;

    using TaskManagerInterface;

    public class AboutDialog : IGuiMessageDialog, IDisposable
    {
        private Gtk.Dialog thisDialog;

        public AboutDialog(Window parent)
        {
            Initialize(parent);
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

        public void Dispose()
        {
            this.thisDialog.Dispose();
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

        public int ShowDialog()
        {
            return Run();
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
    }
}