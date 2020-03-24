using Blockchain;
using Blockchain.Interfaces;
using Handler.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Shared.EncryptionHandler;

namespace Handler.Handlers
{
    class PublisherHandler : IHandler
    {
        //Delegates
        private Action onShutDown;

        //Publisher parameters
        Guid publisherAddress;
        KeyPair keys;
        TransactionGenerator transactionGenerator;
        TransactionBuffer transactionBuffer;
        ParticipantHandler participantHandler;

        //Parallel running tasks
        Timer blockGenerator;
        Timer blockProcessor;

        //Chain parameters
        private Chain chain;

        public void StartUp(Action onShutDown)
        {
            CLI.DisplayLine("Starting PublisherHandler...");

            //Initialize Handler
            this.onShutDown = onShutDown;
            transactionBuffer = new TransactionBuffer();
            participantHandler = new ParticipantHandler();
            chain = new Chain();

            //TODO Prompt user for username
            CLI.DisplayLine("Welcome to the publisher interface");
            
            //TODO Prompt for username, test whether publisher is already initialized, read in data if present
            RegisterPublisher();

            //TODO Set up node (Store all blockchain data) => Read from disk if present, prompt for URL else
            //TODO Start listening to incoming requests
            
            //TODO Start publishsing job
            CLI.DisplayLine("Starting publishing jobs...");

            blockGenerator = new Timer(transactionBuffer.BundleTransactions, null, 5000, 5000);
            blockProcessor = new Timer(HandlePublishing, null, 7000, 5000);

            CLI.DisplayLine("Publishing jobs started");


            CLI.DisplayLine("PublisherHandler started. Enter Q to quit.");
            CLI.DisplayLineDelimiter();

            //TODO Start listening to user input
            var userInput = "";
            while (userInput != "Q")
            {
                userInput = CLI.PromptUser("What do you want me to do now?");
                HandleUserInput(userInput);
            }

            ShutDown();
        }

        public void ShutDown()
        {
            CLI.DisplayLine("Shutting down PublisherHandler...");

            //TODO stop listening to incoming requests
            //TODO stop publishing job
            blockGenerator.Dispose();
            blockProcessor.Dispose();

            //TODO Save blockchain to file

            CLI.DisplayLine("PublisherHandler shut down");

            //Invoke callback
            onShutDown();
        }

        public void RegisterPublisher()
        {
            CLI.DisplayLine("You are not a registered publisher yet. Please provide us with the following data:");
            var country = CLI.PromptUser("Your country:");
            var region = CLI.PromptUser("Your region:");
            var entityName = CLI.PromptUser("Name of your organisation:");

            CLI.DisplayLine("Initializing publisher...");
            
            keys = EncryptionHandler.GenerateNewKeys();
            transactionGenerator = new TransactionGenerator(keys.PrivateKey);
            ITransaction registration = transactionGenerator.InitializeAsNewPublisher(keys.PublicKey, country, region, entityName);
            publisherAddress = registration.TransactionId;
            transactionBuffer.RecordTransaction(registration);

            CLI.DisplayLine("Publisher initialized");
        }

        public void HandleUserInput(string input)
        {
            switch (input)
            {
                case "validate":
                    CLI.DisplayLine(chain.Validate(participantHandler.Clone(), null).ToString());
                    break;
                default:
                    break;
            }
        }

        public void HandleIncomingRequests()
        {
            throw new NotImplementedException();
            //TODO Set up node to handle incoming requests
            //TODO Set up node to handle incoming requests
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
                    else
                    {
                        nextBlock.Index = chain.Blockhead.Index + 1;
                        nextBlock.PreviousBlock = chain.Blockhead;
                        nextBlock.PreviousHash = chain.Blockhead.Hash;
                    }

                    nextBlock.Sign(keys.PrivateKey);
                    CLI.DisplayLine(nextBlock.AsJSON());
                    appendix.Add(nextBlock);
                    chain.Add(nextBlock);
                }

                //TODO Broadcast new blocks to other nodes
            }
        }
    }
}
