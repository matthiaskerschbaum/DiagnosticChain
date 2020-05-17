using Blockchain.Interfaces;
using Blockchain.Transactions;
using Blockchain.Utilities;
using Newtonsoft.Json;
using Shared;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Blockchain
{
    public class Block : WebSerializable
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

        public void AddTransaction(ITransaction transaction)
        {
            TransactionList.Add(transaction);
            TransactionList.Sort((x, y) => x.Timestamp.CompareTo(y.Timestamp));
        }

        public string AsString(bool includeHash)
        {
            var ret = Index + "||"
                      + Timestamp.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "||"
                      + (includeHash ? Hash + "||" : "")
                      + (includeHash ? PreviousHash + "||" : "");

            foreach (var t in TransactionList)
            {
                ret += t.AsString() + "||";
            }

            ret += Publisher; //PublisherVerification is not part of the string, because it is calculated after hashing

            return ret;
        }

        internal bool HasTransaction(Guid address)
        {
            var hit = from transaction in TransactionList
                      where transaction.TransactionId == address
                      select transaction;

            return hit.Count() > 0;
        }

        public bool ProcessContracts(ParticipantHandler participantHandler, List<Chain> chains)
        {
            var ret = true;

            foreach (var t in TransactionList)
            {
                ret &= t.ProcessContract(participantHandler, chains);
            }

            return ret;
        }

        public void RecalculateHash()
        {
            Hash = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.Unicode.GetBytes(AsString(includeHash: false))));
        }

        public void Sign(RSAParameters privateKey)
        {
            Timestamp = DateTime.UtcNow;
            RecalculateHash();
            PublisherVerification = EncryptionHandler.Sign(AsString(includeHash: true), privateKey);
        }

        public bool ValidateBlockIntegrity(RSAParameters? signerPublicKey)
        {
            if (signerPublicKey == null) return false;

            var ret = true;
            ret &= EncryptionHandler.VerifiySignature(AsString(includeHash: true), PublisherVerification, signerPublicKey.Value);
            ret &= ValidateSequence();

            return ret;
        }

        public bool ValidateContextual(ParticipantHandler participantHandler, List<Chain> context)
        {
            var blockIsValid = true;

            //Handle initializing block
            if (Index == 0) 
            {
                blockIsValid &= TransactionList.Count() == 2;

                var registration = (from t in TransactionList
                                    where t.GetType() == typeof(PublisherRegistrationTransaction)
                                    select (PublisherRegistrationTransaction)t).FirstOrDefault();
                var vote = (from t in TransactionList
                            where t.GetType() == typeof(VotingTransaction)
                            select (VotingTransaction)t).FirstOrDefault();

                blockIsValid &= registration != null && vote != null;
                blockIsValid &= ValidateBlockIntegrity(registration.PublicKey);
                blockIsValid &= participantHandler.ProcessTransaction(registration, context);
                blockIsValid &= participantHandler.ProcessTransaction(vote, context);
            }
            //Handle new Publisher registration
            else if (TransactionList.Count() == 1 && TransactionList[0].GetType() == typeof(PublisherRegistrationTransaction)) 
            {
                var registration = (PublisherRegistrationTransaction)TransactionList[0];

                blockIsValid &= ValidateBlockIntegrity(registration.PublicKey);
                blockIsValid &= participantHandler.ProcessTransaction(registration, context);
            }
            //Handle regular block
            else if (participantHandler.HasPublisher(Publisher)) 
            {
                blockIsValid &= ValidateBlockIntegrity(participantHandler.GetPublisherKey(Publisher));
                
                foreach (var t in TransactionList)
                {
                    if (t.GetType() == typeof(PhysicianRegistrationTransaction))
                    {
                        PhysicianRegistrationTransaction temp = (PhysicianRegistrationTransaction)t;
                        blockIsValid &= temp.ValidateTransactionIntegrity(temp.PublicKey);
                        blockIsValid &= participantHandler.ProcessTransaction(temp, context);
                    }
                    else if (participantHandler.HasSender(t.SenderAddress))
                    {
                        blockIsValid &= t.ValidateTransactionIntegrity(participantHandler.GetSenderKey(t.SenderAddress));
                        blockIsValid &= participantHandler.ProcessTransaction(t, context);
                    }
                    else
                    {
                        blockIsValid = false;
                    }
                }
            }
            //Fallback - Block definitely invalid
            else
            {
                blockIsValid = false;
            }

            return blockIsValid;
        }

        public bool ValidateSequence()
        {
            return PreviousBlock != null ? (PreviousHash == PreviousBlock.Hash && Index == PreviousBlock.Index+1) : true;
        }
    }
}
