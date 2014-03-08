// created on 31.01.2006 at 2:33

using System;
using System.IO;
using System.Xml;
using System.Collections; 
using GanttTracker.TaskManager.ManagerException;
using TaskManagerInterface;

namespace GanttTracker.ProjectManager
{
	public class ProjectHistory
	{
		private ProjectHistory()
		{
			fProjects = new Hashtable();
		}
		
		private static ProjectHistory fInstance;
		public static ProjectHistory Instance
		{
			get
			{
				if (fInstance == null)
					fInstance = new ProjectHistory();
				return fInstance;
			}
			
			set
			{
			}			
		}
		
		private Hashtable fProjects;
		public Hashtable Projects
		{
			get
			{
				return fProjects;
			}
			set
			{
				fProjects = value;
			}
		}
		
		public void SaveProject(string fileName)
		{					 
			if (fProjects != null)
			{
				XmlDocument doc = new XmlDocument();
				XmlElement mainTeg = (XmlElement)doc.CreateElement("projectHistory");			
				
				foreach (string path in fProjects.Keys)
				{
					XmlElement projectTeg = (XmlElement)doc.CreateElement("project");
					projectTeg.SetAttribute("path",path);
					projectTeg.SetAttribute("name",(string)fProjects[path]);
					mainTeg.AppendChild(projectTeg);					
				}
				doc.AppendChild(mainTeg);
				StreamWriter s = new StreamWriter(fileName); 
				doc.Save(s.BaseStream);
				s.Close();
			}
		}
		
		public void OpenProject(string fileName)
		{
			if (fProjects != null)
			{
				StreamReader r = new StreamReader(fileName);
				XmlDocument doc = new XmlDocument();
				doc.Load(r.BaseStream);
				r.Close();
				XmlNodeList list = doc.SelectNodes(@"projectHistory\project");
				foreach (XmlNode projectNode in list)
				{
					if (!fProjects.ContainsKey(((XmlElement)projectNode).GetAttribute("path")))
					{
						fProjects.Add(((XmlElement)projectNode).GetAttribute("path"),((XmlElement)projectNode).GetAttribute("name"));
					}
				}
			}			
		} 
	}
}