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
        private Dictionary<string, Action> userCommands;

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
            userCommands = new Dictionary<string, Action>()
            {
                { "list transactions", () =>
                    {
                        CLI.DisplayLineDelimiter();
                        chain.ListTransactions();
                        CLI.DisplayLineDelimiter();
                    }
                }
                , { "list physicians proposed", () =>
                    {
                        CLI.DisplayLine("");
                        var proposedPhysicians = participantHandler.GetProposedPhysicians();
                        for (int i = 0; i < proposedPhysicians.Length; i++)
                        {
                            CLI.DisplayLine(i + "\t" + proposedPhysicians[i].Name + ", " + proposedPhysicians[i].Country + ", " + proposedPhysicians[i].Region);
                        }
                        CLI.DisplayLine("");
                    }
                }
                , { "list physicians", () =>
                    {
                        CLI.DisplayLine("");
                        var confirmedPhysicians = participantHandler.GetConfirmedPhysicians();
                        for (int i = 0; i < confirmedPhysicians.Length; i++)
                        {
                            CLI.DisplayLine(i + "\t" + confirmedPhysicians[i].Name + ", " + confirmedPhysicians[i].Country + ", " + confirmedPhysicians[i].Region);
                        }
                        CLI.DisplayLine("");
                    }
                }
                , { "list publishers proposed", () =>
                    {
                        CLI.DisplayLine("");
                        var proposedPublishers = participantHandler.GetProposedPublishers();
                        for (int i = 0; i < proposedPublishers.Length; i++)
                        {
                            CLI.DisplayLine(i + "\t" + proposedPublishers[i].EntityName + ", " + proposedPublishers[i].Country + ", " + proposedPublishers[i].Region);
                        }
                        CLI.DisplayLine("");
                    }
                }
                , { "list publishers", () => 
                    {
                        CLI.DisplayLine("");
                        var proposedPublishers = participantHandler.GetConfirmedPublishers();
                        for (int i = 0; i < proposedPublishers.Length; i++)
                        {
                            CLI.DisplayLine(i + "\t" + proposedPublishers[i].EntityName + ", " + proposedPublishers[i].Country + ", " + proposedPublishers[i].Region);
                        }
                        CLI.DisplayLine("");
                    } 
                }
                , { "test transactions", () => { GenerateTestTransactions(); } }
                , { "validate chain", () => { CLI.DisplayLine(chain.Validate(participantHandler.Clone(), null).ToString()); } }
                , { "vote publisher", () =>
                    {
                        CLI.DisplayLine("");
                        var proposedPublishers = participantHandler.GetProposedPublishers();

                        CLI.DisplayLine("The following publishers are available:\n");
                        for (int i = 0; i < proposedPublishers.Length; i++)
                        {
                            CLI.DisplayLine(i + "\t" + proposedPublishers[i].EntityName + ", " + proposedPublishers[i].Country + ", " + proposedPublishers[i].Region);
                        }
                        CLI.DisplayLine("");

                        var input = CLI.PromptUser("Please enter the index of the publisher you are voting for, or enter Q to quit: ");

                        if (input == "Q") return;
                        int index;
                        if (int.TryParse(input, out index) && index < proposedPublishers.Length)
                        {
                            var vote = CLI.PromptUser("Please enter y for confirmed, or n for dismiss");
                            var votingTransaction = transactionGenerator.GenerateVotingTransaction(proposedPublishers[index].Address, vote == "y");
                            transactionBuffer.RecordTransaction(votingTransaction);
                        } else
                        {
                            CLI.DisplayLine("Publisher not found");
                        }
                    }
                }
            };

            var userInput = "";
            while (userInput != "Q")
            {
                userInput = CLI.PromptUser("What do you want me to do now?");
                HandleUserInput(userInput);
            }

            ShutDown();
        }

        private void GenerateTestTransactions()
        {
            var physicianRegistration = transactionGenerator.GeneratePhysicianRegistrationTransaction(EncryptionHandler.GenerateNewKeys().PublicKey, "Austria", "Vienna", "Der Herbert");
            var physicianVoting = transactionGenerator.GenerateVotingTransaction(physicianRegistration.TransactionId, true);

            transactionBuffer.RecordTransaction(physicianRegistration);
            transactionBuffer.RecordTransaction(physicianVoting);
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
            if (userCommands.ContainsKey(input))
            {
                userCommands[input]();
            } else
            {
                CLI.DisplayLine("Command not found. The following options are available:\n");
                foreach (string key in userCommands.Keys)
                {
                    CLI.DisplayLine(key);
                }

                CLI.DisplayLineDelimiter();
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
                    } else
                    {
                        nextBlock.Index = appendix.Blockhead.Index + 1;
                        nextBlock.PreviousBlock = appendix.Blockhead;
                        nextBlock.PreviousHash = appendix.Blockhead.Hash;
                    }

                    nextBlock.Sign(keys.PrivateKey);
                    appendix.Add(nextBlock);
                }
                
                appendix.HandleContextual(participantHandler, new List<Chain>() { chain });
                chain.Add(appendix);
                
                //TODO Broadcast new blocks to other nodes
            }
        }
    }
}
