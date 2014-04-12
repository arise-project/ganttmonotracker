//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 07.02.2006 at 10:52

using System.Data;

namespace TaskManagerInterface
{
	public interface IGuiAssigment
	{
		DataSet AssigmentSource
		{
			get;
			set;
		}
		
		void BindAssigment();
	}
}