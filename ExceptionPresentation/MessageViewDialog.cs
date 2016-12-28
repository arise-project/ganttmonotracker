﻿//----------------------------------------------------------------------------------------------
// <copyright file="MessageViewDialog.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// Auto-generated by Glade# Code Generator
// http://eric.extremeboredom.net/projects/gladesharpcodegenerator/

namespace GanttMonoTracker.ExceptionPresentation
{
    using System;
    using System.IO;
    using System.Threading;

    using Gtk;

    using TaskManagerInterface;

    public class MessageViewDialog : IGuiMessageDialog
    {
        [Glade.Widget]
        private Button btnOk;
        [Glade.Widget]
        private HButtonBox hbuttonbox1;
        [Glade.Widget]
        private Label label1;
        [Glade.Widget]
        private Label lbSubject;
        [Glade.Widget]
        private Label lbWarning;
        [Glade.Widget]
        private Label lbWarningDescription;
        private Dialog thisDialog;
        [Glade.Widget]
        private TextView tvDescription;

        public MessageViewDialog(string type, string subject, string description, Window parent)
        {
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WarningView.glade");
            Glade.XML glade = new Glade.XML(stream, "dialog1", null);
            stream.Close();
            glade.Autoconnect(this);
            thisDialog = ((Dialog)(glade.GetWidget("dialog1")));
            thisDialog.TransientFor = parent;
            thisDialog.Modal = true;

            this.WarningType = type;
            this.WarningSubject = subject;
            this.WarningDescription = description;
        }

        public MessageViewDialog(string subject,Window parent)
        {
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WarningView.glade");
            Glade.XML glade = new Glade.XML(stream, "dialog1", null);
            stream.Close();
            glade.Autoconnect(this);
            thisDialog = ((Dialog)(glade.GetWidget("dialog1")));
            thisDialog.TransientFor = parent;
            thisDialog.Modal = true;

            WarningSubject = subject;
            lbWarningDescription.Text = "";
            lbWarning.Text = "";
            tvDescription.Sensitive = false;
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

        public string WarningDescription
        {
            get
            {
                return tvDescription.Buffer.Text;
            }

            set
            {
                tvDescription.Buffer.Text = value;
            }
        }

        public string WarningSubject
        {
            get
            {
                return lbSubject.Text;
            }

            set
            {
                lbSubject.Text = value;
            }
        }

        public string WarningType
        {
            get
            {
                return lbWarning.Text;
            }

            set
            {
                lbWarning.Text = value;
            }
        }

        public void Dispose()
        {
            thisDialog.Dispose();
        }

        public int Run()
        {
            thisDialog.ShowAll();

            int result = 0;
            for (; true; )
            {
                result = thisDialog.Run();
                if ((result != ((int)(ResponseType.None))))
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
    }
}