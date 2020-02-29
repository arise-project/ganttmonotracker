//----------------------------------------------------------------------------------------------
// <copyright file="IGuiCore.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 22.01.2006 at 1:50
namespace TaskManagerInterface
{
    public enum CoreState
    {
        EmptyProject, CreateProject, OpenProject
    }

    public interface IGuiCore
    {
        CoreState State
        {
            get;
            set;
        }

        IStateManager StateManager
        {
            get;
            set;
        }

        IStorageManager StorageManager
        {
            get;
            set;
        }

        ITaskManager TaskManager
        {
            get;
            set;
        }
    }
}