using Blockchain.Interfaces;
using Blockchain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
    [Serializable]
    public class VotingTransaction : ITransaction
    {
        public Guid TransactionAddress { get; set; }
        public bool Vote { get; set; }

        public override string AsString()
        {
            return base.AsString() + "|" + TransactionAddress + "|" + Vote;
        }

        public override bool ValidateContextual(ParticipantHandler participantHandler, List<Chain> chains)
        {
            return 
                (participantHandler.IsEmpty() && participantHandler.IsVotablePublisher(TransactionAddress))
                || ((participantHandler.IsVotablePublisher(TransactionAddress) || participantHandler.IsVotablePhysician(TransactionAddress))
                    && participantHandler.HasSender(SenderAddress)
                    && ValidateTransactionIntegrity(participantHandler.GetSenderKey(SenderAddress)));
        }

        public override bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains)
        {
            if (!ValidateContextual(participantHandler, chains))
            {
                return false;
            }

            if (participantHandler.IsEmpty() && participantHandler.IsVotablePublisher(TransactionAddress)) //For the first publisher in the chain
            {
                if (Vote)
                {
                    return participantHandler.CastVoteForPublisher(TransactionAddress, SenderAddress);
                }
                else
                {
                    participantHandler.CastVoteAgainstPublisher(TransactionAddress, SenderAddress);
                }
            } else if (participantHandler.IsVotablePublisher(TransactionAddress) && participantHandler.HasPublisher(SenderAddress))
            {
                if (Vote)
                {
                    return participantHandler.CastVoteForPublisher(TransactionAddress, SenderAddress);
                } else
                {
                    participantHandler.CastVoteAgainstPublisher(TransactionAddress, SenderAddress);
                }
            } else if (participantHandler.IsVotablePhysician(TransactionAddress) && participantHandler.HasSender(SenderAddress))
            {
                if (Vote)
                {
                    participantHandler.CastVoteForPhysician(TransactionAddress, SenderAddress);
                }
                else
                {
                    participantHandler.CastVoteAgainstPhysician(TransactionAddress, SenderAddress);
                }
            } else
            {
                return false;
            }

            return true;
        }
    }
}
