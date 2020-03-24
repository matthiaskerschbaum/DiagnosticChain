using Blockchain.Interfaces;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Blockchain
{
    public class Block
    {
        public long Index { get; set; }
        public DateTime Timestamp { get; set; }
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public Block PreviousBlock { get; set; }
        public List<ITransaction> TransactionList { get; set; } = new List<ITransaction>();
        public Guid Publisher { get; set; }
        public string PublisherVerification { get; set; }

        public Block() { }

        public Block(Guid publisher)
        {
            this.Publisher = publisher;
        }

       
        public string AsXML()
        {
            XmlSerializer xsSubmit = new XmlSerializer(this.GetType(), new Type[] { typeof(ITransaction) });
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

        public string AsString()
        {
            var ret = Index + "||"
                      + Timestamp.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "||"
                      + Hash + "||"
                      + PreviousHash + "||";

            foreach (var t in TransactionList)
            {
                ret += t.AsString() + "||";
            }

            ret += Publisher; //PublisherVerification is not part of the string, because it is calculated after hashing

            return ret;
        }

        public void AddTransaction(ITransaction transaction)
        {
            TransactionList.Add(transaction);
        }

        public void Sign(RSAParameters privateKey)
        {
            Timestamp = DateTime.UtcNow;
            CalculateHash();
            PublisherVerification = EncryptionHandler.Sign(AsString(), privateKey);
        }

        internal bool HasTransaction(Guid address)
        {
            var hit = from transaction in TransactionList
                      where transaction.TransactionId == address
                      select transaction;

            return hit.Count() > 0;
        }

        public bool ValidateSequence()
        {
            return PreviousBlock != null ? (PreviousHash == PreviousBlock.Hash && Index == PreviousBlock.Index+1) : true;
        }

        public bool Validate(RSAParameters publicKey)
        {
            return EncryptionHandler.VerifiySignature(AsString(), PublisherVerification, publicKey) && ValidateSequence();
        }

        public void CalculateHash()
        {
            Hash = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.Unicode.GetBytes(AsString())));
        }
    }
}
