//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 10.02.2006 at 12:05

using System;

namespace TaskManagerInterface
{
	public interface ICommentView
	{
	
		bool CommentedEntryPresent { get; set; }
		
		int CommentedEntryID { get; set; }
		
		string Description { get; set; }
		
		DateTime Date {	get; set; }	
	}
}
