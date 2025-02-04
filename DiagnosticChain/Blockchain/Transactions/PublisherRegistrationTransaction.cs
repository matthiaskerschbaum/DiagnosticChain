﻿using Blockchain.Entities;
using Blockchain.Interfaces;
using Blockchain.Utilities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Transactions
{
    [Serializable]
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

        public override bool ValidateContextual(ParticipantHandler participantHandler, List<Chain> chains)
        {
            return SenderAddress == TransactionId
                && ValidateTransactionIntegrity(PublicKey)
                && !participantHandler.HasPublisher(TransactionId);
        }

        public override bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains)
        {
            if (ValidateContextual(participantHandler, chains))
            {
                participantHandler.ProposePublisher(new Publisher()
                {
                    Address = TransactionId
                    ,
                    PublicKey = PublicKey
                    ,
                    Country = Country
                    ,
                    Region = Region
                    ,
                    EntityName = EntityName
                });

                return true;
            }

            return false;
        }
    }
}
