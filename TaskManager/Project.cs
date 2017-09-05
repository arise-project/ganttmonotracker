using System;
using TaskManagerInterface;

namespace GanttMonoTracker
{
	//TODO: this class will be used to create dialog for manage projects and move tasks from one to another and add projects methadata
	public class Project : IManagerEntity, IProject
	{
		public Project(ITaskManager parent)
		{
			Parent = parent;
		}

		public Project(ITaskManager parent, int id)
		{
			Id = id;
			Parent = parent;
			Parent.BindProject(this);
		}

		public int Id { get; set; }

		public bool IsNew
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool isUpdated
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ITaskManager Parent { get; set; }

		public void BindData()
		{
			throw new NotImplementedException();
		}

		public void Delete()
		{
			throw new NotImplementedException();
		}

		public void Save()
		{
			throw new NotImplementedException();
		}
	}
}
