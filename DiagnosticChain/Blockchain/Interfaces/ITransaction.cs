using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using Shared;
using Blockchain.Transactions;
using System.Collections.Generic;
using Shared.Interfaces;
using Blockchain.Utilities;

namespace Blockchain.Interfaces
{
    [XmlInclude(typeof(DiagnosesTransaction))
            , XmlInclude(typeof(PatientRegistrationTransaction))
            , XmlInclude(typeof(PhysicianRegistrationTransaction))
            , XmlInclude(typeof(PublisherRegistrationTransaction))
            , XmlInclude(typeof(SymptomsTransaction))
            , XmlInclude(typeof(TreatmentTransaction))
            , XmlInclude(typeof(VotingTransaction))]
    public abstract class ITransaction : WebSerializable
    {
        public TransactionType Type { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid SenderAddress { get; set; }
        public string SenderVerification { get; set; }

        internal virtual string AsString()
        {
            return Type + "|" + TransactionId + "|" + Timestamp.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "|" + SenderAddress;
        }

        public void Sign(RSAParameters privateKey)
        {
            SenderVerification = EncryptionHandler.Sign(AsString(), privateKey);
        }

        public bool ValidateTransactionIntegrity(RSAParameters publicKey)
        {
            return EncryptionHandler.VerifiySignature(AsString(), SenderVerification, publicKey);
        }

        public abstract bool ValidateContextual(ParticipantHandler participantHandler, List<Chain> chains);

        public abstract bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains);
    }
}
