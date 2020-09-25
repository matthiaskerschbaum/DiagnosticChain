using Blockchain;
using Blockchain.Interfaces;
using Blockchain.Transactions;
using Blockchain.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeManagement;
using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TestProjekt
{
    [TestClass]
    public class PerformanceTests
    {
        private int maxBlockSize = 100;

        [TestMethod]
        public void MeasureTransactionProcessing()
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

            var referencePhysicianKeys = EncryptionHandler.GenerateNewKeys();
            var referencePhysicianGenerator = new TransactionGenerator(referencePhysicianKeys.PrivateKey);
            var referencePhysician = referencePhysicianGenerator.InitializeAsNewPhysician(referencePhysicianKeys.PrivateKey, "Austria", "Vienna", "Physician");
            var referencePhysicianVote = transactionGeneratorPublisher.GenerateVotingTransaction(referencePhysician.TransactionId, true);

            var referencePatient = referencePhysicianGenerator.GeneratePatientRegistrationTransaction("Austria", "Vienna", "1990");
            var referenceTreatment = referencePhysicianGenerator.GenerateTreatmentTransaction(referencePhysician.TransactionId, referencePatient.TransactionId);

            publisher.OnReceiveTransaction(referencePhysician);
            publisher.OnReceiveTransaction(referencePhysicianVote);
            publisher.OnReceiveTransaction(referencePatient);
            publisher.OnReceiveTransaction(referenceTreatment);

            publisher.BundleOpenTransactionsManually();
            publisher.PublishOpenBlocks(publisher);

            Assert.AreEqual(1, publisher.GetChain().Blockhead.Index);

            Random random = new Random();

            for (int i = 0; i < 50000; i++)
            {
                int choice = random.Next(1,6);

                switch (choice)
                {
                    case 1:
                        var keysNewPhysician = EncryptionHandler.GenerateNewKeys();

                        TransactionGenerator transactionGeneratorNewPhysician = new TransactionGenerator(keysNewPhysician.PrivateKey);
                        var transaction = transactionGeneratorNewPhysician.InitializeAsNewPhysician(keysNewPhysician.PublicKey, "Austria", "Vienna", "Physician");

                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        publisher.OnReceiveTransaction(transaction);
                        stopwatch.Stop();

                        string log = "PHYSICIAN\t"
                            + (i % maxBlockSize)
                            + "\t" + maxBlockSize
                            + "\t" + publisher.GetChain().Blockhead.Index
                            + "\t" + publisher.GetChain().GetTransactions().Count
                            + "\t" + stopwatch.ElapsedMilliseconds
                            + "\t" + GetRAMUsage()
                            + "\n";
                        FileHandler.Append("D:\\DiagnosticChain\\DiagnosticChain\\TransactionLog.txt", log);
                        break;
                    case 2:
                        transaction = referencePhysicianGenerator.GeneratePatientRegistrationTransaction("Austria", "Vienna", "Patient");

                        stopwatch = new Stopwatch();
                        stopwatch.Start();
                        publisher.OnReceiveTransaction(transaction);
                        stopwatch.Stop();

                        log = "PATIENT\t"
                            + (i % maxBlockSize)
                            + "\t" + maxBlockSize
                            + "\t" + publisher.GetChain().Blockhead.Index
                            + "\t" + publisher.GetChain().GetTransactions().Count
                            + "\t" + stopwatch.ElapsedMilliseconds
                            + "\t" + GetRAMUsage()
                            + "\n";
                        FileHandler.Append("D:\\DiagnosticChain\\DiagnosticChain\\TransactionLog.txt", log);
                        break;
                    case 3:
                        var patients = (from x in publisher.GetChain().GetTransactions()
                                       where x.GetType() == typeof(PatientRegistrationTransaction)
                                       select x.TransactionId).ToList();
                        var p = patients.Count() > 1 ? patients[random.Next(1, patients.Count())-1] : referencePatient.TransactionId;
                        transaction = referencePhysicianGenerator.GenerateTreatmentTransaction(referencePhysician.TransactionId, p);

                        stopwatch = new Stopwatch();
                        stopwatch.Start();
                        publisher.OnReceiveTransaction(transaction);
                        stopwatch.Stop();

                        log = "TREATMENT\t"
                            + (i % maxBlockSize)
                            + "\t" + maxBlockSize
                            + "\t" + publisher.GetChain().Blockhead.Index
                            + "\t" + publisher.GetChain().GetTransactions().Count
                            + "\t" + stopwatch.ElapsedMilliseconds
                            + "\t" + GetRAMUsage()
                            + "\n";
                        FileHandler.Append("D:\\DiagnosticChain\\DiagnosticChain\\TransactionLog.txt", log);
                        break;
                    case 4:
                        var treatments = (from x in publisher.GetChain().GetTransactions()
                                          where x.GetType() == typeof(TreatmentTransaction)
                                          select x.TransactionId).ToList();
                        var t = treatments.Count() > 1 ? treatments[random.Next(1, treatments.Count())-1] : referenceTreatment.TransactionId;
                        transaction = referencePhysicianGenerator.GenerateSymptomTransaction(t, new List<string>() { "Symptom" });

                        stopwatch = new Stopwatch();
                        stopwatch.Start();
                        publisher.OnReceiveTransaction(transaction);
                        stopwatch.Stop();

                        log = "SYMPTOM\t"
                            + (i % maxBlockSize)
                            + "\t" + maxBlockSize
                            + "\t" + publisher.GetChain().Blockhead.Index
                            + "\t" + publisher.GetChain().GetTransactions().Count
                            + "\t" + stopwatch.ElapsedMilliseconds
                            + "\t" + GetRAMUsage()
                            + "\n";
                        FileHandler.Append("D:\\DiagnosticChain\\DiagnosticChain\\TransactionLog.txt", log);
                        break;
                    case 5:
                        transaction = transactionGeneratorPublisher.GenerateVotingTransaction(referencePhysician.TransactionId, true);

                        stopwatch = new Stopwatch();
                        stopwatch.Start();
                        publisher.OnReceiveTransaction(transaction);
                        stopwatch.Stop();

                        log = "VOTING\t"
                            + (i % maxBlockSize)
                            + "\t" + maxBlockSize
                            + "\t" + publisher.GetChain().Blockhead.Index
                            + "\t" + publisher.GetChain().GetTransactions().Count
                            + "\t" + stopwatch.ElapsedMilliseconds
                            + "\t" + GetRAMUsage()
                            + "\n";
                        FileHandler.Append("D:\\DiagnosticChain\\DiagnosticChain\\TransactionLog.txt", log);
                        break;
                    default:
                        break;
                }

                publisher.participantHandler.EvaluateParkedTransactions();

                if (i%maxBlockSize == 0)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    publisher.BundleOpenTransactionsManually();
                    publisher.PublishOpenBlocks(publisher);
                    stopwatch.Stop();

                    string log = "BLOCK\t"
                        + (i % maxBlockSize)
                        + "\t" + maxBlockSize
                        + "\t" + publisher.GetChain().Blockhead.Index
                        + "\t" + publisher.GetChain().GetTransactions().Count
                        + "\t" + stopwatch.ElapsedMilliseconds
                        + "\n";
                    FileHandler.Append("D:\\DiagnosticChain\\DiagnosticChain\\BlockLog.txt", log);

                    stopwatch = new Stopwatch();
                    stopwatch.Start();
                    publisher.ValidateChain();
                    stopwatch.Stop();

                    log = "VALIDATION\t"
                        + (i % maxBlockSize)
                        + "\t" + maxBlockSize
                        + "\t" + publisher.GetChain().Blockhead.Index
                        + "\t" + publisher.GetChain().GetTransactions().Count
                        + "\t" + stopwatch.ElapsedMilliseconds
                        + "\n";
                    FileHandler.Append("D:\\DiagnosticChain\\DiagnosticChain\\ValidationLog.txt", log);

                    QueryNode query = new QueryNode();
                    query.UpdateChain(publisher.GetChain());

                    stopwatch = new Stopwatch();
                    stopwatch.Start();
                    query.ExtractData((PatientRegistrationTransaction p) => true,
                                        (TreatmentTransaction t) => true,
                                        (SymptomsTransaction s) => true,
                                        (DiagnosesTransaction d) => true);
                    stopwatch.Stop();

                    log = "QUERY\t"
                        + (i % maxBlockSize)
                        + "\t" + maxBlockSize
                        + "\t" + publisher.GetChain().Blockhead.Index
                        + "\t" + publisher.GetChain().GetTransactions().Count
                        + "\t" + stopwatch.ElapsedMilliseconds
                        + "\n";
                    FileHandler.Append("D:\\DiagnosticChain\\DiagnosticChain\\QueryLog.txt", log);
                }
            }
        }

        public long GetRAMUsage()
        {
            return GC.GetTotalMemory(true);
        }
    }
}
