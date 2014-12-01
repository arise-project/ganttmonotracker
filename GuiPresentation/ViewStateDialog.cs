//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// Auto-generated by Glade# Code Generator
// http://eric.extremeboredom.net/projects/gladesharpcodegenerator/

using System;
using System.Threading;
using System.Collections;
using System.Data;
using System.IO;
using Gtk;

using GanttTracker.StateManager;
using GanttTracker.TaskManager.ManagerException;
using GanttMonoTracker.ExceptionPresentation;
using TaskManagerInterface;

using Arise.Logic;

namespace GanttMonoTracker.GuiPresentation
{
	public class ViewStateDialog : IGuiState, IGuiMessageDialog,IDisposable
	{
		ListStore fStateSearchDictionaryStore;
		StateCore stateCore;

		Gtk.Dialog thisDialog;

		
		[Glade.Widget()]
		Gtk.Button btnApply;

		
		[Glade.Widget()]
		Gtk.Entry entStateSearch;

		
		[Glade.Widget()]
		Gtk.Button btnSearch;

		
		[Glade.Widget()]
		Gtk.TreeView tvState;

		
		[Glade.Widget()]
		Gtk.Button btnCreateState;

		
		[Glade.Widget()]
		Gtk.Button btnEditState;

		
		[Glade.Widget()]
		Gtk.Button btnDeleteState;

		
		[Glade.Widget()]
		Gtk.TreeView tvStateConnection;

		
		[Glade.Widget()]
		Gtk.Button btnCreateConnection;

		
		[Glade.Widget()]
		Gtk.Button btnEditConnection;

		
		[Glade.Widget()]
		Gtk.Button btnDeleteConnection;


		public ViewStateDialog(Window parent, IGuiCore guiCore)
		{
		    Initialize(parent, guiCore);
		}

		
		private void Initialize(Window parent, IGuiCore guiCore)
		{
			stateCore = new StateCore(guiCore, this); 
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewStateDialog.glade");
			Glade.XML glade = new Glade.XML(stream, "EditStatesDialog", null);
			stream.Close();
			glade.Autoconnect(this);
			thisDialog = ((Gtk.Dialog)(glade.GetWidget("EditStatesDialog")));
			thisDialog.Modal = true;
			thisDialog.TransientFor = parent;
			thisDialog.SetDefaultSize(800,600);
			
			btnApply.Clicked += new EventHandler(OnApply);
			
			btnSearch.Clicked += new EventHandler(OnStateSearch);
			btnCreateState.Clicked += new EventHandler(OnCreateState);
			btnEditState.Clicked += new EventHandler(OnEditState);
			btnDeleteState.Clicked += new EventHandler(OnDeleteState);
			
			btnCreateConnection.Clicked += new EventHandler(OnCreateConnection);
			btnEditConnection.Clicked += new EventHandler(OnEditConnection);
			btnDeleteConnection.Clicked += new EventHandler(OnDeleteConnection);
			
			TaskStateStore = new TreeStore(typeof(int), typeof(string),typeof(int));
			
			tvState.HeadersVisible = true;
			tvState.AppendColumn("ID",new CellRendererText(), "text", 1);
			tvState.AppendColumn("Name",new CellRendererText(), "text", 1);
			tvState.GetColumn(0).Visible = false;
			
			tvStateConnection.HeadersVisible = true;
			tvStateConnection.AppendColumn("ID",new CellRendererText(), "text", 1);
			tvStateConnection.AppendColumn("Name",new CellRendererText(), "text", 1);
			tvStateConnection.GetColumn(0).Visible = false;
			
			Source = stateCore.TaskManager.TaskStateSource;
			
			BindStateSearchDictionary();
			BindStates();
			BindStateSearchCompletion();
		}

		
		public int Run()
		{
			thisDialog.ShowAll();
			
			int result = 0;
			for (; true;) 
			{
				result = thisDialog.Run();
				if ((result != ((int)(Gtk.ResponseType.None))) && result != ((int)(Gtk.ResponseType.Apply)))
				{
					break;
				}
				Thread.Sleep(500);
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
			this.thisDialog.Dispose();
		}
		
		#endregion
		
		#region IGuiCore Implementation
		
		public CoreState State { get; set; }
		
		#endregion
		
		#region IGuiState Implementation
	
		public DataSet Source {get; set;}
			
			
		public void BindStates()
		{		
			TaskStateStore.Clear();
			foreach (DataRow row in Source.Tables["TaskState"].Rows)
			{
				TaskStateStore.AppendValues(row["ID"], row["Name"]);
				if (StateSearchDictionary == null)
					throw new ManagementException(ExceptionType.NotAllowed,"State Search Dictionary no set to instance of object");
				if (!StateSearchDictionary.ContainsKey(row["Name"]))
					StateSearchDictionary.Add(row["Name"],row["Name"]);
			}
			tvState.Model = TaskStateStore;
		}
		
		
		public void BindConnections(IManagerEntity stateEntry)
		{
			State state = (State)stateEntry;
			TaskStateConnectionStore = new TreeStore(typeof(int),typeof(string));
			TaskStateConnectionStore.Clear();
			foreach (int connectionID in state.Connections.Keys)
			{
				State connectedState = (State)stateCore.TaskManager.GetTaskStateConnection(connectionID);
				TaskStateStore.AppendValues(connectionID, connectedState.Name);
			}
			tvStateConnection.Model = TaskStateConnectionStore; 
		}

		
		public void CreateConnection(IManagerEntity stateEntry,IManagerEntity connectedEntry)
		{
		}

		
		public void EditConnection(IManagerEntity stateEntry,IManagerEntity connectedEntry)
		{
		}

		
		public void DeleteConnection(IManagerEntity stateEntry,IManagerEntity connectedEntry)
		{
		}

		
		public void ClearConnections(IManagerEntity stateEntry)
		{
		}
		
		#endregion

		private void BindStateSearchDictionary()
		{
			StateSearchDictionary = new Hashtable();  
		}

		
		public TreeStore TaskStateStore	{get;set;}

		
		public TreeStore TaskStateConnectionStore {	get;set; }

		
		private void OnApply(object sender, EventArgs args)
		{
			stateCore.StorageManager.Save();
		}
		
		public string StateSearch {	get;set; } 
		

		public Hashtable StateSearchDictionary { get; set; }


		public void BindStateSearchCompletion()
		{
			entStateSearch.Completion = new EntryCompletion();
			fStateSearchDictionaryStore = new ListStore(typeof(string));
			if (StateSearchDictionary == null)
				throw new ManagementException(ExceptionType.NotAllowed,"State Search Dictionary no set to instance of object");
			foreach (object searchKey in StateSearchDictionary.Keys)
				fStateSearchDictionaryStore.AppendValues(searchKey);
			
			entStateSearch.Completion.Model =fStateSearchDictionaryStore; 
			entStateSearch.Completion.TextColumn = 0; 
		}


		private void OnStateSearch(object sender, EventArgs args)
		{
			StateSearch = entStateSearch.Text;
			if (StateSearch.Trim().Length > 0)
			{
				if (StateSearchDictionary == null)
					throw new ManagementException(ExceptionType.NotAllowed,"State Search Dictionary no set to instance of object");
				
				if (!StateSearchDictionary.ContainsKey(StateSearch))
				{
					StateSearchDictionary.Add(StateSearch,StateSearch);
					BindStateSearchCompletion();
				}
				
				//fStateSource = Arise.Logic.DataSearch.GetFilteredDataSet(fStateCore.TaskManager.TaskStateSource, fStateSearch);				
			}
			else
				Source = stateCore.TaskManager.TaskStateSource;
			BindStates();
		}

		
		private void OnCreateState(object sender, EventArgs args)
		{
			stateCore.CreateTaskState();
		}

		
		private void OnEditState(object sender, EventArgs args)
		{
			TreeModel model;
			TreeIter iter;
			((TreeSelection)tvState.Selection).GetSelected (out model, out iter);
			int stateId = -1;
			bool stateRequired = true;
			try
			{
				object res = model.GetValue(iter,0);
				if(res != null)
				{
					stateId = (int)res;
				}
			}
			catch(Exception ex)
			{
				stateRequired = false;
				IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex ,thisDialog);
				dialog.Title = "State Edit";
				dialog.ShowDialog();
			}
			if (stateRequired && stateId != -1)
			{
				stateCore.EditTaskState(stateId);
			}
		}

		
		private void OnDeleteState(object sender, EventArgs args)
		{
			TreeModel model;
			TreeIter iter;
			((TreeSelection)tvState.Selection).GetSelected (out model, out iter);
			int stateId = -1;
			bool stateRequired = true;
			try
			{
				object res = model.GetValue(iter,0);
				if(res != null)
				{
					stateId = (int)res;
				}
			}
			catch(Exception ex)
			{
				stateRequired = false;
				IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex,thisDialog);
				dialog.Title = "State Edit";
				dialog.ShowDialog();
			}
			if (stateRequired && stateId != -1)
			{
				stateCore.DeleteTaskState(stateId);
			}
		}

		
		private void OnCreateConnection(object sender, EventArgs args)
		{
			TreeModel model;
			TreeIter iter;
			((TreeSelection)tvState.Selection).GetSelected (out model, out iter);				
			int stateId = -1;
			bool stateRequired = true;
			try
			{
				object res = model.GetValue(iter,0);
				if(res != null)
				{
					stateId = (int)res;
				}
			}
			catch(Exception ex)
			{
				stateRequired = false;
				IGuiMessageDialog dialog = MessageFactory.CreateErrorDialog(ex ,thisDialog);
				dialog.Title = "State Edit";
				dialog.ShowDialog();
			}
			if (stateRequired && stateId != -1)
			{
				stateCore.CreateTaskStateConnection(stateId);
			}
		}

		
		private void OnEditConnection(object sender, EventArgs args)
		{
		}

		
		private void OnDeleteConnection(object sender, EventArgs args)
		{
		}
	}
}
