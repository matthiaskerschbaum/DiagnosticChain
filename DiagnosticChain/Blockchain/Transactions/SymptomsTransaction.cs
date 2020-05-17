using Blockchain.Interfaces;
using Blockchain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
    public class SymptomsTransaction : ITransaction
    {
        public Guid TreatmentTransactionAddress { get; set; }
        public List<string> Symptoms { get; set; }

        internal override string AsString()
        {
            var ret = base.AsString() + "|" + TreatmentTransactionAddress;

            foreach (var s in Symptoms)
            {
                ret += "|" + s;
            }

            return ret;
        }

        public override bool ValidateContextual(ParticipantHandler participantHandler, List<Chain> chains)
        {
            var valid = false;

            valid &= participantHandler.HasSender(SenderAddress)
                && ValidateTransactionIntegrity(participantHandler.GetSenderKey(SenderAddress));

            foreach (var c in chains)
            {
                valid |= c.HasTransaction(TreatmentTransactionAddress);
            }

            return valid;
        }

        public override bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains)
        {
            return ValidateContextual(participantHandler, chains);
        }
    }
}
