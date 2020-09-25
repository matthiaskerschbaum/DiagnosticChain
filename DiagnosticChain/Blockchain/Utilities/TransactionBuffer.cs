using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Blockchain.Interfaces;

namespace Blockchain.Utilities
{
    //Stores all the transactions and blocks that have not been added to the official chain yet
    public class TransactionBuffer
    {
        public List<ITransaction> openTransactions = new List<ITransaction>();
        public List<Block> unpublishedBlocks = new List<Block>();

        public void BundleTransactions(object state)
        {
            if (openTransactions.Count > 0)
            {
                Block block = new Block();

                block.TransactionList = openTransactions;
                openTransactions = new List<ITransaction>();

                unpublishedBlocks.Add(block);
            }
        }

        public Block GetNextBlock()
        {
            if (unpublishedBlocks.Count > 0)
            {
                var ret = unpublishedBlocks[0];
                unpublishedBlocks.RemoveAt(0);
                return ret;
            }

            return null;
        }

        public bool HasBlocks()
        {
            return unpublishedBlocks.Count > 0;
        }

        public Chain Peek()
        {
            Chain ret = new Chain();
            Block peekTransactions = new Block();

            foreach (var b in unpublishedBlocks)
            {
                foreach (var t in b.TransactionList)
                {
                    peekTransactions.AddTransaction(t);
                }
            }

            foreach (var t in openTransactions)
            {
                peekTransactions.AddTransaction(t);
            }

            ret.Add(peekTransactions);
            return ret;
        }

        public void RecordTransaction(ITransaction transaction)
        {
            openTransactions.Add(transaction);
        }

        public void UnrecordTransaction(ITransaction t)
        {
            openTransactions.Remove(t);
        }
    }
}
