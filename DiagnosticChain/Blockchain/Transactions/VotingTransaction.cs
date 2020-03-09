using Blockchain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
    public class VotingTransaction : ITransaction
    {
        public Guid TransactionAddress { get; set; }
        public bool Vote { get; set; }

        public override string AsString()
        {
            return base.AsString() + "|" + TransactionAddress + "|" + Vote;
        }
    }
}
