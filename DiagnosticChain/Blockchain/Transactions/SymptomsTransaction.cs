using Blockchain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
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

        internal override bool HandleContextual(ParticipantHandler participantHandler, List<Chain> chains)
        {
            var valid = false;

            foreach (var c in chains)
            {
                valid |= c.HasTransaction(TreatmentTransactionAddress);
            }

            return valid;
        }
    }
}
