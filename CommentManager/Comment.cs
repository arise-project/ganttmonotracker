// created on 08.02.2006 at 18:46

using System;
using System.Collections;

using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;

namespace GanttTracker.CommentManager
{
	public class Comment : ICommentView, IManagerEntity
	{	
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
		
		private void Initialize(ITaskManager parent,IManagerEntity commentedEntity)
		{
			fParent = parent;			
			if (commentedEntity != null)
			{
				fCommentedEntity = commentedEntity;
				CommentedEntryID = fCommentedEntity.ID;
			}			
		}
		
		private IManagerEntity fCommentedEntity;
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
					CommentedEntryID =fCommentedEntity.ID;				
				else 
					CommentedEntryPresent = false;
			}
		}
		
		#region ICommentView Implementation	
	
		private bool fCommentedEntryPresent;
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
		
		private int fCommentedEntryID;
		public int CommentedEntryID
		{
			get
			{
				if (!CommentedEntryPresent)
					throw new NotAllowedException("Commented Entry Not Present");
				return  fCommentedEntryID;
			}
			
			set
			{
				CommentedEntryPresent = true;
				fCommentedEntryID = value;
			}
		}
		
		private string fDescription;
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
		
		private DateTime fDate;
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
		
		#endregion
		
		#region IManagerEntity Implementation
		
		public int fID;
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
		
		private bool fNew;
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
		
		private ITaskManager fParent;
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
				
		public void Save()
		{
			fNew = false;
			fParent.UpdateComment(this);
		}
		
		public void Delete()
		{
			fParent.DeleteComment(ID);
		}
		
		#endregion

	}
}