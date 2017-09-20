//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014

using System;
using Gdk;
using Pango; 

namespace GanttMonoTracker
{
	//TODO: the pallete is specific for a theme, missed interface here
	public static class GdkPalette
	{
		public static Gdk.GC CurrentGC { get;set; }

		public static Gdk.GC InitColor(Drawable area, ColorEnum value)
		{
			DestroyColor();
			//TODO: use dictionary instead
			switch(value)
			{
			case ColorEnum.Axis:
				return CurrentGC = new Gdk.GC(area) { Foreground = StringToColor ("red:ff/0/0")};
			case ColorEnum.ActorLabel:
				return CurrentGC = new Gdk.GC(area) { Foreground = StringToColor ("green:0/ff/0")};
			case ColorEnum.DateLabel:
				return CurrentGC = new Gdk.GC(area) { Foreground = StringToColor ("blue:0/0/ff")};
			case ColorEnum.TaskLabel:
				return CurrentGC = new Gdk.GC(area) { Foreground = StringToColor ("unknown:0/ff/ff")};
			case ColorEnum.DateNow:
				return CurrentGC = new Gdk.GC(area) { Foreground = StringToColor ("white:ff/ff/ff")};
			default:
				return CurrentGC = new Gdk.GC(area) { Foreground = StringToColor ("red:ff/0/0")};
			}
		}

		public static void DestroyColor()
		{
			if(CurrentGC != null)
			{
				CurrentGC.Dispose();
				CurrentGC = null;
			}
		}

		/// <summary>
		/// Get color by rgb abriviature.
		/// </summary>
		/// <returns>color.</returns>
		/// <param name="colorStr">Color string for example.white:0xFF/0xFF/0xFF</param>
		/// authored in md.
		/// UserTasksView.cs
		///
		/// Author:
		/// David Makovský <yakeen@sannyas-on.net>
		static Gdk.Color StringToColor (string colorStr)
		{
			string[] rgb = colorStr.Substring (colorStr.IndexOf (':') + 1).Split ('/');
			if (rgb.Length != 3) return new Gdk.Color (0, 0, 0);
			Gdk.Color color = Gdk.Color.Zero;
			try
			{
				color.Red = UInt16.Parse (rgb[0], System.Globalization.NumberStyles.HexNumber);
				color.Green = UInt16.Parse (rgb[1], System.Globalization.NumberStyles.HexNumber);
				color.Blue = UInt16.Parse (rgb[2], System.Globalization.NumberStyles.HexNumber);
			}
			catch
			{
				// something went wrong, then use neutral black color
				color = new Gdk.Color (0, 0, 0);
			}

			return color;
		}
	}
}
