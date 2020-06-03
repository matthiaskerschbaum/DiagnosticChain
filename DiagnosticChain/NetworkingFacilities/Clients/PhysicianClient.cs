using Blockchain.Entities;
using Blockchain.Interfaces;
using Grpc.Core;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace NetworkingFacilities.Clients
{
    public class PhysicianClient
    {
        private ServerAddress serverAddress;

        public PhysicianClient(ServerAddress serverAddress)
        {
            this.serverAddress = serverAddress;
        }

        private Channel OpenConnection()
        {
            return new Channel(serverAddress.FullAddress, ChannelCredentials.Insecure);
        }

        public List<Physician> RequestPhysicians()
        {
            var channel = OpenConnection();
            var client = new PublisherServer.PublisherServerClient(channel);

            var response = client.RequestPendingPhysicians(new ServerAddressMessage()
            {
                Ip = ServerAddress.EmptyAddress.Ip
                ,
                Port = ServerAddress.EmptyAddress.Port
            });

            List<Physician> ret = new List<Physician>();
            foreach (var p in response.PhysicianList)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Physician), new Type[] { typeof(ITransaction) });
                Physician physician = (Physician)serializer.Deserialize(new StringReader(p.Xml));
                ret.Add(physician);
            }

            return ret;
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

        public AckMessage SendTransaction(ITransaction transaction)
        {
            var channel = OpenConnection();
            var client = new PublisherServer.PublisherServerClient(channel);

            var senderAddress = new ServerAddressMessage()
            {
                Ip = ServerAddress.EmptyAddress.Ip
                , Port = ServerAddress.EmptyAddress.Port
            };

            var response = client.ReceiveTransaction(new TransactionMessage()
            {
                SenderAddress = senderAddress
                ,Xml = new TransactionWrapper(transaction).AsXML()
            });;

            channel.ShutdownAsync().Wait();

            return response;
        }
    }
}
