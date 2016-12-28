//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 22.01.2006 at 3:28

namespace TaskManagerInterface
{
    using System;
    using System.Data;

    public interface IGuiState : IGuiSource
    {
        void BindConnections(IManagerEntity stateEntry);

        void BindStates();

        void ClearConnections(IManagerEntity stateEntry);

        void CreateConnection(IManagerEntity stateEntry,IManagerEntity connectedEntry);

        void DeleteConnection(IManagerEntity stateEntry,IManagerEntity connectedEntry);

        void EditConnection(IManagerEntity stateEntry,IManagerEntity connectedEntry);
    }
}