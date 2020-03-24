using Blockchain.Entities;
using Blockchain.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Transactions
{
    public class PhysicianRegistrationTransaction : ITransaction
    {
        public RSAParameters PublicKey { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }

        public override string AsString()
        {
            return base.AsString() + "|" + PublicKey + "|" + Country + "|" + Region + "|" + Name;
        }

        internal override bool HandleContextual(ParticipantHandler participantHandler, List<Chain> chains)
        {
            if (SenderAddress != TransactionId)
            {
                return false;
            }

            participantHandler.ProposePhysician(new Physician()
            { 
                Address = TransactionId
                ,PublicKey = PublicKey
                ,Country = Country
                ,Region = Region
                ,Name = Name
            });

            return true;
        }
    }
}
