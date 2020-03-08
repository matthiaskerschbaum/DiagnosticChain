using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;

namespace Blockchain.Interfaces
{
    public abstract class ITransaction
    {
        public string Type { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid SenderAddress { get; set; }
        public string SenderVerification { get; set; } //Hash of all the data in the object

        public string AsXML()
        {
            //TODO Test whether this actually works
            XmlSerializer xsSubmit = new XmlSerializer(this.GetType());
            var subReq = Activator.CreateInstance(this.GetType());
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    xml = sww.ToString();
                }
            }

            return xml;
        }

        public string AsJSON()
        {
            //TODO Test whether this actually works
            return JsonConvert.SerializeObject(this);
        }

        public virtual string AsString()
        {
            return TransactionId + "|" + Timestamp.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "|" + SenderAddress;
        }

        public void Sign(string privateKey)
        {
            //TODO implement encryption with private key
            SenderVerification = this.AsString();
        }

        public bool Validate(string publicKey)
        {
            //TODO decrypt SenderVerification and compare to string representation
            return SenderVerification == this.AsString();
        }
    }
}
