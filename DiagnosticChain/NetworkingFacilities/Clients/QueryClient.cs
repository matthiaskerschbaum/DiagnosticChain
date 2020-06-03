using Blockchain;
using Grpc.Core;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkingFacilities.Clients
{
    public class QueryClient
    {
        private ServerAddress serverAddress;

        public QueryClient(ServerAddress serverAddress)
        {
            this.serverAddress = serverAddress;
        }

        private Channel OpenConnection()
        {
            return new Channel(serverAddress.FullAddress, ChannelCredentials.Insecure);
        }

        public Chain RequestDeltaChain(long currentIndex)
        {
            var channel = OpenConnection();
            var client = new PublisherServer.PublisherServerClient(channel);

            var senderAddress = new ServerAddressMessage()
            {
                Ip = ServerAddress.EmptyAddress.Ip
                ,
                Port = ServerAddress.EmptyAddress.Port
            };

            var response = client.RequestDeltaChain(new DeltaRequest
            {
                SenderAddress = senderAddress
                ,
                CurrentIndex = currentIndex
            });

            channel.ShutdownAsync().Wait();

            return new Chain(response.Xml);
        }

        public Chain RequestFullChain()
        {
            var channel = OpenConnection();
            var client = new PublisherServer.PublisherServerClient(channel);

            var senderAddress = new ServerAddressMessage()
            {
                Ip = ServerAddress.EmptyAddress.Ip
                ,
                Port = ServerAddress.EmptyAddress.Port
            };

            var response = client.RequestFullChain(senderAddress);

            channel.ShutdownAsync().Wait();

            return new Chain(response.Xml);
        }

        public List<ServerAddress> RequestNodes()
        {
            var channel = OpenConnection();
            var client = new PublisherServer.PublisherServerClient(channel);

            var response = client.RequestNodes(
                new ServerAddressMessage()
                {
                    Ip = ServerAddress.EmptyAddress.Ip
                    ,
                    Port = ServerAddress.EmptyAddress.Port
                }
                );

            channel.ShutdownAsync();

            List<ServerAddress> ret = new List<ServerAddress>();

            foreach (var n in response.AddressList)
            {
                ret.Add(new ServerAddress()
                {
                    Ip = n.Ip
                    ,
                    Port = n.Port
                });
            }

            return ret;
        }
    }
}
