//----------------------------------------------------------------------------------------------
// <copyright file="AssigmentDiagram.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:pirogov.e@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 04.02.2006 at 18:33


using System.Reflection;
using System.Windows.Forms;

namespace GanttMonoTracker
{
    public class BrowserForm : Form
	{
		private WebBrowser browser;
		public BrowserForm()
        {
            var p = new Panel
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(p);
            browser = new WebBrowser
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Fill
            };
            p.Controls.Add(browser);
			p.SizeChanged += (sender, e) => { browser.Refresh(); };
			Load += (sender, e) => 
			{
				var stream = Assembly.GetEntryAssembly().GetManifestResourceStream("GanttMonoTracker.Resources.GanttExample.html");
				browser.DocumentStream = stream;
			};
		}
	}
}
