using System.Data;

namespace GanttMonoTracker
{
	public interface IGanttSource
	{
		bool DateNowVisible
		{
			get;
			set;
		}

		bool ReadOnly
		{
			get;
			set;
		}

		DataSet Source
		{
			get;
			set;
		}

		DataSet StaticSource
		{
			get;
			set;
		}

		void Refresh();
	}
}
