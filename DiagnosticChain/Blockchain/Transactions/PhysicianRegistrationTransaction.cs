using Blockchain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
    class PhysicianRegistrationTransaction : ITransaction
    {
        public string PublicKey { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }

        public override string AsString()
        {
            return base.AsString() + "|" + PublicKey + "|" + Country + "|" + Region + "|" + Name;
        }
    }
}
