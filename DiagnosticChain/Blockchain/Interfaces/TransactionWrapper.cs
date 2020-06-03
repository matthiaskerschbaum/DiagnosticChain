using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Interfaces
{
    public class TransactionWrapper : WebSerializable
    {
        public ITransaction Transaction;

        public TransactionWrapper()
        {

        }
        public TransactionWrapper(ITransaction transaction)
        {
            Transaction = transaction;
        }
    }
}
