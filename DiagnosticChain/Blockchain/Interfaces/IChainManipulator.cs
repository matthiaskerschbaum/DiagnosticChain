using Blockchain.Entities;
using Blockchain.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Interfaces
{
    public interface IChainManipulator
    {
        Chain GetChain();
        Chain GetChainDelta(long currentIndex);
        List<Physician> GetPendingPhysicians();
        bool OnReceiveChain(Chain chain);
        bool OnReceiveTransaction(ITransaction transaction);
    }
}
