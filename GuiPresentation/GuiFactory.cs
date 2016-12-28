//----------------------------------------------------------------------------------------------
// <copyright file="GuiFactory.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 18.12.2005 at 22:41

namespace GanttMonoTracker.GuiPresentation
{
    using System;
    using System.Data;

    using GanttTracker.StateManager;
    using GanttTracker.TaskManager;
    using GanttTracker.TaskManager.ManagerException;

    using Gtk;

    using TaskManagerInterface;

    public static class GuiFactory
    {
        public static AboutDialog CreateAboutDialog(Window parent)
        {
            AboutDialog aboutDialog = new AboutDialog(parent);
            return aboutDialog;
        }

        public static IGuiActorView CreateActorView(Window parent)
        {
            ViewActorDialog actorDialog = new ViewActorDialog(parent);
            actorDialog.Title = "Create Actor";
            return actorDialog;
        }

        public static IGuiActorView CreateActorView(Window parent,ITaskManager taskManager, Actor actor)
        {
            ViewActorDialog actorDialog = new ViewActorDialog(parent);
            actorDialog.ActorName = actor.Name;
            actorDialog.ActorEmail = actor.Email;
            actorDialog.Title = "Edit Actor";
            return actorDialog;
        }

        public static ViewConnectionDialog CreateConnectionView(Window parent, DataSet taskStateSource)
        {
            ViewConnectionDialog connectionView = new ViewConnectionDialog(parent,taskStateSource);
            connectionView.Title = "New Connection";
            connectionView.BindStateIn();
            connectionView.BindStateOut();
            return connectionView;
        }

        public static ViewSingleStateDialog CreateSingleStateView(Window parent)
        {
            ViewSingleStateDialog stateView = new ViewSingleStateDialog(parent);
            stateView.Title = "New State";
            return stateView;
        }

        public static ViewSingleStateDialog CreateSingleStateView(Window parent, State state)
        {
            ViewSingleStateDialog stateView = new ViewSingleStateDialog(parent);
            stateView.Title = "Edit State";

            stateView.Name = state.Name;

            stateView.ColorRed = state.ColorRed;
            stateView.ColorGreen = state.ColorGreen;
            stateView.ColorBlue = state.ColorBlue;
            stateView.BindStateColor();

            if (state.IsMapped)
            {
                stateView.MappingID = state.MappingID;
            }
            else
            {
                stateView.IsMapped = false;
            }
            return stateView;
        }

        public static ViewStateDialog CreateStateView(Window parent, IGuiCore guiCore)
        {
            ViewStateDialog stateView = new ViewStateDialog(parent,guiCore);
            stateView.Title = "Edit States";
            return stateView;
        }

        public static ViewTaskAssign CreateTaskAssign(Window parent, IGuiCore core, int taskID)
        {
            ValidateCore(core);
            Task task = (Task)core.TaskManager.GetTask(taskID);
            ViewTaskAssign assignDialog = new ViewTaskAssign(parent);
            if(task == null)
            {
                return assignDialog;
            }
            assignDialog.ActorSource = core.TaskManager.ActorSource;
            assignDialog.BindActor();
            if (task.ActorPresent)
            {
                assignDialog.ActorID = task.ActorID;
            }
            assignDialog.Title = "Assign Task";
            assignDialog.AssignAction = string.Format("Assign task {0} to Actor", task.Id);

            return  assignDialog;
        }

        public static IGuiTask CreateTaskView(Window parent, IGuiCore core)
        {
            ValidateCore(core);

            ViewTaskDialog taskDialog = new ViewTaskDialog(parent, true);
            taskDialog.Title ="Create Task";
            taskDialog.ActorSource = core.TaskManager.ActorSource;
            taskDialog.StateSource = core.TaskManager.GetInitialTaskStateSource();
            if(taskDialog.StateSource == null)
            {
                return taskDialog;
            }
            taskDialog.BindActor();
            taskDialog.BindState();

            return taskDialog;
        }

        public static IGuiTask CreateTaskView(Window parent, IGuiCore core, int taskID)
        {
            ValidateCore(core);

            Task task = (Task)core.TaskManager.GetTask(taskID);
            ViewTaskDialog taskDialog = new ViewTaskDialog(parent, false);
            if(task == null)
            {
                return taskDialog;
            }
            if (task.StatePresent)
            {
                State state = (State)core.TaskManager.GetTaskState(task.StateID);
                if(state == null)
                {
                    return taskDialog;
                }
                taskDialog.StateSource = core.TaskManager.GetTaskStateSource(state);
                if(taskDialog.StateSource == null)
                {
                    return taskDialog;
                }
                taskDialog.BindState();
                taskDialog.StateID = task.StateID;
            }
            else
            {
                taskDialog.StateSource = core.TaskManager.GetInitialTaskStateSource();
                if(taskDialog.StateSource == null)
                {
                    return taskDialog;
                }
                taskDialog.BindState();
            }

            taskDialog.Title ="Edit Task";
            taskDialog.ActorSource = core.TaskManager.ActorSource;

            taskDialog.BindActor();
            if (task.ActorPresent)
            {
                taskDialog.ActorID = task.ActorID;
            }
            taskDialog.Description = task.Description;
            taskDialog.EndTime = task.EndTime;
            taskDialog.StartTime = task.StartTime;
            taskDialog.Comment = task.Comment;
            return taskDialog;
        }

        private static void ValidateCore(IGuiCore core)
        {
            if (core.TaskManager.ActorSource.Tables["Actor"].Rows.Count == 0)
            {
                throw new ManagementException(ExceptionType.NotAllowed,"Create actors before create tasks");
            }

            if (core.TaskManager.TaskStateSource.Tables["TaskState"].Rows.Count == 0)
            {
                throw new ManagementException(ExceptionType.NotAllowed,"Create states before create tasks");
            }
        }
    }
}