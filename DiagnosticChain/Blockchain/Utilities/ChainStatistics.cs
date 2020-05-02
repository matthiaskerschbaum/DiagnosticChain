using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Utilities
{
    public class ChainStatistics
    {
        private Chain chain;

        public ChainStatistics(Chain chain)
        {
            this.chain = chain;
        }

        public List<string> GetOverviewPerBlock()
        {
            var ret = new List<string>();
            var blockList = chain.GetBlocks();

            foreach (var b in blockList)
            {
                ret.Add(b.Index + "\t" + b.Timestamp.ToString("yyyy-MM-dd hh:mm:ss") + "\t" + b.TransactionList.Count + " Transactions");
            }

            return ret;
        }

        public List<string> GetOverviewPerTransaction()
        {
            var ret = new List<string>();
            var transactionList = chain.GetTransactions();

            foreach (var t in transactionList)
            {
                ret.Add(t.AsString());
            }

            return ret;
        }
    }
}
