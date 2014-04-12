//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// Auto-generated by Glade# Code Generator
// http://eric.extremeboredom.net/projects/gladesharpcodegenerator/
using System;
using System.IO;
using System.Reflection;
using Glade;
using Gtk;
using TaskManagerInterface;

namespace GanttMonoTracker.GuiPresentation
{	
	public class ViewActorDialog : IGuiActorView
	{
		
		private Gtk.Dialog thisDialog;

		[Widget]
		private Gtk.Entry entName;

		[Widget]
		private Gtk.Entry entEmail;

		private string fActorName;

		private string fActorEmail;

		public ViewActorDialog(Window parent)
		{
		    Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewActorDialog.glade");
			Glade.XML glade = Glade.XML.FromAssembly("ViewActorDialog.glade","ViewActorDialog", null);
			stream.Close();
			glade.Autoconnect(this);
			entName.Changed += OnChangeName;
			entEmail.Changed += OnChangeEmail;
			thisDialog = ((Gtk.Dialog)(glade.GetWidget("ViewActorDialog")));
			thisDialog.Modal = true;
			thisDialog.TransientFor = parent;
		}	
		
		public int Run()
		{
			thisDialog.Show();
			
			int result = 0;
			for (
			; true; 
			) 
			{
				result = thisDialog.Run();
				if ((result != ((int)(Gtk.ResponseType.None))))
				{
					break;
				}
			}
			thisDialog.Destroy();
			return result;
		}
		
		#region IGuiMessageDialog Implementation
		
		public int ShowDialog()
		{
			return Run();
		}
		
		public string Title
		{
			get
			{
				return thisDialog.Title;
			}
			set
			{
				thisDialog.Title = value;
			}			
		}
		
		#endregion
		
		#region IDisposable Implementation
		
		public void Dispose()
		{
			entName.Changed -= OnChangeName;
			entEmail.Changed -= OnChangeEmail;
			this.thisDialog.Dispose();
		}
		
		#endregion
		
		#region IActorView Implementation
		
		public string ActorName
		{
			get
			{
				return fActorName;
			}
			set
			{
				fActorName = value;
				entName.Text = value;
			}
		}
		
		public string ActorEmail
		{
			get
			{
				return fActorEmail;
			}
			set
			{
				fActorEmail = value;
				entEmail.Text = value;
			}
		}
		
		#endregion

		#region Private Memebrs

		private void OnChangeName(object sender, EventArgs e)
		{
			ActorName = (sender as Entry).Text;
		}



		private void OnChangeEmail(object sender, EventArgs e)
		{
			ActorEmail = (sender as Entry).Text;
		}

		#endregion
	}
}
