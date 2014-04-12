//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
using System;
using System.IO;
using System.Reflection;

namespace GanttMonoTracker
{
	public static class FSLocations
	{
		public static string GetPath (string filename)
		{
			return Path.Combine(Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location), filename);
		}
	}
}

