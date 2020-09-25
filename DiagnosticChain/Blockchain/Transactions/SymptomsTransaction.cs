using Blockchain.Interfaces;
using Blockchain.Utilities;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
    [Serializable]
    public class SymptomsTransaction : ITransaction
    {
        public Guid TreatmentTransactionAddress { get; set; }
        public List<string> Symptoms { get; set; }

        public override string AsString()
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
                valid |= participantHandler.HasParkedTreatment(TreatmentTransactionAddress);
            }

            return valid;
        }

        public override bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains)
        {
            if (ValidateContextual(participantHandler, chains))
            {
                var l = 5; //Count of distinct Symtpoms for the corresponding patient over all chains

                if (l < 5)
                {
                    participantHandler.ParkSymptom(this);

                    throw new TransactionParkedException();
                }

                return true;
            }

            return false;
        }
    }
}
