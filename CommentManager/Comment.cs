//----------------------------------------------------------------------------------------------
// <copyright file="Comment.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
// created on 08.02.2006 at 18:46

namespace GanttTracker.CommentManager
{
    using System;
    using System.Collections;

    using GanttTracker.TaskManager.ManagerException;

    using TaskManagerInterface;

    public class Comment : ICommentView, IManagerEntity
    {
        public int fID;

        private IManagerEntity fCommentedEntity;
        private int fCommentedEntryID;
        private bool fCommentedEntryPresent;
        private DateTime fDate;
        private string fDescription;
        private bool fNew;
        private ITaskManager fParent;

        public Comment()
        {
            fNew = true;
            Initialize(null,null);
        }

        public Comment(ITaskManager parent)
        {
            fNew = true;
            Initialize(parent,null);
        }

        public Comment(ITaskManager parent, IManagerEntity commentedEntity)
        {
            fNew = true;
            Initialize(parent,commentedEntity);
        }

        public Comment(ITaskManager parent, int ID)
        {
            fNew = false;
            Initialize(parent,null);
            fID = ID;
        }

        public IManagerEntity CommentedEntity
        {
            get
            {
                return fCommentedEntity;
            }

            set
            {
                fCommentedEntity = value;
                if (fCommentedEntity != null)
                {
                    CommentedEntryID =fCommentedEntity.ID;
                }
                else
                {
                    CommentedEntryPresent = false;
                }
            }
        }

        public int CommentedEntryID
        {
            get
            {
                if (!CommentedEntryPresent)
                {
                    throw new NotAllowedException("Commented Entry Not Present");
                }
                return  fCommentedEntryID;
            }

            set
            {
                CommentedEntryPresent = true;
                fCommentedEntryID = value;
            }
        }

        public bool CommentedEntryPresent
        {
            get
            {
                return fCommentedEntryPresent;
            }

            set
            {
                fCommentedEntryPresent = value;
            }
        }

        public DateTime Date
        {
            get
            {
                return fDate;
            }

            set
            {
                fDate = value;
            }
        }

        public string Description
        {
            get
            {
                return fDescription;
            }

            set
            {
                fDescription = value;
            }
        }

        public int ID
        {
            get
            {
                return fID;
            }

            set
            {
                throw new NotAllowedException("Can not set ID for managed entity");
            }
        }

        public bool isNew
        {
            get
            {
                return fNew;
            }

            set
            {
                throw new NotAllowedException("Change state for managed entity not allowed");
            }
        }

        public bool isUpdated
        {
            get
            {
                return fParent.isUpdatedComment(this);
            }

            set
            {
                throw new NotAllowedException("Change state for managed entity not allowed");
            }
        }

        public ITaskManager Parent
        {
            get
            {
                return fParent;
            }

            set
            {
                fParent = value;
            }
        }

        public void BindData()
        {
            fParent.BindComment(this);
        }

        public void Delete()
        {
            fParent.DeleteComment(ID);
        }

        public void Save()
        {
            fNew = false;
            fParent.UpdateComment(this);
        }

        private void Initialize(ITaskManager parent,IManagerEntity commentedEntity)
        {
            fParent = parent;
            if (commentedEntity != null)
            {
                fCommentedEntity = commentedEntity;
                CommentedEntryID = fCommentedEntity.ID;
            }
        }
    }
}