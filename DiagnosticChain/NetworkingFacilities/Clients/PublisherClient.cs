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
        private ServerAddress selfAddress;

        public PublisherClient(ServerAddress serverAddress, ServerAddress selfAddress)
        {
            this.serverAddress = serverAddress;
            this.selfAddress = selfAddress;
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
            AckMessage response = new AckMessage();
            try
            {
                CLI.DisplayLine("Attempting connection");
                response = client.RegisterNode(new ServerAddressMessage
                {
                    Ip = selfAddress.Ip
                    ,
                    Port = selfAddress.Port
                });
            } catch (RpcException)
            {
                CLI.DisplayLine("Connection failed");
                return false;
            }

            channel.ShutdownAsync().Wait();

            return response.Status == AckMessage.Types.Status.Ok;
        }

        public Chain RequestDeltaChain(long currentIndex)
        {
            var channel = OpenConnection();
            var client = new PublisherServer.PublisherServerClient(channel);

            var response = client.RequestDeltaChain(new DeltaRequest
            {
                SenderAddress = new ServerAddressMessage()
                {

                    Ip = selfAddress.Ip
                    ,
                    Port = selfAddress.Port
                }
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

            var response = client.RequestFullChain(new ServerAddressMessage
            {
                Ip = serverAddress.Ip
                ,
                Port = serverAddress.Port
            });

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
                    Ip = selfAddress.Ip
                    ,
                    Port = selfAddress.Port
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

        public AckMessage SendChain(Chain chain)
        {
            var channel = OpenConnection();
            var client = new PublisherServer.PublisherServerClient(channel);

            var response = client.ReceiveChain(new ChainMessage
            {
                SenderAddress = new ServerAddressMessage()
                {

                    Ip = selfAddress.Ip
                    ,
                    Port = selfAddress.Port
                }
                ,
                Xml = chain.AsXML()
            });

            return response;
        }
    }
}
