using Blockchain.Entities;
using Blockchain.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Transactions
{
    public class PublisherRegistrationTransaction : ITransaction
    {
        public RSAParameters PublicKey { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string EntityName { get; set; }

        public override string AsString()
        {
            return base.AsString() + "|" + PublicKey + "|" + Country + "|" + Region + "|" + EntityName;
        }

        internal override bool HandleContextual(ParticipantHandler participantHandler, List<Chain> chains)
        {
            participantHandler.ProposePublisher(new Publisher()
            {
                Address = TransactionId
                ,PublicKey = PublicKey
                ,Country = Country
                ,Region = Region
                ,EntityName = EntityName
            });

            return true;
        }
    }
}
