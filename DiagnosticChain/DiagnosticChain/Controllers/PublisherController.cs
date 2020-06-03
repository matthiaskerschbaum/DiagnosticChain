using Blockchain;
using Blockchain.Entities;
using Blockchain.Interfaces;
using Blockchain.Utilities;
using Grpc.Core;
using NetworkingFacilities.Clients;
using NetworkingFacilities.Servers;
using NodeManagement;
using NodeManagement.Entities;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace DiagnosticChain.Controllers
{
    public class PublisherController
    {
        private string currentUser = "";
        
        private PublisherNode node;
        private Server server;
        private ServerAddress serverSettings;

        private TransactionGenerator transactionGenerator;

        public PublisherController()
        {
            node = new PublisherNode();
            serverSettings = new ServerAddress();
        }

        public PublisherController(string currentUser) : this()
        {
            this.currentUser = currentUser;
        }

        internal bool ChangeServerAddress(string newIp, int newPort)
        {
            //Test whether settings work
            try
            {
                var newServer = new Server
                {
                    Services = { PublisherServer.BindService(new PublisherServerImpl(node, node, serverSettings)) },
                    Ports = { new ServerPort(newIp, newPort, ServerCredentials.Insecure) }
                };
                newServer.Start();
                newServer.ShutdownAsync().Wait();
            } catch(RpcException)
            {
                return false;
            }

            //If settings are valid, shut down current server instance, start new one and update server settings
            server.ShutdownAsync().Wait();

            serverSettings = new ServerAddress()
            {
                Ip = newIp
                ,
                Port = newPort
            };
            node.SetSelfAddress(serverSettings);

            server = new Server
            {
                Services = { PublisherServer.BindService(new PublisherServerImpl(node, node, serverSettings)) },
                Ports = { new ServerPort(newIp, newPort, ServerCredentials.Insecure) }
            };
            server.Start();

            return true;
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

            node.OnReceiveTransaction(physicianRegistration);
            node.OnReceiveTransaction(physicianVoting);
            Thread.Sleep(4000);
            node.OnReceiveTransaction(patientRegistration);
            node.OnReceiveTransaction(treatment);
            Thread.Sleep(1000);
            node.OnReceiveTransaction(symptom);
            node.OnReceiveTransaction(diagnoses);
        }

        internal List<string> GetBlockSummary()
        {
            return node.GetChainStatisics().GetOverviewPerBlock();
        }

        internal List<Blockchain.Entities.Physician> GetConfirmedPhysicians()
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

        internal List<Blockchain.Entities.Patient> GetPatients()
        {
            var patients = node.GetPatients();
            patients.Sort((x, y) => x.Address.CompareTo(y.Address));
            return patients;
        }

        internal List<Blockchain.Entities.Physician> GetProposedPhysicians()
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

        internal string GetServerAddress()
        {
            return serverSettings.FullAddress;
        }

        internal List<string> GetTransactionSummary()
        {
            return node.GetChainStatisics().GetOverviewPerTransaction();
        }

        internal bool HasSavedState()
        {
            return File.Exists(currentUser + FileHandler.UserState_NodePath)
                && File.Exists(currentUser + FileHandler.UserState_ServerPath);
        }

        internal bool HasChain()
        {
            return node.IsChainInitialized();
        }

        internal List<ServerAddress> GetKnownNodes()
        {
            return node.GetKnownNodes();
        }

        internal bool LoadChain()
        {
            return node.LoadChain();
        }

        internal void LoadState()
        {
            if (HasSavedState())
            {
                var nodeState = FileHandler.Read(currentUser + FileHandler.UserState_NodePath);
                XmlSerializer serializer = new XmlSerializer(node.GetType());
                node = (PublisherNode)serializer.Deserialize(new StringReader(nodeState));
                currentUser = node.User.Username;
                transactionGenerator = new TransactionGenerator(node.User.UserAddress, node.User.Keys.PrivateKey);

                //TODO read server state
                var serverState = FileHandler.Read(currentUser + FileHandler.UserState_ServerPath);
                serializer = new XmlSerializer(serverSettings.GetType());
                serverSettings = (ServerAddress)serializer.Deserialize(new StringReader(serverState));
                node.SetSelfAddress(serverSettings);
            }
        }

        internal string PingNode(ServerAddress url)
        {
            return new PublisherClient(url, serverSettings).PingNode().Status.ToString();
        }

        internal bool RegisterAt(ServerAddress address)
        {
            if (new PublisherClient(address, serverSettings).RegisterNode(serverSettings))
            {
                node.AddServerAddress(address);
                return RequestChain() || node.IsChainInitialized(); //Either chain file was loaded from another client, or the chain is initialized and up to date
            }

            return false;
        }

        internal bool RequestChain()
        {
            var chainLoaded = false;
            var knownNodes = node.GetKnownNodes();
            Chain receivedChain;

            foreach (var n in knownNodes)
            {
                try
                {
                    receivedChain = new PublisherClient(n, serverSettings).RequestFullChain();

                    if (node.OnReceiveChain(receivedChain))
                    {
                        chainLoaded = true; //Continue to query other known nodes after this, in case newer version of chain is found
                    }
                } catch (RpcException) { }
            }

            return chainLoaded;
        }

        internal void RequestChainDelta()
        {
            var knownNodes = node.GetKnownNodes();
            var currentIndex = node.GetCurrentChainIndex();

            foreach (var n in knownNodes)
            {
                try
                {
                    var receivedChain = new PublisherClient(n, serverSettings).RequestDeltaChain(currentIndex);

                    if (!receivedChain.IsEmpty() && node.OnReceiveChain(receivedChain))
                    {
                        currentIndex = node.GetCurrentChainIndex();
                    }
                } catch (RpcException) { }
            }
        }

        internal void SaveState()
        {
            //Saving node state
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

            FileHandler.Save(currentUser + FileHandler.UserState_NodePath, xml);

            //Saving server state
            xsSubmit = new XmlSerializer(serverSettings.GetType());
            xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, serverSettings);
                    xml = sww.ToString();
                }
            }

            FileHandler.Save(currentUser + FileHandler.UserState_ServerPath, xml);
        }

        internal void StartAsNewPublisher(string country, string region, string entityName, ServerAddress selfAddress, ServerAddress initializerAddress)
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
                UserAddress = registration.TransactionId
            };

            serverSettings = selfAddress;
            node.SetSelfAddress(serverSettings);

            Start();

            if (initializerAddress != null)
            {
                RegisterAt(initializerAddress);
                UpdateKnownNodes();
            }

            if (!node.IsChainInitialized())
            {
                ITransaction vote = transactionGenerator.GenerateVotingTransaction(registration.TransactionId, true);
                node.InitializeEmptyChain(registration, vote);
            } else
            {
                node.OnReceiveTransaction(registration);
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
            transactionGenerator = new TransactionGenerator(node.User.UserAddress, node.User.Keys.PrivateKey);
            node.LoadChain();
            
            server = new Server
            {
                Services = { PublisherServer.BindService(new PublisherServerImpl(node, node, serverSettings)) },
                Ports = { new ServerPort(serverSettings.Ip, serverSettings.Port, ServerCredentials.Insecure) }
            };
            server.Start();

            RequestChainDelta();
            UpdateKnownNodes();

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

        private void UpdateKnownNodes()
        {
            var nodes2Add = new List<ServerAddress>();

            foreach (var n in node.knownNodes)
            {
                try
                {
                    var nodeList = new PublisherClient(n, serverSettings).RequestNodes();

                    foreach (var newN in nodeList)
                    {
                        if (!node.knownNodes.Contains(newN) && !nodes2Add.Contains(newN) && newN.FullAddress != serverSettings.FullAddress)
                        {
                            nodes2Add.Add(newN);
                        }
                    }
                } catch (RpcException) { }
            }

            foreach (var n in nodes2Add)
            {
                node.AddServerAddress(n);
            }
        }

        internal bool ValidateChain()
        {
            return node.ValidateChain();
        }

        internal void VoteFor(Guid participant, bool vote)
        {
            var votingTransaction = transactionGenerator.GenerateVotingTransaction(participant, vote);
            node.OnReceiveTransaction(votingTransaction);
        }
    }
}
