using Blockchain.Entities;
using Blockchain.Interfaces;
using Blockchain.Utilities;
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

        internal override string AsString()
        {
            return base.AsString() + "|" + PublicKey + "|" + Country + "|" + Region + "|" + Name;
        }

        public override bool ValidateContextual(ParticipantHandler participantHandler, List<Chain> chains)
        {
            return SenderAddress == TransactionId
                && ValidateTransactionIntegrity(PublicKey)
                && !participantHandler.HasPhysician(TransactionId);
        }

        public override bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains)
        {
            if (ValidateContextual(participantHandler, chains))
            {
                participantHandler.ProposePhysician(new Physician()
                {
                    Address = TransactionId
                    ,
                    PublicKey = PublicKey
                    ,
                    Country = Country
                    ,
                    Region = Region
                    ,
                    Name = Name
                });

                return true;
            }

            return false;
        }
    }
}
