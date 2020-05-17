using System.Threading.Tasks;
using Blockchain;
using Blockchain.Interfaces;
using Grpc.Core;
using Shared;
using Shared.Interfaces;

namespace NetworkingFacilities.Servers
{
    public class PublisherServerImpl : PublisherServer.PublisherServerBase
    {
        private IChainManipulator chainManipulator;
        private IServerAddressRepository serverAddressRepository;

        public PublisherServerImpl(IChainManipulator chainManipulator, IServerAddressRepository serverAddressRepository)
        {
            this.chainManipulator = chainManipulator;
            this.serverAddressRepository = serverAddressRepository;
        }

        public override Task<AckMessage> Ping(PingRequest request, ServerCallContext context)
        {
            return Task.FromResult(new AckMessage { Status = AckMessage.Types.Status.Ok });
        }

        public override Task<AckMessage> ReceiveChain(ChainMessage request, ServerCallContext context)
        {
            var success = chainManipulator.OnReceiveChain(new Chain(request.Xml));

            return Task.FromResult(
                new AckMessage()
                {
                    Status = success ? AckMessage.Types.Status.Ok : AckMessage.Types.Status.Nok
                }
            );
        }

        public override Task<AckMessage> RegisterNode(ServerAddressMessage request, ServerCallContext context)
        {
            serverAddressRepository.AddServerAddress(
                new ServerAddress()
                {
                    Ip = request.Ip
                    ,
                    Port = request.Port
                }
                );

            return Task.FromResult(new AckMessage { Status = AckMessage.Types.Status.Ok });
        }

        public override Task<ChainMessage> RequestDeltaChain(DeltaRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ChainMessage { Xml = chainManipulator.GetChainDelta(request.CurrentIndex).AsXML() });
        }

        public override Task<ChainMessage> RequestFullChain(ServerAddressMessage request, ServerCallContext context)
        {
            return Task.FromResult(new ChainMessage { Xml = chainManipulator.GetChain().AsXML() });
        }
    }
}
