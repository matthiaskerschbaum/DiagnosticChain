using Blockchain.Interfaces;
using Blockchain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
    public class DiagnosesTransaction : ITransaction
    {
        public Guid TreatmentTransactionAddress { get; set; }
        public List<string> Diagnoses { get; set; }

        internal override string AsString()
        {
            var ret = base.AsString() + "|" + TreatmentTransactionAddress;

            foreach (var d in Diagnoses)
            {
                ret += "|" + d;
            }

            return ret;
        }

        internal override bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains)
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
