using Blockchain.Entities;
using Blockchain.Interfaces;
using Blockchain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
    public class PatientRegistrationTransaction : ITransaction
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string Birthyear { get; set; }

        internal override string AsString()
        {
            return base.AsString() + "|" + Country + "|" + Region + "|" + Birthyear;
        }

        internal override bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains)
        {
            if (participantHandler.HasPatient(TransactionId))
            {
                return false;
            }

            participantHandler.AddPatient(new Patient()
            {
                Address = TransactionId
                ,Country = Country
                ,Region = Region
                ,Birthyear = Birthyear
            });

            return true;
        }
    }
}
