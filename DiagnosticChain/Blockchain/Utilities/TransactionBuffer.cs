using System.Collections.Concurrent;
using Blockchain.Interfaces;

namespace Blockchain.Utilities
{
    //Stores all the transactions and blocks that have not been added to the official chain yet
    public class TransactionBuffer
    {
        public ConcurrentBag<ITransaction> openTransactions = new ConcurrentBag<ITransaction>();
        public ConcurrentBag<Block> unpublishedBlocks = new ConcurrentBag<Block>();

        public void RecordTransaction(ITransaction transaction)
        {
            openTransactions.Add(transaction);
        }

        public void BundleTransactions(object state)
        {
            if (!openTransactions.IsEmpty)
            {
                Block block = new Block();

                while (!openTransactions.IsEmpty)
                {
                    ITransaction nextTransaction;
                    if (openTransactions.TryTake(out nextTransaction))
                    {
                        block.AddTransaction(nextTransaction);
                    }
                }

                unpublishedBlocks.Add(block);
            }
        }

        public Block GetNextBlock()
        {
            Block nextBlock;
            if (unpublishedBlocks.TryTake(out nextBlock))
            {
                return nextBlock;
            }

            return null;
        }

        public bool HasBlocks()
        {
            return !unpublishedBlocks.IsEmpty;
        }
    }
}
