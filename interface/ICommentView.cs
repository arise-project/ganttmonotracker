// created on 10.02.2006 at 12:05

using System;

namespace TaskManagerInterface
{
	public interface ICommentView
	{
	
		bool CommentedEntryPresent
		{
			get;
			set;
		}
		
		int CommentedEntryID
		{
			get;
			set;
		}
		
		string Description
		{
			get;
			set;
		}
		
		DateTime Date
		{
			get;
			set;
		}	
	}
}
