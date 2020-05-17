using Blockchain;
using Grpc.Core;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkingFacilities.Clients
{
    public class PublisherClient
    {
        private ServerAddress serverAddress;

        public PublisherClient(ServerAddress serverAddress)
        {
            this.serverAddress = serverAddress;
        }

        private Channel OpenConnection()
        {
            return new Channel(serverAddress.FullAddress, ChannelCredentials.Insecure);
        } 

        public AckMessage PingNode()
        {
            var channel = OpenConnection();
            var client = new PublisherServer.PublisherServerClient(channel);
            var response = client.Ping(new PingRequest());

            channel.ShutdownAsync().Wait();

            return response;
        }

        public bool RegisterNode(ServerAddress selfAddress)
        {
            var channel = OpenConnection();
            var client = new PublisherServer.PublisherServerClient(channel);
            var response = client.RegisterNode(new ServerAddressMessage
            {
                Ip = selfAddress.Ip
                ,
                Port = selfAddress.Port
            });

            channel.ShutdownAsync().Wait();

            return response.Status == AckMessage.Types.Status.Ok;
        }

        public Chain RequestDeltaChain(long currentIndex)
        {
            var channel = OpenConnection();
            var client = new PublisherServer.PublisherServerClient(channel);

            var response = client.RequestDeltaChain(new DeltaRequest
            {
                CurrentIndex = currentIndex
            });

            channel.ShutdownAsync().Wait();

            return new Chain(response.Xml);
        }

        public Chain RequestFullChain()
        {
            var channel = OpenConnection();
            var client = new PublisherServer.PublisherServerClient(channel);

            var response = client.RequestFullChain(new ServerAddressMessage
            {
                Ip = serverAddress.Ip
                ,
                Port = serverAddress.Port
            });

            channel.ShutdownAsync().Wait();

            return new Chain(response.Xml);
        }

        public AckMessage SendChain(Chain chain)
        {
            var channel = OpenConnection();
            var client = new PublisherServer.PublisherServerClient(channel);

            var response = client.ReceiveChain(new ChainMessage { Xml = chain.AsXML() });

            return response;
        }
    }
}
