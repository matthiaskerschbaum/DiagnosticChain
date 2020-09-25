using Blockchain.Interfaces;
using Blockchain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
    [Serializable]
    public class TreatmentTransaction : ITransaction
    {
        public Guid PhysicianAddress { get; set; }
        public Guid PatientAddress { get; set; }

        public override string AsString()
        {
            return base.AsString() + "|" + PhysicianAddress + "|" + PatientAddress;
        }

        public override bool ValidateContextual(ParticipantHandler participantHandler, List<Chain> chains)
        {
            var ret = participantHandler.HasSender(SenderAddress)
                && ValidateTransactionIntegrity(participantHandler.GetSenderKey(SenderAddress));

            ret &= participantHandler.HasSender(PhysicianAddress) && participantHandler.HasPatient(PatientAddress);

            return ret;
        }

        public override bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains)
        {
            return ValidateContextual(participantHandler, chains);
        }
    }
}
