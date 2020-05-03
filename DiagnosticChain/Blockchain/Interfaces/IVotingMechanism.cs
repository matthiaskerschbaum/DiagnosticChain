using Blockchain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Interfaces
{
    internal interface IVotingMechanism
    {
        bool CastVoteAgainstPhysician(ParticipantHandler participantHandler, Guid physicianAddress, Guid senderAddress);
        bool CastVoteAgainstPublisher(ParticipantHandler participantHandler, Guid publisherAddress, Guid senderAddress);
        bool CastVoteForPhysician(ParticipantHandler participantHandler, Guid physicianAddress, Guid senderAddress);
        bool CastVoteForPublisher(ParticipantHandler participantHandler, Guid publisherAddress, Guid senderAddress);
    }
}
