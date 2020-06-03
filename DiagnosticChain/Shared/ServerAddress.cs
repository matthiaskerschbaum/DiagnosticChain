using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class ServerAddress : IEquatable<ServerAddress>
    {
        public static ServerAddress EmptyAddress = new ServerAddress()
        {
            Ip = "-1"
            ,
            Port = -1
        };

        public string Ip;
        public int Port;

        public string FullAddress
        {
            get
            {
                return Ip + ":" + Port;
            }
        }

        public bool Equals(ServerAddress other)
        {
            return this.FullAddress == other.FullAddress;
        }
    }
}
