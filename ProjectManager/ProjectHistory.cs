//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 31.01.2006 at 2:33

using System.IO;
using System.Xml;
using System.Collections;

namespace GanttTracker.ProjectManager
{
    public class ProjectHistory
	{
		static ProjectHistory fInstance;

		ProjectHistory()
		{
			Projects = new Hashtable();
		}
		
		public static ProjectHistory Instance
		{
			get
			{
				if (fInstance == null)
					fInstance = new ProjectHistory();
				return fInstance;
			}	
		}

		public Hashtable Projects {	get;set; }

		public void SaveProject(string fileName)
		{
			if (Projects != null)
			{
				XmlDocument doc = new XmlDocument();
				XmlElement mainTeg = (XmlElement)doc.CreateElement("projectHistory");			
				
				foreach (string path in Projects.Keys)
				{
					XmlElement projectTeg = (XmlElement)doc.CreateElement("project");
					projectTeg.SetAttribute("path",path);
					projectTeg.SetAttribute("name",(string)Projects[path]);
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
			if (Projects != null)
			{
				StreamReader r = new StreamReader(fileName);
				XmlDocument doc = new XmlDocument();
				doc.Load(r.BaseStream);
				r.Close();
				XmlNodeList list = doc.SelectNodes(@"projectHistory\project");
				foreach (XmlNode projectNode in list)
				{
					var node = (XmlElement)projectNode;
					if (!Projects.ContainsKey(node.GetAttribute("path")))
					{
						Projects.Add(node.GetAttribute("path"), node.GetAttribute("name"));
					}
				}
			}
		} 
	}
}