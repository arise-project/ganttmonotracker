// created on 21.01.2006 at 20:14

using System.Collections;

namespace TaskManagerInterface
{
	public interface IStateView
	{
		string Name
		{
			get;
			set;
		}
		
		byte ColorGreen
		{
			get;
			set;
		}
		
		byte ColorRed
		{
			get;
			set;
		}
		
		byte ColorBlue
		{
			get;
			set;
		}
		
		
		int MappingID
		{
			get;
			set;
		}
		
		bool IsMapped 
		{
			get;
			set;
		}
		
		Hashtable Connections
		{
			get;
			set;
		}
		
		void Connect(IManagerEntity stateEntry, string connectionName);
		void Disconnect(IManagerEntity stateEntry);
		bool IsConnected(IManagerEntity stateEntry);
		void ClearConnections();						
	}
}
