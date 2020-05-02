using Blockchain.Interfaces;
using Blockchain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Transactions
{
    public class TreatmentTransaction : ITransaction
    {
        public Guid PhysicianAddress { get; set; }
        public Guid PatientAddress { get; set; }

        public override string AsString()
        {
            return base.AsString() + "|" + PhysicianAddress + "|" + PatientAddress;
        }

        internal override bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains)
        {
            return participantHandler.HasSender(PhysicianAddress) && participantHandler.HasPatient(PatientAddress);
        }
    }
}
