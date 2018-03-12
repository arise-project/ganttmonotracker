//----------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 08.11.2005 at 23:25
using System.Collections.Generic;
using System.Linq;

namespace GanttMonoTracker.GuiPresentation
{
	using System;
	using System.Configuration;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using DrawingPresentation;
	using ExceptionPresentation;

	using GanttTracker;
	using GanttTracker.TaskManager.ManagerException;

	using Gdk;

	using Glade;

	using Gtk;

	using TaskManagerInterface;

	public class MainForm : Gtk.Window, IGuiTracker
	{
		#pragma warning disable 0649
		[WidgetAttribute]
		Button btnAssignTask;
		[WidgetAttribute]
		Button btnChangeTask;
		[WidgetAttribute]
		Button btnCreateTask;
		[WidgetAttribute]
		Button btnSearchTask;
		DrawingArea drwAssigment;
		//DrawingArea drwGantt;
		[WidgetAttribute]
		Entry entSearchTask;
		[WidgetAttribute]
		Button btnGoogleGantt;

		#pragma warning restore 0649

		private readonly TreeStore fActorStore = new TreeStore(typeof(int), typeof(string), typeof(string));
		private FileSelection fFileSelection; // todo : FileChooserWidget

		//                                   id,          status          description,   start time,     end time,      actor,         
		TreeStore fTaskStore = new TreeStore(typeof(int), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));

		#pragma warning disable 0649

		[WidgetAttribute]
		MenuItem miAbout;
		[WidgetAttribute]
		MenuItem miActorCreate;
		[WidgetAttribute]
		MenuItem miActorDelete;
		[WidgetAttribute]
		MenuItem miActorEdit;
		[WidgetAttribute]
		MenuItem miAssignTask;
		[WidgetAttribute]
		MenuItem miChangeTaskState;
		[WidgetAttribute]
		MenuItem miCloseProject;
		[WidgetAttribute]
		MenuItem miCreateProject;
		[WidgetAttribute]
		MenuItem miExit;
		[WidgetAttribute]
		MenuItem miOpenProject;
		[WidgetAttribute]
		MenuItem miRecentProject;
		[WidgetAttribute]
		MenuItem miSaveProject;
		[WidgetAttribute]
		MenuItem miStateEdit;
		[WidgetAttribute]
		MenuItem miTaskCreate;
		[WidgetAttribute]
		MenuItem miUpdateFromXml;
		[WidgetAttribute]
		MenuItem miExportToHtml;
		[WidgetAttribute]
		MenuItem miGooglePoint;
		[WidgetAttribute]
		TreeView tvActorTree;
		[WidgetAttribute]
		TreeView tvTaskTree;

		#pragma warning restore 0649

		private string searchTask;
		private string selectedFile;
		readonly bool[] taskSort = new bool[6];

		#pragma warning disable 0649
		/// <summary>
		/// Gantt.
		/// </summary>
		[Widget]
		VBox vbox3;

		/// <summary>
		/// Assigment.
		/// </summary>
		[Widget]
		VBox vbox4;
		#pragma warning disable 0649

		private string recent;

		public MainForm()
		: base("Gantt Tracker")
		{
			InitializeComponents();

			var r = TrackerCore.Instance.Recent;
			bool foundRecent = false;
			for (int i = 0; i < r.Count (); i++) {
				if (File.Exists (r [i])) {
					foundRecent = true;
					recent = r [i];
					break;
				}
			}
			if (!foundRecent) {
				var logoWindow = new LogoForm();
				logoWindow.ShowDialog();
			}



			TrackerCore.Instance.Tracker = this;

			TrackerCore.Instance.State = CoreState.EmptyProject;
			TrackerCore.Instance.BindProject();
		}


		public DataSet ActorSource
		{
			get;
			set;
		}

		public DataSet StateSource
		{
			get;
			set;
		}

		public DataSet TaskSource
		{
			get;
			set;
		}

		public void BindActor()
		{
			fActorStore.Clear();
			foreach (DataRow row in ActorSource.Tables["Actor"].Rows)
			{
				fActorStore.AppendValues(row["ID"], row["Name"], row["Email"]);
			}

			tvActorTree.Model = fActorStore;
		}

		public void BindState()
		{
		}

		public void BindTask()
		{
			fTaskStore.Clear();
			var passedState = ConfigurationManager.AppSettings["passed_state"];
			try
			{
				foreach (DataRow row in TaskSource.Tables["Task"].Rows)
				{
					var actorName = ActorSource.Tables["Actor"].Select("ID = " + (int)row["ActorID"])[0]["Name"];
					var stateName = string.Empty;

					if (StateSource.Tables["TaskState"].Select("ID = " + (int)row["StateID"]).Length > 0)
					{
						stateName = (string)StateSource.Tables["TaskState"].Select("ID = " + (int)row["StateID"])[0]["Name"];
					}

					if (passedState.Split(';').Select(s => s.ToLower()).Contains(stateName.ToLower()))
					{
						continue;
					}

					if (!string.IsNullOrEmpty(searchTask))
					{
						string text = string.Join("", row["Id"], row["Description"], row["StartTime"], actorName, stateName);
						if (!text.ToLower().Contains(searchTask.ToLower()))
						{
							continue;
						}
					}

					var itemNode = fTaskStore.AppendNode();
					fTaskStore.SetValue(itemNode, 0, row["Id"]);
					fTaskStore.SetValue(itemNode, 1, stateName);
					fTaskStore.SetValue(itemNode, 2, row["Description"]);
					fTaskStore.SetValue(itemNode, 3, ((DateTime)row["StartTime"]).ToShortDateString());
					fTaskStore.SetValue(itemNode, 4, ((DateTime)row["EndTime"]).ToShortDateString());
					fTaskStore.SetValue(itemNode, 5, actorName);
				}
			}
			catch (Exception ex)
			{
				var m = ex.Message;
				//todo: fix ui exception
			}
		}

		public void OnRecentProject(object sender, EventArgs args)
		{
			var r = new List<string>(TrackerCore.Instance.Recent)
				.Where(f => !string.Equals(f, recent, StringComparison.Ordinal))
				.Concat(new string[]{recent});

			bool foundRecent = false;
			foreach(var f in r) {
				if (File.Exists (f)) {
					foundRecent = true;
					recent = f;
					break;
				}
			}
			if (!foundRecent)
				return;

			File.WriteAllLines("recent.txt".GetPath(), r);
			selectedFile = recent;
			TrackerCore.Instance.State = CoreState.OpenProject;
			OnFileSelectionResponce(fFileSelection, null);
		}

		[GLib.ConnectBefore]
		private void HandleTaskButtonPressEvent(object o, ButtonPressEventArgs args)
		{
			if (args.Event.Type == EventType.TwoButtonPress)
			{
				OnChangeTaskState(o, EventArgs.Empty);
			}
		}

		[GLib.ConnectBefore]
		private void HandleActorButtonPressEvent(object o, ButtonPressEventArgs args)
		{
			if (args.Event.Type == EventType.TwoButtonPress)
			{
                OnActorEdit(o, EventArgs.Empty);
			}
		}

		private void InitializeComponents()
		{
			var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MainForm.glade");
			var glade = new XML(stream, "MainForm", null);
			stream.Close();
			glade.Autoconnect(this);
			IconList = new[] { new Pixbuf(System.Reflection.Assembly.GetExecutingAssembly(), "GMTLogo.bmp") };
			Icon = IconList[0];

			if (miCreateProject != null)
			{
				miCreateProject.Activated += OnCreateProject;
			}
			else
			{
				throw new KeyNotFoundException<string>("miCreateProject");
			}

			BindRecentProjects(this, EventArgs.Empty);

			miOpenProject.Activated += OnOpenProject;
			miSaveProject.Activated += OnSaveProject;
			miCloseProject.Activated += OnCloseProject;
			miExit.Activated += OnExitProgramm;

			miActorCreate.Activated += OnActorCreate;
			miActorEdit.Activated += OnActorEdit;
			miActorDelete.Activated += OnActorDelete;

			miTaskCreate.Activated += OnTaskCreate;
			miChangeTaskState.Activated += OnChangeTaskState;
			miAssignTask.Activated += OnTaskAssign;

			miStateEdit.Activated += OnStateEdit;

			miUpdateFromXml.Activated += OnUpdateFromXml;

			miExportToHtml.Activated += OnExportToHtml;
			miGooglePoint.Activated += OnGooglePoint;

			miAbout.Activated += OnAbout;

			btnCreateTask.Clicked += OnTaskCreate;
			btnAssignTask.Clicked += OnTaskAssign;
			btnChangeTask.Clicked += OnChangeTaskState;
			btnSearchTask.Clicked += OnSearchTask;
			btnGoogleGantt.Clicked += OnGoogleGantt;

			tvActorTree.HeadersVisible = true;
			tvActorTree.AppendColumn("Name", new CellRendererText(), "text", 1);
			tvActorTree.AppendColumn("Email", new CellRendererText(), "text", 2);
			tvActorTree.ButtonPressEvent += HandleActorButtonPressEvent;

			//tvActorTree.AppendColumn("Id",new CellRendererText(), "text", 2).Visible = false;
			tvTaskTree.HeadersVisible = true;
			tvTaskTree.AppendColumn("Id", new CellRendererText(), "text", 0);
			tvTaskTree.AppendColumn("State", new CellRendererText(), "text", 1);
			tvTaskTree.AppendColumn("Description", new CellRendererText(), "text", 2);
			tvTaskTree.AppendColumn("Start Time", new CellRendererText(), "text", 3);
			tvTaskTree.AppendColumn("End Time", new CellRendererText(), "text", 4);
			tvTaskTree.AppendColumn("Actor", new CellRendererText(), "text", 5);

			tvTaskTree.ButtonPressEvent += HandleTaskButtonPressEvent;
			//todo: use multi row mode
			//tvTaskTree.Selection.Mode = SelectionMode.Multiple;

			// Assigment
			drwAssigment = new AssigmentDiagramm();
			vbox4.Add(drwAssigment);
			drwAssigment.Show();



			//vbox3.Add(vb);

			for (var i = 2; i < 5; i++)
			{
				fTaskStore.SetSortFunc(i, delegate (TreeModel model, TreeIter a, TreeIter b)
				{
					var s1 = model.GetValue(a, 0).ToString();
					var s2 = model.GetValue(b, 0).ToString();
					return string.Compare(s1, s2);
				});
			}

			fTaskStore.SetSortFunc(0, delegate (TreeModel model, TreeIter a, TreeIter b)
			{
				var s1 = (int)model.GetValue(a, 0);
				var s2 = (int)model.GetValue(b, 0);
				return s1 > s2 ? -1 : s1 == s2 ? 0 : 1;
			});

			tvTaskTree.Model = fTaskStore;
			var index = 0;
			tvTaskTree.Columns.ToList().ForEach(c =>
			{
				c.Clickable = true;
				c.SortColumnId = index++;
				c.Clicked += (sender, e) =>
				{
					taskSort[c.SortColumnId] = !taskSort[c.SortColumnId];
					fTaskStore.SetSortColumnId(c.SortColumnId, taskSort[c.SortColumnId] ? SortType.Ascending : SortType.Descending);
				};
			});
		}

		[GLib.ConnectBefore]
		void BindRecentProjects(object o, EventArgs args)
		{
			if (TrackerCore.Instance.Recent.Any())
			{
				var recentMenu = new Menu();

				//TODO: Submenu not visible
				//TODO: Recent projects should be limited to a range not to last item
				var lastItem = new MenuItem("test"); //new MenuItem(System.IO.Path.GetFileNameWithoutExtension(r)) { TooltipText = r };
				lastItem.Activated += OnRecentProject;
				//miRecentProject.Submenu.Chi(lastItem);
				//miRecentProject.Submenu = recentMenu;
				miRecentProject.Visible = true;
			}
			else
			{
				miRecentProject.Visible = false;
			}
		}

		//TODO: what exactly commands will be useed with multyrow selection
		private void RemoveSelectedRows(TreeView treeView, ListStore listStore, TreeModelSort treeModelSort)
		{
			TreeModel model;
			TreeIter iter;

			TreePath[] treePath = treeView.Selection.GetSelectedRows(out model);

			for (int i  = treePath.Length; i > 0; i--)
			{
				model.GetIter(out iter, treePath[(i - 1)]);

				string value = (string)model.GetValue(iter, 0);
				Console.WriteLine("Removing: " + value);

				TreeIter childIter = treeModelSort.ConvertIterToChildIter(iter);
				listStore.Remove(ref childIter);
			}
		}

		private static void OnAbout(object sender, EventArgs args)
		{
			TrackerCore.Instance.ShowAboutDialog();
		}

		private static void OnActorCreate(object sender, EventArgs args)
		{
			TrackerCore.Instance.CreateActor();
		}

		private void OnActorDelete(object sender, EventArgs args)
		{
			TreeModel model;
			TreeIter iter;
			tvActorTree.Selection.GetSelected(out model, out iter);
			var actorId = -1;
			var actionRequired = true;
			try
			{
				var res = model.GetValue(iter, 0);
				if (res != null)
				{
					actorId = (int)res;
				}
			}
			catch (Exception ex)
			{
				actionRequired = false;
				var dialog = MessageFactory.CreateErrorDialog(ex, this);
				dialog.Title = "Actor Edit";
				dialog.ShowDialog();
			}

			if (actionRequired && actorId != -1)
			{
				TrackerCore.Instance.DeleteActor(actorId);
			}
		}

		private void OnActorEdit(object sender, EventArgs args)
		{
			TreeModel model;
			TreeIter iter;

			tvActorTree.Selection.GetSelected(out model, out iter);

			var actorId = -1;
			var actionRequierd = true;
			try
			{
				var res = model.GetValue(iter, 0);
				if (res != null)
				{
					actorId = (int)res;
				}
			}
			catch (Exception ex)
			{
				actionRequierd = false;
				var dialog = MessageFactory.CreateErrorDialog(ex, this);
				dialog.Title = "Actor Edit";
				dialog.ShowDialog();
			}

			if (actionRequierd && actorId != -1)
			{
				TrackerCore.Instance.EditActor(actorId);
			}
		}

		private void OnChangeTaskState(object sender, EventArgs args)
		{
			TreeModel model;
			TreeIter iter;
			tvTaskTree.Selection.GetSelected(out model, out iter);
			var taskID = -1;
			var actionRequired = true;
			try
			{
				var res = model.GetValue(iter, 0);
				if (res != null)
				{
					taskID = (int)res;
				}
			}
			catch (Exception ex)
			{
				actionRequired = false;
				var dialog = MessageFactory.CreateErrorDialog(ex, this);
				dialog.Title = "Change Task State";
				dialog.ShowDialog();
			}

			if (actionRequired && taskID != -1)
			{
				TrackerCore.Instance.UpdateTaskState(taskID);
			}
		}

		private static void OnCloseProject(object sender, EventArgs args)
		{
			TrackerCore.Instance.State = CoreState.EmptyProject;
			TrackerCore.Instance.BindProject();
		}

		private void OnCreateProject(object sender, EventArgs args)
		{
			TrackerCore.Instance.State = CoreState.CreateProject;
			fFileSelection = new FileSelection("Cerate Project");
			fFileSelection.Response += OnFileSelectionResponce;
			fFileSelection.Run();
			fFileSelection.Hide();
		}

		private static void OnExitProgramm(object sender, EventArgs args)
		{
			Application.Quit();
		}

		private void OnFileSelectionResponce(object sender, ResponseArgs args)
		{
			if (args != null && args.ResponseId != ResponseType.Ok) return;
			switch (TrackerCore.Instance.State)
			{
				case CoreState.CreateProject:
				case CoreState.OpenProject:
					TrackerCore.Instance.ProjectFileName = args == null ? selectedFile : ((FileSelection)sender).Filename;

					TrackerCore.Instance.BindProject();
					if(!TrackerCore.Instance.Recent.Contains(TrackerCore.Instance.ProjectFileName))
				    	File.WriteAllLines("recent.txt".GetPath(), 
						new string[]
						{
							TrackerCore.Instance.ProjectFileName
						}
						.Concat(TrackerCore.Instance.Recent));
					
					//backup file.
					File.AppendAllText(TrackerCore.Instance.ProjectFileName + ".backup",
						File.ReadAllText(TrackerCore.Instance.ProjectFileName));
					break;
				case CoreState.EmptyProject:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

        [GLib.ConnectBefore]
		private void OnSearchKeyPress(object sender, KeyPressEventArgs args)
		{	
			if(args.Event.Key == Gdk.Key.Return)
				OnSearchTask(sender, args);
		}

		private void OnKeyPress(object sender, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.F2 ||
				args.Event.State == (ModifierType.ControlMask | ModifierType.Mod2Mask) &&
				args.Event.Key == Gdk.Key.o)
			{
				OnOpenProject(this, new EventArgs());
			}

			if (args.Event.Key == Gdk.Key.F3 ||
				args.Event.State == (ModifierType.ControlMask | ModifierType.Mod2Mask) &&
				args.Event.Key == Gdk.Key.s)
			{
				OnSaveProject(this, new EventArgs());
			}

			if (args.Event.State == (ModifierType.ControlMask | ModifierType.Mod2Mask) &&
				args.Event.Key == Gdk.Key.q)
			{
				OnExitProgramm(this, new EventArgs());
			}

			if (args.Event.State == (ModifierType.ControlMask | ModifierType.Mod2Mask) &&
				args.Event.Key == Gdk.Key.e)
			{
				OnCloseProject(this, new EventArgs());
			}
		}

		private void OnOpenProject(object sender, EventArgs args)
		{
			TrackerCore.Instance.State = CoreState.OpenProject;
			fFileSelection = new FileSelection("Open Project");
			fFileSelection.Response += OnFileSelectionResponce;
			fFileSelection.Run();
			fFileSelection.Hide();
		}

		private static void OnSaveProject(object sender, EventArgs args)
		{
			TrackerCore.Instance.SaveProject();
		}

		private void OnGoogleGantt(object sender, EventArgs args)
		{
			try
			{
				//todo: for linux should be default browse instead. Mono Howto OpenBrowser
				//http://www.mono-project.com/archived/howto_openbrowser/
				//Browser compatibility mode
				//https://blogs.msdn.microsoft.com/patricka/2015/01/12/controlling-webbrowser-control-compatibility/
				//https://stackoverflow.com/questions/6771258/what-does-meta-http-equiv-x-ua-compatible-content-ie-edge-do

				//for now browser on windows has error : libgluezilla not found. To have webbrowser support, you need libgluezilla installed
				
				//I think it is possible to check Cef.Glue for linux, it uses Chronium
				//Xilium.CefGlue
				//Xilium.CefGlue is a .NET/Mono binding for The Chromium Embedded Framework (CEF) by Marshall A. Greenblatt.
				//https://bitbucket.org/xilium/xilium.cefglue/wiki/Home
				//The Chromium Embedded Framework (CEF) is a simple framework for embedding Chromium-based browsers in other applications.

				var msForm = new BrowserForm
				{
					StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen,
					Width = 800,
					Height = 600
				};

				msForm.Show();
			}
			catch (Exception ex)
			{
			}
		}

		private void OnSearchTask(object sender, EventArgs args)
		{
			searchTask = entSearchTask.Text;
			BindTask();
		}

		private void OnStateEdit(object sender, EventArgs args)
		{
			try
			{
				TrackerCore.Instance.StateEdit();
			}
			catch (InvalidCastException ex)
			{
				var dialog = MessageFactory.CreateErrorDialog(ex, this);
				dialog.Title = "Actor Edit";
				dialog.ShowDialog();
			}
		}

		private void OnTaskAssign(object sender, EventArgs args)
		{
			TreeModel model;
			TreeIter iter;
			tvTaskTree.Selection.GetSelected(out model, out iter);

			var taskID = -1;
			var actionRequired = true;
			try
			{
				var res = model.GetValue(iter, 0);
				if (res != null)
				{
					taskID = (int)res;
				}
			}
			catch (Exception ex)
			{
				actionRequired = false;
				var dialog = MessageFactory.CreateErrorDialog(ex, this);
				dialog.Title = "Assign Task";
				dialog.ShowDialog();
			}

			if (actionRequired && taskID != -1)
			{
				TrackerCore.Instance.AssignTask(taskID);
			}
		}

		private static void OnTaskCreate(object sender, EventArgs args)
		{
			TrackerCore.Instance.CreateTask();
		}

		private static void OnUpdateFromXml(object sender, EventArgs args)
		{
		}

		private void OnExportToHtml(object sender, EventArgs args)
		{
			var html = HtmlHelpders.ToHTMLTable(TaskSource.Tables["Task"]);            
			File.WriteAllText(ConfigurationManager.AppSettings["webpage"], html);
		}

		private async void OnGooglePoint(object sender, EventArgs args)
		{
			await TrackerCore.Instance.TaskManager.SyncronizeAsync();
		}

		private void OnWindowDeleteEvent(object sender, DeleteEventArgs a)
		{
			a.RetVal = true;
			Application.Quit();
		}
	}
}