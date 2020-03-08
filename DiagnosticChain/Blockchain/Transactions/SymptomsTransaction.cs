using Blockchain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
    class SymptomsTransaction : ITransaction
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
    }
}
