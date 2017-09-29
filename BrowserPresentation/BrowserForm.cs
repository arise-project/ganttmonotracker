using System;
using System.Windows.Forms;

namespace GanttMonoTracker
{
	public class BrowserForm : Form
	{
		private WebBrowser browser;
		public BrowserForm()
		{
			browser = new WebBrowser();
			browser.Dock = DockStyle.Fill;
			Controls.Add(browser);
			Load += (sender, e) => { browser.Navigate("https://linux.org.ru");};

		}
	}
}
