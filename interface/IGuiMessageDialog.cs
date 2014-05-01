//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.12.2005 at 23:57
using System;

namespace TaskManagerInterface
{	
	public interface IGuiMessageDialog : IDisposable
	{
		int ShowDialog();


		string Title { get;set;	}
	}
}