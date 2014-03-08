// created on 21.12.2005 at 23:57
using System;

namespace TaskManagerInterface
{	
	public interface IGuiMessageDialog : IDisposable
	{
		int ShowDialog();
		
		string Title
		{
			get;
			set;
		}		
	}
}