using Blockchain.Interfaces;
using Blockchain.Utilities;
using NodeManagement;
using NodeManagement.Entities;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace DiagnosticChain.Controllers
{
    class PublisherController
    {
        private string currentUser = "";
        private Node node;
        private TransactionGenerator transactionGenerator;

        public PublisherController()
        {
            node = new Node();
        }

        public PublisherController(string currentUser) : this()
        {
            this.currentUser = currentUser;
        }

        internal bool HasSavedState()
        {
            return File.Exists(currentUser + FileHandler.StatePath);
        }

        internal void LoadState()
        {
            if (HasSavedState())
            {
                var state = FileHandler.Read(currentUser + FileHandler.StatePath);
                XmlSerializer serializer = new XmlSerializer(node.GetType());
                node = (Node)serializer.Deserialize(new StringReader(state));
            }
        }

        internal void SetupNewPublisher(string country, string region, string entityName)
        {
            var keys = EncryptionHandler.GenerateNewKeys();
            transactionGenerator = new TransactionGenerator(keys.PrivateKey);
            ITransaction registration = transactionGenerator.InitializeAsNewPublisher(keys.PublicKey, country, region, entityName);

            node.User = new UserProperties()
            {
                Username = currentUser
                ,
                Keys = keys
                ,
                PublisherAddress = registration.TransactionId
            };

            if (!node.IsChainInitialized())
            {
                ITransaction vote = transactionGenerator.GenerateVotingTransaction(registration.TransactionId, true);
                node.RequestChainInitialization(registration, vote);
            } else
            {
                node.OnReveiceTransaction(registration);
            }
        }

        internal void StartPublishing()
        {
            node.StartPublishing();
        }
    }
}
