//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 21.01.2006 at 20:14

namespace TaskManagerInterface
{
    using System.Collections;

    public interface IState
    {
        byte ColorBlue
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

        Hashtable Connections
        {
            get;
            set;
        }

        bool IsMapped
        {
            get;
            set;
        }

        int MappingID
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        void ClearConnections();

        void Connect(IManagerEntity stateEntry, string connectionName);

        void Disconnect(IManagerEntity stateEntry);

        bool IsConnected(IManagerEntity stateEntry);
    }
}