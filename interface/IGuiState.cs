// created on 22.01.2006 at 3:28

using System;
using System.Data;

namespace TaskManagerInterface
{	
	public interface IGuiState
	{
		DataSet StateSource
		{
			get;		
			set;		
		}	
		
		void BindStates();
		void BindConnections(IManagerEntity stateEntry);
		void CreateConnection(IManagerEntity stateEntry,IManagerEntity connectedEntry);
		void EditConnection(IManagerEntity stateEntry,IManagerEntity connectedEntry);
		void DeleteConnection(IManagerEntity stateEntry,IManagerEntity connectedEntry);
		void ClearConnections(IManagerEntity stateEntry);					
	}
}
