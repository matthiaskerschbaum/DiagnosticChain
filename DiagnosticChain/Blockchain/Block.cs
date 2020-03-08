using Blockchain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain
{
    public class Block
    {
        public long Index { get; set; }
        public DateTime Timestamp { get; set; }
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
        public Block PreviousBlock { get; set; }
        public List<ITransaction> Transactions { get; set; }
        public Guid Publisher { get; set; }
        public string PublisherVerification { get; set; }

        public string AsXML()
        {
            throw new NotImplementedException();
            //TODO Convert object to xml
        }

        public string AsJSON()
        {
            throw new NotImplementedException();
            //TODO Convert object to json
        }

        public string AsString()
        {
            throw new NotImplementedException();
            //TODO Convert object to string
        }

        public void CalculateHash()
        {
            throw new NotImplementedException();
            //TODO Get String from object, calculate Hash and store it in Hash
        }

        public void AddTransaction(ITransaction transaction)
        {
            Transactions.Add(transaction);
        }

        public void Sign(string privateKey)
        {
            //TODO implement encryption with private key
            PublisherVerification = this.AsString();
        }

        public bool ValidateSequence()
        {
            //TODO decrypt SenderVerification and compare to string representation
            return PreviousBlock != null ? PreviousHash == PreviousBlock.Hash : true;
        }

        public bool Validate(string publicKey)
        {
            //TODO decrypt SenderVerification and compare to string representation
            return PublisherVerification == this.AsString() && ValidateSequence();
        }
    }
}
