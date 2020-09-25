using Blockchain.Entities;
using Blockchain.Interfaces;
using Blockchain.Utilities;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
    [Serializable]
    public class PatientRegistrationTransaction : ITransaction
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string Birthyear { get; set; }

        public override string AsString()
        {
            return base.AsString() + "|" + Country + "|" + Region + "|" + Birthyear;
        }

        public override bool ValidateContextual(ParticipantHandler participantHandler, List<Chain> chains)
        {
            var ret = participantHandler.HasSender(SenderAddress)
                && ValidateTransactionIntegrity(participantHandler.GetSenderKey(SenderAddress));

            ret &= !participantHandler.HasPatient(TransactionId);

            return ret;
        }

        public override bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains)
        {
            if (ValidateContextual(participantHandler, chains))
            {
                var k = participantHandler.CountSimilarPatients(Country, Region, Birthyear);

                if (k < 3)
                {
                    throw new TransactionParkedException();
                }
                else
                {

                    participantHandler.AddPatient(new Patient()
                    {
                        Address = TransactionId
                        ,
                        Country = Country
                        ,
                        Region = Region
                        ,
                        Birthyear = Birthyear
                    });

                    return true;
                }
            }

            return false;
        }
    }
}
