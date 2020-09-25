using Blockchain;
using Blockchain.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using System;
using System.Collections.Generic;
using static Shared.EncryptionHandler;

namespace TestProjekt
{
    [TestClass]
    public class BasicUnitTests
    {
        [TestMethod]
        public void TransactionGeneration()
        {
            var keys = EncryptionHandler.GenerateNewKeys();
            TransactionGenerator transactionGenerator = new TransactionGenerator(keys.PrivateKey);
            var registration = transactionGenerator.InitializeAsNewPublisher(keys.PublicKey, "Austria", "Vienna", "Master");

            Assert.IsTrue(registration.ValidateTransactionIntegrity(keys.PublicKey));
        }

        [TestMethod]
        public void BlockGeneration()
        {
            Block block = new Block();
            block.Index = 0;
            block.Publisher = Guid.NewGuid();

            var keys = EncryptionHandler.GenerateNewKeys();
            block.Sign(keys.PrivateKey);

            Assert.IsTrue(block.ValidateBlockIntegrity(keys.PublicKey));
        }

        [TestMethod]
        public void AddTransactionToBlock()
        {
            var keys = EncryptionHandler.GenerateNewKeys();

            TransactionGenerator transactionGenerator = new TransactionGenerator(keys.PrivateKey);
            var registration = transactionGenerator.InitializeAsNewPublisher(keys.PublicKey, "Austria", "Vienna", "Master");

            Block block = new Block();
            block.Index = 0;
            block.Publisher = registration.TransactionId;

            block.AddTransaction(registration);
            Assert.IsTrue(block.TransactionList.Count == 1);

            block.AddTransaction(registration);
            Assert.IsTrue(block.TransactionList.Count == 2);

            block.Sign(keys.PrivateKey);
            Assert.IsTrue(block.ValidateBlockIntegrity(keys.PublicKey));
        }

        [TestMethod]
        public void AddBlockToChain()
        {
            var keys = EncryptionHandler.GenerateNewKeys();

            TransactionGenerator transactionGenerator = new TransactionGenerator(keys.PrivateKey);
            var registration = transactionGenerator.InitializeAsNewPublisher(keys.PublicKey, "Austria", "Vienna", "Master");
            var vote = transactionGenerator.GenerateVotingTransaction(registration.TransactionId, true);

            Block block = new Block();
            block.Index = 0;
            block.Publisher = registration.TransactionId;

            block.AddTransaction(registration);
            block.AddTransaction(vote);
            block.Sign(keys.PrivateKey);

            Chain chain = new Chain();
            chain.Add(block);

            Assert.IsTrue(chain.ValidateContextual(new ParticipantHandler(), null));
        }
    }
}
