using Blockchain;
using Blockchain.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeManagement;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProjekt
{
    [TestClass]
    public class CriticalTests
    {
        [TestMethod]
        public void ConsolidateChains()
        {
            #region Set up Publisher A
            var keysA = EncryptionHandler.GenerateNewKeys();

            TransactionGenerator transactionGeneratorA = new TransactionGenerator(keysA.PrivateKey);
            var registrationA = transactionGeneratorA.InitializeAsNewPublisher(keysA.PublicKey, "Austria", "Vienna", "PublisherA");
            var voteA = transactionGeneratorA.GenerateVotingTransaction(registrationA.TransactionId, true);

            PublisherNode publisherA = new PublisherNode();
            publisherA.User = new NodeManagement.Entities.UserProperties()
            {
                Keys = keysA,
                UserAddress = registrationA.TransactionId,
                Username = "PublisherA"
            };

            publisherA.InitializeEmptyChain(registrationA, voteA);
            #endregion

            #region Set up Publisher B
            var keysB = EncryptionHandler.GenerateNewKeys();

            TransactionGenerator transactionGeneratorB = new TransactionGenerator(keysB.PrivateKey);
            var registrationB = transactionGeneratorB.InitializeAsNewPublisher(keysB.PublicKey, "Austria", "Vienna", "PublisherB");

            PublisherNode publisherB = new PublisherNode();
            publisherB.User = new NodeManagement.Entities.UserProperties()
            {
                Keys = keysB,
                UserAddress = registrationB.TransactionId,
                Username = "PublisherB"
            };

            publisherB.OnReceiveChain(publisherA.GetChain());

            publisherB.OnReceiveTransaction(registrationB);
            publisherB.BundleOpenTransactionsManually();
            publisherB.PublishOpenBlocks(publisherB);
            #endregion

            #region Authorize Publisher B
            publisherA.OnReceiveChain(publisherB.GetChainDelta(0));

            Assert.AreEqual(publisherA.GetChain().AsXML(), publisherB.GetChain().AsXML());

            var voteB = transactionGeneratorA.GenerateVotingTransaction(registrationB.TransactionId, true);
            publisherA.OnReceiveTransaction(voteB);
            publisherA.BundleOpenTransactionsManually();
            publisherA.PublishOpenBlocks(publisherA);
            publisherB.OnReceiveChain(publisherA.GetChainDelta(1));

            Assert.AreEqual(publisherA.GetChain().AsXML(), publisherB.GetChain().AsXML());
            #endregion

            var transactionA = transactionGeneratorA.GenerateVotingTransaction(registrationB.TransactionId, true);
            var transactionB = transactionGeneratorB.GenerateVotingTransaction(registrationA.TransactionId, true);
            var transactionC = transactionGeneratorB.GenerateVotingTransaction(registrationA.TransactionId, true);

            #region Send transactions to publishers
            publisherA.OnReceiveTransaction(transactionA);
            publisherA.BundleOpenTransactionsManually();
            publisherA.PublishOpenBlocks(publisherA);

            publisherB.OnReceiveTransaction(transactionB);
            publisherB.BundleOpenTransactionsManually();
            publisherB.PublishOpenBlocks(publisherB);

            publisherB.OnReceiveTransaction(transactionC);
            publisherB.BundleOpenTransactionsManually();
            publisherB.PublishOpenBlocks(publisherB);
            #endregion

            publisherA.OnReceiveChain(publisherB.GetChainDelta(2));

            var hit = from t in publisherA.GetChain().GetTransactions()
                      where t.TransactionId == transactionA.TransactionId
                      select t;
            Assert.AreEqual(hit.Count(), 0);

            publisherA.BundleOpenTransactionsManually();
            publisherA.PublishOpenBlocks(publisherA);

            hit = from t in publisherA.GetChain().GetTransactions()
                  where t.TransactionId == transactionA.TransactionId
                  select t;
            Assert.AreEqual(1, hit.Count());
        }

        [TestMethod]
        public void RegistrateAndVotePublisher()
        {
            #region Set up Publisher A
            var keysA = EncryptionHandler.GenerateNewKeys();

            TransactionGenerator transactionGeneratorA = new TransactionGenerator(keysA.PrivateKey);
            var registrationA = transactionGeneratorA.InitializeAsNewPublisher(keysA.PublicKey, "Austria", "Vienna", "PublisherA");
            var voteA = transactionGeneratorA.GenerateVotingTransaction(registrationA.TransactionId, true);

            PublisherNode publisherA = new PublisherNode();
            publisherA.User = new NodeManagement.Entities.UserProperties()
            {
                Keys = keysA,
                UserAddress = registrationA.TransactionId,
                Username = "PublisherA"
            };

            publisherA.InitializeEmptyChain(registrationA, voteA);

            Assert.IsTrue(publisherA.participantHandler.HasPublisher(registrationA.TransactionId));
            #endregion

            #region Set up Publisher B
            var keysB = EncryptionHandler.GenerateNewKeys();

            TransactionGenerator transactionGeneratorB = new TransactionGenerator(keysB.PrivateKey);
            var registrationB = transactionGeneratorB.InitializeAsNewPublisher(keysB.PublicKey, "Austria", "Vienna", "PublisherB");

            PublisherNode publisherB = new PublisherNode();
            publisherB.User = new NodeManagement.Entities.UserProperties()
            {
                Keys = keysB,
                UserAddress = registrationB.TransactionId,
                Username = "PublisherB"
            };

            publisherB.OnReceiveChain(publisherA.GetChain());

            publisherB.OnReceiveTransaction(registrationB);
            publisherB.BundleOpenTransactionsManually();
            publisherB.PublishOpenBlocks(publisherB);
            #endregion

            Assert.IsFalse(publisherA.participantHandler.HasPublisher(registrationB.TransactionId));
            Assert.IsFalse(publisherB.participantHandler.HasPublisher(registrationB.TransactionId));

            #region Authorize Publisher B
            publisherA.OnReceiveChain(publisherB.GetChainDelta(0));

            Assert.AreEqual(publisherA.GetChain().AsXML(), publisherB.GetChain().AsXML());

            var voteB = transactionGeneratorA.GenerateVotingTransaction(registrationB.TransactionId, true);
            publisherA.OnReceiveTransaction(voteB);
            publisherA.BundleOpenTransactionsManually();
            publisherA.PublishOpenBlocks(publisherA);
            publisherB.OnReceiveChain(publisherA.GetChainDelta(1));

            Assert.AreEqual(publisherA.GetChain().AsXML(), publisherB.GetChain().AsXML());
            #endregion

            Assert.IsTrue(publisherA.participantHandler.HasPublisher(registrationB.TransactionId));
            Assert.IsTrue(publisherB.participantHandler.HasPublisher(registrationB.TransactionId));
        }

        [TestMethod]
        public void RegistrateAndVotePhysician()
        {
            #region Set up Publisher
            var keysPublisher = EncryptionHandler.GenerateNewKeys();

            TransactionGenerator transactionGeneratorPublisher = new TransactionGenerator(keysPublisher.PrivateKey);
            var registrationPublisher = transactionGeneratorPublisher.InitializeAsNewPublisher(keysPublisher.PublicKey, "Austria", "Vienna", "Publisher");
            var votePublisher = transactionGeneratorPublisher.GenerateVotingTransaction(registrationPublisher.TransactionId, true);

            PublisherNode publisher = new PublisherNode();
            publisher.User = new NodeManagement.Entities.UserProperties()
            {
                Keys = keysPublisher,
                UserAddress = registrationPublisher.TransactionId,
                Username = "Publisher"
            };

            publisher.InitializeEmptyChain(registrationPublisher, votePublisher);

            Assert.IsTrue(publisher.participantHandler.HasPublisher(registrationPublisher.TransactionId));
            #endregion

            #region Generate registration for physician
            var keysPhysician = EncryptionHandler.GenerateNewKeys();

            TransactionGenerator transactionGeneratorPhyisician = new TransactionGenerator(keysPhysician.PrivateKey);
            var registrationPhysician = transactionGeneratorPhyisician.InitializeAsNewPhysician(keysPhysician.PublicKey, "Austria", "Vienna", "Physician");
            var votePhysician = transactionGeneratorPublisher.GenerateVotingTransaction(registrationPhysician.TransactionId, true);
            #endregion

            publisher.OnReceiveTransaction(registrationPhysician);

            Assert.IsFalse(publisher.participantHandler.HasPhysician(registrationPhysician.TransactionId));

            publisher.OnReceiveTransaction(votePhysician);

            publisher.BundleOpenTransactionsManually();
            publisher.PublishOpenBlocks(publisher);

            Assert.IsTrue(publisher.participantHandler.HasPhysician(registrationPhysician.TransactionId));
        }

        [TestMethod]
        public void ProcessPatientData()
        {
            #region Set up Publisher
            var keysPublisher = EncryptionHandler.GenerateNewKeys();

            TransactionGenerator transactionGeneratorPublisher = new TransactionGenerator(keysPublisher.PrivateKey);
            var registrationPublisher = transactionGeneratorPublisher.InitializeAsNewPublisher(keysPublisher.PublicKey, "Austria", "Vienna", "Publisher");
            var votePublisher = transactionGeneratorPublisher.GenerateVotingTransaction(registrationPublisher.TransactionId, true);

            PublisherNode publisher = new PublisherNode();
            publisher.User = new NodeManagement.Entities.UserProperties()
            {
                Keys = keysPublisher,
                UserAddress = registrationPublisher.TransactionId,
                Username = "Publisher"
            };

            publisher.InitializeEmptyChain(registrationPublisher, votePublisher);
            Assert.IsTrue(publisher.participantHandler.HasPublisher(registrationPublisher.TransactionId));
            #endregion

            #region Register physician
            var keysPhysician = EncryptionHandler.GenerateNewKeys();

            TransactionGenerator transactionGeneratorPhyisician = new TransactionGenerator(keysPhysician.PrivateKey);
            var registrationPhysician = transactionGeneratorPhyisician.InitializeAsNewPhysician(keysPhysician.PublicKey, "Austria", "Vienna", "Physician");
            var votePhysician = transactionGeneratorPublisher.GenerateVotingTransaction(registrationPhysician.TransactionId, true);

            publisher.OnReceiveTransaction(registrationPhysician);
            publisher.OnReceiveTransaction(votePhysician);

            publisher.BundleOpenTransactionsManually();
            publisher.PublishOpenBlocks(publisher);

            Assert.IsTrue(publisher.participantHandler.HasPhysician(registrationPhysician.TransactionId));
            #endregion

            var patientA = transactionGeneratorPhyisician.GeneratePatientRegistrationTransaction("Austria", "Vienna", "1990");

            publisher.OnReceiveTransaction(patientA);

            Assert.IsTrue(publisher.participantHandler.parkedPatients.Contains(patientA));
            Assert.IsFalse(publisher.participantHandler.HasPatient(patientA.TransactionId));

            publisher.BundleOpenTransactionsManually();
            publisher.PublishOpenBlocks(publisher);

            Assert.IsTrue(publisher.participantHandler.parkedPatients.Contains(patientA));
            Assert.IsFalse(publisher.participantHandler.HasPatient(patientA.TransactionId));

            for (int i = 0; i < 2; i++)
            {
                publisher.OnReceiveTransaction(
                    transactionGeneratorPhyisician.GeneratePatientRegistrationTransaction("Austria", "Vienna", "1990")
                    );
            }

            Assert.IsTrue(publisher.participantHandler.parkedPatients.Contains(patientA));
            Assert.IsFalse(publisher.participantHandler.HasPatient(patientA.TransactionId));

            publisher.EvaluateParkedTransactions();
            publisher.BundleOpenTransactionsManually();
            publisher.PublishOpenBlocks(publisher);

            Assert.IsFalse(publisher.participantHandler.parkedPatients.Contains(patientA));
            Assert.IsTrue(publisher.participantHandler.HasPatient(patientA.TransactionId));
        }
    }
}
