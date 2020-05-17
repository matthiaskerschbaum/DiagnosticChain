using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Interfaces
{
    public interface IChainManipulator
    {
        Chain GetChain();
        Chain GetChainDelta(long currentIndex);
        bool OnReceiveChain(Chain chain);
        void OnReceiveTransaction(ITransaction transaction);
    }
}
