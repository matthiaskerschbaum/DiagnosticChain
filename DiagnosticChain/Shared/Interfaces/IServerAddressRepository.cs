using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces
{
    public interface IServerAddressRepository
    {
        void AddServerAddress(ServerAddress address);
        List<ServerAddress> GetServerAddresses();
        bool HasServerAddress(ServerAddress address);
        void DeleteServerAddress(ServerAddress address);
    }
}
