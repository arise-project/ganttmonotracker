using System;
using WebKit.Linux;
using Gtk;

namespace WebKit.Linux
{
	public class LinuxWebKitBrowser : WebKitBrowser
	{
		internal GtkReparentingWrapperNoThread linuxwrapper;
		private object syncRoot = new object();
		public LinuxWebKitBrowser()
		{
		}

		public GtkReparentingWrapperNoThread Linuxwrapper
		{ 
			get
			{
				if (linuxwrapper != null) return linuxwrapper;
				lock(syncRoot)
				{
					if (linuxwrapper == null)
					{
						linuxwrapper = new GtkReparentingWrapperNoThread(new Window(WindowType.Popup), this);
                        webView.setHostWindow(linuxwrapper.BrowserWindow.Handle.ToInt32());
					}
				}

				return linuxwrapper;
			}
		}

        public Window GetWindow()
        {
            return Linuxwrapper.BrowserWindow;
        }
		protected override void OnHandleCreated (EventArgs e)
		{
			Linuxwrapper.Init ();
			base.OnHandleCreated (e);
		}
		protected override void OnResize (EventArgs e)
		{
			Linuxwrapper.BrowserWindow.Resize (base.Width, base.Height );
			base.OnResize (e);
		}
        protected override void OnHandleDestroyed(EventArgs e)
        {
            Linuxwrapper.BrowserWindow.Destroy();
            Linuxwrapper.Dispose();
            base.OnHandleDestroyed(e);
        }
		protected override void OnGotFocus (EventArgs e)
		{
			Linuxwrapper.BrowserWindow.GrabFocus();
			base.OnGotFocus (e);
		}
	}
}
