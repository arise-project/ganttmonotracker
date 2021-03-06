﻿//----------------------------------------------------------------------------------------------
// <copyright file="ViewTaskDialog.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// Auto-generated by Glade# Code Generator
// http://eric.extremeboredom.net/projects/gladesharpcodegenerator/
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace GanttMonoTracker.GuiPresentation
{
    using System;
    using System.Data;
    using System.IO;
    using System.Threading;

    using GanttTracker.TaskManager.ManagerException;

    using Gtk;

    using TaskManagerInterface;

    public class ViewTaskDialog : IGuiTask, IGuiTracker
    {
		int fActorID = -1;
		ListStore fActorStore;
		string fComment;
		string fDescription;
		int fStateID = -1;
		ListStore fStateStore;

		#pragma warning disable 0649
		#pragma warning disable 0169
        [Glade.Widget]
        Calendar calEndTime;
        [Glade.Widget]
        Calendar calStartTime;
        [Glade.Widget]
        ComboBoxEntry cbActor;
        [Glade.Widget]
        ComboBoxEntry cbState;
        
        [Glade.Widget]
        Label lbCommentDescription;
        [Glade.Widget]
        ScrolledWindow swComment;
        Dialog thisDialog;
        [Glade.Widget]
        TextView tvComment;
        [Glade.Widget]
        TextView tvDescription;

        [Glade.Widget]
        SpinButton spbPriority;
        #pragma warning restore 0169
        #pragma warning restore 0649

        public ViewTaskDialog(Window parent, bool isTaskInit)
        {
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewTaskDialog.glade");
            Glade.XML glade = new Glade.XML(stream, "ViewTaskDialog", null);
            stream.Close();
            glade.Autoconnect(this);
            thisDialog = ((Dialog)(glade.GetWidget("ViewTaskDialog")));
            thisDialog.Modal = true;
            thisDialog.TransientFor = parent;
            thisDialog.WindowPosition = WindowPosition.CenterAlways;

            cbActor.Entry.IsEditable = false;
            cbActor.Changed += new EventHandler(OnCbActorChanged);
            calStartTime.Date = DateTime.Now.Date;
            calEndTime.Date = DateTime.Now.Date;
            cbState.Entry.IsEditable = false;
            cbState.Changed += new EventHandler(OnCbStateChanged);
            spbPriority.SetRange(0, 10000);
            spbPriority.SetIncrements(1, 10);

            IsInitTask = isTaskInit;
            tvDescription.KeyReleaseEvent += HandleKeyReleaseEvent;
            tvComment.KeyReleaseEvent += CommentKeyReleaseEvent;
        }

        public int ActorID
        {
            get
            {
                if (!ActorPresent)
                {
                    throw new ManagementException(ExceptionType.NotAllowed,"Actor not present");
                }

                return fActorID;
            }

            set
            {
                ActorPresent = true;
                if (ActorSource == null)
                {
                    throw new ManagementException(ExceptionType.NotAllowed,"Bind combo before with BindActor method");
                }

                int index = 0;
                foreach(DataRow row in ActorSource.Tables["Actor"].Rows)
                {
                    if ((int)row["ID"] == 	value)
                    {
                        cbActor.Active = index;
                        fActorID = value;
                        return;
                    }

                    index++;
                }

                throw new ManagementException(ExceptionType.NotAllowed,"ActorID not found in Actor Source");
            }
        }

        public bool ActorPresent
        {
            get;
            set;
        }

        public DataSet ActorSource
        {
            get;
            set;
        }

        public string Comment
        {
            get
            {
                return tvComment.Buffer.Text;
            }

            set
            {
                tvComment.Buffer.Text = value;
            }
        }

        public string Description
        {
            get
            {
				return string.IsNullOrWhiteSpace (fComment) ? fDescription : fDescription + Environment.NewLine + fComment;
            }

            set
            {
				fDescription = TrimWords (
					string.IsNullOrWhiteSpace (fComment) ? value : value + Environment.NewLine + fComment, 
					int.Parse (ConfigurationManager.AppSettings ["detailsWidth"]));
				
				tvDescription.Buffer.Text = fDescription;
            }
        }

		private string TrimWords(string sentence, int partLength)
		{
			string[] words = sentence.Split(' ');
			var b = new StringBuilder ();
			string part = string.Empty;
			foreach (var word in words)
			{
				if (part.Length + word.Length < partLength)
				{
					part += string.IsNullOrEmpty(part) ? word : " " + word;
				}
				else
				{
					b.AppendLine (part);
					part = word;
				}
			}
			b.AppendLine(part);
			return b.ToString ();
		}

        public DateTime EndTime
        {
            get
            {
                DateTime endTime = calEndTime.Date.Date;
                return endTime;
            }

            set
            {
                calEndTime.Date = value;
            }
        }

        public bool IsInitTask
        {
            get;
            private set;
        }

        public DateTime StartTime
        {
            get
            {
                DateTime startTime = calStartTime.Date.Date;
                return startTime;
            }

            set
            {
                calStartTime.Date = value;
            }
        }

        public int StateID
        {
            get
            {
                return fStateID;
            }

            set
            {
                if (StateSource == null)
                {
                    throw new ManagementException(ExceptionType.NotAllowed,"Bind combo before with BindState method");
                }

                int index = 0;
                foreach(DataRow row in StateSource.Tables["TaskState"].Rows)
                {
                    if ((int)row["ID"] == 	value)
                    {
                        cbState.Active = index;
                        fStateID = value;
                        return;
                    }

                    index++;
                }

                throw new ManagementException(ExceptionType.NotAllowed,"StateID not found in State Source");
            }
        }

        public DataSet StateSource
        {
            get;
            set;
        }

        public DataSet TaskSource
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

        public int Priority
        {
            get
            {
                return (int)spbPriority.Value;
            }

            set
            {
                spbPriority.Value = value;
            }
        }

        public void BindActor()
        {
            fActorStore = new ListStore(typeof(int),typeof(string));
            cbActor.Clear();
            foreach (DataRow row in ActorSource.Tables["Actor"].Rows)
            {
                fActorStore.AppendValues((int)row["ID"],(string)row["Name"]);
            }

            cbActor.Model = fActorStore;
            CellRendererText actorText = new CellRendererText();
            actorText.Style = Pango.Style.Oblique;

            //actorText.BackgroundGdk = new Gdk.Color(0x63,0,0);
            cbActor.PackStart(actorText,true);
            cbActor.AddAttribute(actorText,"text",1);
            TreeIter iter;
            if (fActorStore.GetIterFirst(out iter))
            {
                cbActor.SetActiveIter(iter);
                fActorID = (int)ActorSource.Tables["Actor"].Rows[0]["ID"];
            }
        }

        public void BindState()
        {
            fStateStore = new ListStore(typeof(int),typeof(string));
            cbState.Clear();
            foreach (DataRow row in StateSource.Tables["TaskState"].Rows)
            {
                fStateStore.AppendValues((int)row["ID"],(string)row["Name"]);
            }

            cbState.Model = fStateStore;
            CellRendererText stateText = new CellRendererText();
            cbState.PackStart(stateText,true);
            cbState.AddAttribute(stateText,"text",1);
            TreeIter iter;
            if (fStateStore.GetIterFirst(out iter))
            {
                cbState.SetActiveIter(iter);
                fStateID = (int)StateSource.Tables["TaskState"].Rows[0]["ID"];
            }
        }

        public void BindTask()
        {
        }

        public void Dispose()
        {
            thisDialog.Dispose();
        }

        public int Run()
        {
            thisDialog.Show();

            if (IsInitTask)
            {
                tvComment.Visible = false;
                fStateID = (int)StateSource.Tables["TaskState"].Rows[0]["ID"];
                lbCommentDescription.Visible = false;
            }
            else
            {
                tvDescription.Editable = false;
            }

            int result = 0;
            for (; true;)
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

        void CommentKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            fComment = tvComment.Buffer.Text;
        }

        void HandleKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            fDescription = tvDescription.Buffer.Text;
        }

        void OnCbActorChanged(object sender, EventArgs args)
        {
            if (cbActor.Active != -1)
            {
                ActorID = (int)ActorSource.Tables["Actor"].Rows[cbActor.Active]["ID"];
                cbActor.Entry.Text = (string)ActorSource.Tables["Actor"].Rows[cbActor.Active]["Name"];
            }
        }

        void OnCbStateChanged(object sender, EventArgs args)
        {
            if (cbState.Active != -1)
            {
                StateID = (int)StateSource.Tables["TaskState"].Rows[cbState.Active]["ID"];
                cbState.Entry.Text = (string)StateSource.Tables["TaskState"].Rows[cbState.Active]["Name"];
            }
        }

        private void SetComment()
        {
        }
    }
}