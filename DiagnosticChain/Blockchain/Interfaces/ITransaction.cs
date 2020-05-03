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

        internal void Sign(RSAParameters privateKey)
        {
            FileHandler.Log("Signing transaction #" + TransactionId);
            FileHandler.Log("With key: " + privateKey.AsString());
            SenderVerification = EncryptionHandler.Sign(AsString(), privateKey);
        }

        internal bool ValidateTransactionIntegrity(RSAParameters publicKey)
        {
            FileHandler.Log("Validating transaction #" + TransactionId);
            FileHandler.Log("With key: " + publicKey.AsString());
            return EncryptionHandler.VerifiySignature(AsString(), SenderVerification, publicKey);
        }

        internal abstract bool ProcessContract(ParticipantHandler participantHandler, List<Chain> chains);
    }
}
