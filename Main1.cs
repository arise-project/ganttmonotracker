//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014

namespace GanttMonoTracker
{
    using System;

    using Gtk;

    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init ();
            MainWindow win = new MainWindow ();
            win.Show ();
            Application.Run ();
        }
    }
}