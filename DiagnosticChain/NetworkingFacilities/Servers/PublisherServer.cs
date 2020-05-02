using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;

namespace NetworkingFacilities.Servers
{
    public class PublisherServerImpl : PublisherServer.PublisherServerBase
    {
        public override Task<PingResult> Ping(PingRequest request, ServerCallContext context)
        {
            return Task.FromResult(new PingResult { Result = "Listening!" }); ;
        }
    }
}
