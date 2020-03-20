using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using Shared;
using Blockchain.Transactions;

namespace Blockchain.Interfaces
{
    [XmlInclude(typeof(DiagnosesTransaction))
            , XmlInclude(typeof(PatientRegistrationTransaction))
            , XmlInclude(typeof(PhysicianRegistrationTransaction))
            , XmlInclude(typeof(PublisherRegistrationTransaction))
            , XmlInclude(typeof(SymptomsTransaction))
            , XmlInclude(typeof(TreatmentTransaction))
            , XmlInclude(typeof(VotingTransaction))]
    public abstract class ITransaction
    {
        public TransactionType Type { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid SenderAddress { get; set; }
        public string SenderVerification { get; set; }

        public string AsXML()
        {
            XmlSerializer xsSubmit = new XmlSerializer(this.GetType());
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, this);
                    xml = sww.ToString();
                }
            }

            return xml;
        }

        public string AsJSON()
        {
            return JsonConvert.SerializeObject(this);
        }

        public virtual string AsString()
        {
            return Type + "|" + TransactionId + "|" + Timestamp.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "|" + SenderAddress;
        }

        public void Sign(RSAParameters privateKey)
        {
            SenderVerification = EncryptionHandler.Sign(AsString(), privateKey);
        }

        public bool Validate(RSAParameters publicKey)
        {
            return EncryptionHandler.VerifiySignature(AsString(), SenderVerification, publicKey);
        }
    }
}
