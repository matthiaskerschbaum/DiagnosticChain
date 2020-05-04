using Blockchain.Entities;
using Blockchain.Interfaces;
using Blockchain.Utilities;
using Grpc.Core;
using NetworkingFacilities.Servers;
using NodeManagement;
using NodeManagement.Entities;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace DiagnosticChain.Controllers
{
    class PublisherController
    {
        private string currentUser = "";
        private Node node;
        private Server server;
        private TransactionGenerator transactionGenerator;

        public PublisherController()
        {
            node = new Node();
        }

        public PublisherController(string currentUser) : this()
        {
            this.currentUser = currentUser;
        }

        internal void GenerateTestTransactions()
        {
            var physicianKeys = EncryptionHandler.GenerateNewKeys();
            var tempTransactions = new TransactionGenerator(physicianKeys.PrivateKey);

            var physicianRegistration = tempTransactions.InitializeAsNewPhysician(physicianKeys.PublicKey, "Austria", "Vienna", "Der Herbert");
            var physicianVoting = transactionGenerator.GenerateVotingTransaction(physicianRegistration.TransactionId, true);

            var patientRegistration = tempTransactions.GeneratePatientRegistrationTransaction("Austria", "Vienna", "1992");
            var treatment = tempTransactions.GenerateTreatmentTransaction(physicianRegistration.TransactionId, patientRegistration.TransactionId);
            var symptom = tempTransactions.GenerateSymptomTransaction(treatment.TransactionId, new List<string>() { "R05", "R50.80" });
            var diagnoses = tempTransactions.GenerateDiagnosesTransaction(treatment.TransactionId, new List<string>() { "B34.2" });

            node.OnReveiceTransaction(physicianRegistration);
            node.OnReveiceTransaction(physicianVoting);
            Thread.Sleep(4000);
            node.OnReveiceTransaction(patientRegistration);
            node.OnReveiceTransaction(treatment);
            Thread.Sleep(1000);
            node.OnReveiceTransaction(symptom);
            node.OnReveiceTransaction(diagnoses);
        }

        internal List<string> GetBlockSummary()
        {
            return node.GetChainStatisics().GetOverviewPerBlock();
        }

        internal List<Patient> GetPatients()
        {
            var patients = node.GetPatients();
            patients.Sort((x, y) => x.Address.CompareTo(y.Address));
            return patients;
        }

        internal List<Physician> GetConfirmedPhysicians()
        {
            var physicians = node.GetConfirmedPhysicians();
            physicians.Sort((x, y) => x.Address.CompareTo(y.Address));
            return physicians;
        }

        internal List<Blockchain.Entities.Publisher> GetConfirmedPublishers()
        {
            var publishers = node.GetConfirmedPublishers();
            publishers.Sort((x, y) => x.Address.CompareTo(y.Address));
            return publishers;
        }

        internal List<Physician> GetProposedPhysicians()
        {
            var physicians = node.GetProposedPhysicians();
            physicians.Sort((x, y) => x.Address.CompareTo(y.Address));
            return physicians;
        }

        internal List<Blockchain.Entities.Publisher> GetProposedPublishers()
        {
            var publishers = node.GetProposedPublishers();
            publishers.Sort((x, y) => x.Address.CompareTo(y.Address));
            return publishers;
        }

        internal List<string> GetTransactionSummary()
        {
            return node.GetChainStatisics().GetOverviewPerTransaction();
        }

        internal bool HasSavedState()
        {
            return File.Exists(currentUser + FileHandler.StatePath);
        }

        internal void LoadChain()
        {
            node.LoadChain();
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

        internal string PingNode(string url)
        {
            Channel channel = new Channel(url, ChannelCredentials.Insecure);
            var client = new PublisherServer.PublisherServerClient(channel);

            var response = client.Ping(new PingRequest());
            
            channel.ShutdownAsync().Wait();

            return response.Result;
        }

        internal void SaveState()
        {
            XmlSerializer xsSubmit = new XmlSerializer(node.GetType());
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, node);
                    xml = sww.ToString();
                }
            }

            FileHandler.Save(currentUser + FileHandler.StatePath, xml);
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

        internal void ShutDown()
        {
            StopPublishing();
            server.ShutdownAsync().Wait();
            node.SaveChain();
            SaveState();
        }

        internal void Start()
        {
            LoadState();
            transactionGenerator = new TransactionGenerator(node.User.Keys.PrivateKey);
            node.LoadChain();

            server = new Server
            {
                Services = { PublisherServer.BindService(new PublisherServerImpl()) },
                Ports = { new ServerPort("localhost", 123456, ServerCredentials.Insecure) }
            };
            server.Start();

            StartPublishing();
        }

        internal void StartPublishing()
        {
            node.StartPublishing();
        }

        internal void SaveChain()
        {
            node.SaveChain();
        }

        internal void StopPublishing()
        {
            node.StopPublishing();
        }
    }
}
