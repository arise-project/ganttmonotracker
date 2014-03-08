// created on 15.01.2006 at 1:25

using System.Data;

namespace TaskManagerInterface
{
	public interface IGuiGantt
	{
		DataSet GanttSource
		{
			get;
			set;
		}
		
		void BindGantt();
	}
}