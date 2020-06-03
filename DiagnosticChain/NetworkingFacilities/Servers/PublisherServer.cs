using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Blockchain;
using Blockchain.Interfaces;
using Blockchain.Transactions;
using Grpc.Core;
using NetworkingFacilities.Clients;
using Shared;
using Shared.Interfaces;

namespace NetworkingFacilities.Servers
{
    public class PublisherServerImpl : PublisherServer.PublisherServerBase
    {
        private IChainManipulator chainManipulator;
        private IServerAddressRepository serverAddressRepository;
        private ServerAddress selfAddress;

        public PublisherServerImpl(IChainManipulator chainManipulator, IServerAddressRepository serverAddressRepository, ServerAddress selfAddress)
        {
            this.chainManipulator = chainManipulator;
            this.serverAddressRepository = serverAddressRepository;
            this.selfAddress = selfAddress;
        }

        public override Task<AckMessage> Ping(PingRequest request, ServerCallContext context)
        {
            return Task.FromResult(new AckMessage { Status = AckMessage.Types.Status.Ok });
        }

        public override Task<AckMessage> ReceiveChain(ChainMessage request, ServerCallContext context)
        {
            RegisterNode(request.SenderAddress, context);
            var success = chainManipulator.OnReceiveChain(new Chain(request.Xml));
            
            return Task.FromResult(
                new AckMessage()
                {
                    Status = success ? AckMessage.Types.Status.Ok : AckMessage.Types.Status.Nok
                }
            );
        }

        public override Task<AckMessage> ReceiveTransaction(TransactionMessage request, ServerCallContext context)
        {
            RegisterNode(request.SenderAddress, context);
            XmlSerializer serializer = new XmlSerializer(typeof(TransactionWrapper));
            TransactionWrapper wrappedTransaction = (TransactionWrapper)serializer.Deserialize(new StringReader(request.Xml));
            var success = chainManipulator.OnReceiveTransaction(wrappedTransaction.Transaction);

            return Task.FromResult(
                new AckMessage()
                {
                    Status = success ? AckMessage.Types.Status.Ok : AckMessage.Types.Status.Nok
                }
            );
        }

        public override Task<AckMessage> RegisterNode(ServerAddressMessage request, ServerCallContext context)
        {
            ServerAddress newAdress = new ServerAddress()
            {
                Ip = request.Ip
                    ,
                Port = request.Port
            };

            if (!serverAddressRepository.HasServerAddress(newAdress) && newAdress.FullAddress != selfAddress.FullAddress && newAdress.FullAddress != ServerAddress.EmptyAddress.FullAddress)
            {
                serverAddressRepository.AddServerAddress(newAdress);

                //Node an alle KnownNodes propagieren
                foreach (var address in serverAddressRepository.GetServerAddresses().Where(a => a.FullAddress != newAdress.FullAddress))
                {
                    new PublisherClient(address, selfAddress).RegisterNode(newAdress);
                }

                return Task.FromResult(new AckMessage { Status = AckMessage.Types.Status.Ok });
            }

            return Task.FromResult(new AckMessage { Status = AckMessage.Types.Status.Nok });
        }

        public override Task<ChainMessage> RequestDeltaChain(DeltaRequest request, ServerCallContext context)
        {
            RegisterNode(request.SenderAddress, context);
            return Task.FromResult(new ChainMessage { Xml = chainManipulator.GetChainDelta(request.CurrentIndex).AsXML() });
        }

        public override Task<ChainMessage> RequestFullChain(ServerAddressMessage request, ServerCallContext context)
        {
            RegisterNode(request, context);
            return Task.FromResult(new ChainMessage { Xml = chainManipulator.GetChain().AsXML() });
        }

        public override Task<ServerAddressMessageList> RequestNodes(ServerAddressMessage request, ServerCallContext context)
        {
            RegisterNode(request, context);
            ServerAddressMessageList nodes = new ServerAddressMessageList();

            foreach (var n in serverAddressRepository.GetServerAddresses())
            {
                nodes.AddressList.Add(new ServerAddressMessage()
                {
                    Ip = n.Ip
                    ,
                    Port = n.Port
                });
            }

            return Task.FromResult(nodes);
        }

        public override Task<PhysicianMessageList> RequestPendingPhysicians(ServerAddressMessage request, ServerCallContext context)
        {
            var physicians = chainManipulator.GetPendingPhysicians();
            PhysicianMessageList ret = new PhysicianMessageList();

            foreach (var p in physicians)
            {
                ret.PhysicianList.Add(new PhysicianMessage()
                {
                    Xml = p.AsXML()
                });
            }

            return Task.FromResult(ret);
        }
    }
}
