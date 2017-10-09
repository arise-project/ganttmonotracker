using System;
using System.Reflection;
using System.Windows.Forms;

namespace GanttMonoTracker
{
	public class BrowserForm : Form
	{
		private WebBrowser browser;
		public BrowserForm()
		{
			var p = new Panel();
			p.Dock = DockStyle.Fill;
			Controls.Add(p);
			browser = new WebBrowser();
			browser.Anchor = AnchorStyles.Top;
			browser.Dock = DockStyle.Fill;
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
