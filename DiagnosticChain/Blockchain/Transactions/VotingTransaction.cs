﻿using Blockchain.Interfaces;
using Blockchain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
    public class VotingTransaction : ITransaction
    {
        public Guid TransactionAddress { get; set; }
        public bool Vote { get; set; }

        internal override string AsString()
        {
            return base.AsString() + "|" + TransactionAddress + "|" + Vote;
        }

        public override bool ValidateContextual(ParticipantHandler participantHandler, List<Chain> chains)
        {
            throw new NotImplementedException();
        }

        public override bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains)
        {
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
            }

            if (participantHandler.IsVotablePublisher(TransactionAddress) && participantHandler.HasPublisher(SenderAddress))
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
