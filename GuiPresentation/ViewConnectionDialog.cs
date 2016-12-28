﻿//----------------------------------------------------------------------------------------------
// <copyright file="ViewConnectionDialog.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// Auto-generated by Glade# Code Generator
// http://eric.extremeboredom.net/projects/gladesharpcodegenerator/

namespace GanttMonoTracker.GuiPresentation
{
    using System;
    using System.Data;
    using System.IO;

    using GanttTracker.TaskManager.ManagerException;

    using Gtk;

    using TaskManagerInterface;

    public class ViewConnectionDialog : IGuiConnection
    {
        [Glade.Widget]
        Button btnCancel;
        [Glade.Widget]
        Button btnOk;
        [Glade.Widget]
        ComboBoxEntry cbStateIn;
        [Glade.Widget]
        ComboBoxEntry cbStateOut;
        [Glade.Widget]
        Entry entName;
        int fStateInID = -1;
        ListStore fStateInStore;
        int fStateOutID = -1;
        ListStore fStateOutStore;
        [Glade.Widget]
        HBox hbox1;
        [Glade.Widget]
        HBox hbox2;
        [Glade.Widget]
        HBox hbox3;
        [Glade.Widget]
        HButtonBox hbuttonbox2;
        [Glade.Widget]
        Label lbConnectionAction;
        [Glade.Widget]
        Label lbNameDescription;
        [Glade.Widget]
        Label lbStateInDescription;
        [Glade.Widget]
        Label lbStateOutDescription;
        Dialog thisDialog;
        [Glade.Widget]
        VBox vbox1;
        [Glade.Widget]
        VBox vbox2;

        public ViewConnectionDialog(Window parent, DataSet taskStateSource)
        {
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewConnectionDialog.glade");
            Glade.XML glade = new Glade.XML(stream, "ViewConnectionDialog", null);
            stream.Close();
            glade.Autoconnect(this);
            thisDialog = ((Dialog)(glade.GetWidget("ViewConnectionDialog")));
            thisDialog.TransientFor = parent;
            thisDialog.Modal = true;
            thisDialog.WindowPosition = WindowPosition.Center;
            TaskStateSource = taskStateSource;

            cbStateIn.Sensitive = false;
            lbConnectionAction.Text = "Create Connection";
            cbStateOut.Entry.IsEditable = false;
            cbStateIn.Changed += new EventHandler(OnCbStateInChanged);
            cbStateOut.Changed += new EventHandler(OnCbStateOutChanged);
        }

        public int MappingID
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                return entName.Text;
            }

            set
            {
                entName.Text = value;
            }
        }

        public int StateID
        {
            get;
            set;
        }

        public DataSet TaskStateSource
        {
            get;
            set;
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

        public void BindStateIn()
        {
            if (TaskStateSource != null)
            {
                fStateInStore = new ListStore(typeof(int),typeof(string));
                cbStateIn.Clear();
                foreach (DataRow row in TaskStateSource.Tables["TaskState"].Rows)
                {
                    fStateInStore.AppendValues((int)row["ID"],(string)row["Name"]);
                }

                cbStateIn.Model = fStateInStore;
                CellRendererText stateText = new CellRendererText();
                stateText.Style = Pango.Style.Oblique;
                stateText.ForegroundGdk = new Gdk.Color(0x63,0,0);
                cbStateIn.PackStart(stateText,true);
                cbStateIn.AddAttribute(stateText,"text",1);
                TreeIter iter;
                if (fStateInStore.GetIterFirst(out iter))
                {
                    cbStateIn.SetActiveIter(iter);
                }
            }
            else
            {
                throw new ManagementException(ExceptionType.NotAllowed,"TaskStateSource not set to instance");
            }
        }

        public void BindStateOut()
        {
            if (TaskStateSource != null)
            {
                fStateOutStore = new ListStore(typeof(int),typeof(string));
                cbStateOut.Clear();
                foreach (DataRow row in TaskStateSource.Tables["TaskState"].Rows)
                {
                    fStateOutStore.AppendValues((int)row["ID"],(string)row["Name"]);
                }

                cbStateOut.Model = fStateOutStore;
                CellRendererText stateText = new CellRendererText();
                stateText.Style = Pango.Style.Oblique;
                cbStateOut.PackStart(stateText,true);
                cbStateOut.AddAttribute(stateText,"text",1);
                TreeIter iter;
                if (fStateOutStore.GetIterFirst(out iter))
                {
                    cbStateOut.SetActiveIter(iter);
                }
            }
            else
            {
                throw new ManagementException(ExceptionType.NotAllowed,"TaskStateSource not set to instance");
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
            for (
                ; true;
            )
            {
                result = thisDialog.Run();
                if ((result != ((int)(ResponseType.None))))
                {
                    break;
                }
            }

            thisDialog.Destroy();
            return result;
        }

        public int ShowDialog()
        {
            return Run();
        }

        private void OnCbStateInChanged(object sender, EventArgs args)
        {
            if (cbStateIn.Active != -1)
            {
                fStateInID = (int)TaskStateSource.Tables["TaskState"].Rows[cbStateIn.Active]["ID"];
                cbStateIn.Entry.Text = (string)TaskStateSource.Tables["TaskState"].Rows[cbStateIn.Active]["Name"];
            }
        }

        private void OnCbStateOutChanged(object sender, EventArgs args)
        {
            if (cbStateOut.Active != -1)
            {
                fStateOutID = (int)TaskStateSource.Tables["TaskState"].Rows[cbStateOut.Active]["ID"];
                cbStateOut.Entry.Text = (string)TaskStateSource.Tables["TaskState"].Rows[cbStateOut.Active]["Name"];
            }
        }
    }
}