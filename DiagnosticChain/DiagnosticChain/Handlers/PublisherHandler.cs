using Blockchain;
using Blockchain.Interfaces;
using Blockchain.Utilities;
using Grpc.Core;
using Handler.Interfaces;
using Handler.IOHandler;
using NetworkingFacilities.Servers;
using Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using static Shared.EncryptionHandler;

namespace Handler.Handlers
{
    public class PublisherHandler : IHandler
    {
        //Publisher parameters
        public Guid publisherAddress;
        public KeyPair keys;
        public TransactionGenerator transactionGenerator;
        internal TransactionBuffer transactionBuffer; //TODO Change to public once serialization is dealt with
        public ParticipantHandler participantHandler;

        //Parallel running tasks
        internal Timer blockGenerator;
        internal Timer blockProcessor;
        internal Server server;

        public override void StartUp(Action onShutDown, bool registerNew = false, string username = null)
        {
            CLI.DisplayLine("Starting PublisherHandler...");

            //Initialize Handler
            this.onShutDown = onShutDown;
            transactionBuffer = new TransactionBuffer();
            participantHandler = new ParticipantHandler();
            chain = new Chain();

            CLI.DisplayLine("Welcome to the publisher interface");

            InitializeChain();

            if (registerNew)
            {
                this.username = username;
                RegisterPublisher();
            }

            CLI.DisplayLine("Starting publishing jobs...");

            blockGenerator = new Timer(transactionBuffer.BundleTransactions, null, 5000, 5000);
            blockProcessor = new Timer(HandlePublishing, null, 7000, 5000);

            //TODO Start listening to incoming requests
            HandleIncomingRequests();

            CLI.DisplayLine("Publishing jobs started");

            CLI.DisplayLine("PublisherHandler started. Enter Q to quit.");
            CLI.DisplayLineDelimiter();

            PublisherInputHandler inputHandler = new PublisherInputHandler(this);
            inputHandler.Run();

            ShutDown();
        }

        internal void GenerateTestTransactions()
        {
            CLI.DisplayLine("Generating Test transactions");
            var physicianKeys = EncryptionHandler.GenerateNewKeys();
            FileHandler.Log("Physician Public Key: " + physicianKeys.PublicKey.AsString());
            FileHandler.Log("Physician Private Key: " + physicianKeys.PrivateKey.AsString());
            var tempTransactions = new TransactionGenerator(physicianKeys.PrivateKey);

            var physicianRegistration = tempTransactions.InitializeAsNewPhysician(physicianKeys.PublicKey, "Austria", "Vienna", "Der Herbert");
            var physicianVoting = transactionGenerator.GenerateVotingTransaction(physicianRegistration.TransactionId, true);

            var patientRegistration = tempTransactions.GeneratePatientRegistrationTransaction("Austria", "Vienna", "1992");
            var treatment = tempTransactions.GenerateTreatmentTransaction(physicianRegistration.TransactionId, patientRegistration.TransactionId);
            var symptom = tempTransactions.GenerateSymptomTransaction(treatment.TransactionId, new List<string>() { "R05", "R50.80" });
            var diagnoses = tempTransactions.GenerateDiagnosesTransaction(treatment.TransactionId, new List<string>() { "B34.2" });

            transactionBuffer.RecordTransaction(physicianRegistration);
            transactionBuffer.RecordTransaction(physicianVoting);
            Thread.Sleep(4000);
            transactionBuffer.RecordTransaction(patientRegistration);
            transactionBuffer.RecordTransaction(treatment);
            Thread.Sleep(1000);
            transactionBuffer.RecordTransaction(symptom);
            transactionBuffer.RecordTransaction(diagnoses);
        }

        public void HandleIncomingRequests()
        {
            server = new Server
            {
                Services = { PublisherServer.BindService(new PublisherServerImpl()) },
                Ports = { new ServerPort("localhost", 123456, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        public void HandlePublishing(object state)
        {
            if (transactionBuffer.HasBlocks())
            {
                CLI.DisplayLine("Publishing...");
                Chain appendix = new Chain();

                while (transactionBuffer.HasBlocks())
                {
                    var nextBlock = transactionBuffer.GetNextBlock();
                    nextBlock.Publisher = publisherAddress;

                    if (chain.IsEmpty())
                    {
                        nextBlock.Index = 0;
                        nextBlock.PreviousBlock = null;
                        nextBlock.PreviousHash = null;
                    }
                    else if (appendix.IsEmpty())
                    {
                        nextBlock.Index = chain.Blockhead.Index + 1;
                        nextBlock.PreviousHash = chain.Blockhead.Hash;
                    }
                    else
                    {
                        nextBlock.Index = appendix.Blockhead.Index + 1;
                        nextBlock.PreviousBlock = appendix.Blockhead;
                        nextBlock.PreviousHash = appendix.Blockhead.Hash;
                    }

                    nextBlock.Sign(keys.PrivateKey);
                    appendix.Add(nextBlock);
                }


                appendix.ProcessContracts(participantHandler, new List<Chain>() { chain });
                chain.Add(appendix);

                //TODO Broadcast new blocks to other nodes
            }
        }

        private void InitializeChain()
        {
            //TODO Set up node (Store all blockchain data) => Read from disk if present, prompt for URL else
            chain = new Chain(FileHandler.Read(FileHandler.ChainPath));
            if (chain.IsEmpty() || !chain.ValidateContextual(participantHandler, new List<Chain>() { chain }))
            {
                var input = CLI.PromptUser("No blockhain data found, or Blockchain file corrupted.\n " +
                    "Please enter URL of a known node, or press C to continue as initial publisher");
                if (input != "C")
                {
                    //TODO Check provided URL and connect to node
                }
            }
        }

        public void RegisterPublisher()
        {
            CLI.DisplayLine("You are not a registered publisher yet. Please provide us with the following data:");
            var country = CLI.PromptUser("Your country:");
            var region = CLI.PromptUser("Your region:");
            var entityName = CLI.PromptUser("Name of your organisation:");

            CLI.DisplayLine("Initializing publisher...");

            keys = EncryptionHandler.GenerateNewKeys();
            FileHandler.Log("Publisher Public Key: " + keys.PublicKey.AsString());
            FileHandler.Log("Publisher Private Key: " + keys.PrivateKey.AsString());

            if (transactionGenerator == null)
            {
                transactionGenerator = new TransactionGenerator(keys.PrivateKey);
            }

            ITransaction registration = transactionGenerator.InitializeAsNewPublisher(keys.PublicKey, country, region, entityName);
            publisherAddress = registration.TransactionId;

            transactionBuffer.RecordTransaction(registration);

            if (chain.IsEmpty()) //The initial publisher grants themselves permission and creates the initial block
            {
                ITransaction vote = transactionGenerator.GenerateVotingTransaction(publisherAddress, true);
                transactionBuffer.RecordTransaction(vote);
            }

            CLI.DisplayLine("Publisher initialized");
        }

        public override void ShutDown()
        {
            CLI.DisplayLine("Shutting down PublisherHandler...");

            server.ShutdownAsync().Wait();

            blockProcessor.Dispose();
            blockGenerator.Dispose();

            FileHandler.Save(FileHandler.ChainPath, chain.AsXML());
            FileHandler.Save(username + FileHandler.StatePath, HandlerStateAsXML());

            CLI.DisplayLine("PublisherHandler shut down");

            onShutDown();
        }
    }
}
