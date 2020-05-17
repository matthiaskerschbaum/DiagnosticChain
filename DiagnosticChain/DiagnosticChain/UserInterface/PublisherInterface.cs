using DiagnosticChain.Controllers;
using DiagnosticChain.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticChain.UserInterface
{
    class PublisherInterface : IUserInterface
    {
        private PublisherController controller;
        private Dictionary<string, Action> userCommands;

        private static readonly string promptNextCommand = "What do you want me to do now?";

        public PublisherInterface()
        {
            controller = new PublisherController();

            userCommands = new Dictionary<string, Action>()
            {
                { "list blocks", ListBlocks }
                , { "list nodes", ListNodes }
                , { "list transactions", ListTransactions }
                , { "list patients", ListPatients }
                , { "list physicians proposed", ListPhysiciansProposed }
                , { "list physicians", ListPhysicians }
                , { "list publishers proposed", ListPublishersProposed }
                , { "list publishers", ListPublishers }
                , { "load chain", LoadChain }
                , { "ping node", PingNode }
                , { "save chain", SaveChain }
                , { "server reset address", ResetServerAddress }
                , { "server stats address", DisplayServerAddress }
                , { "test transactions", GenerateTestTransactions }
                , { "validate chain", ValidateChain }
                , { "vote publisher", VotePublisher }
            };
        }

        private void DisplayServerAddress()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Server listening under: " + controller.GetServerAddress());
            CLI.DisplayLineDelimiter();
        }

        public void Interact(Action onCompletion)
        {
            try
            {
                CLI.DisplayLine("Welcome to the publishing interface!");
                CLI.DisplayLineDelimiter();

                if (!controller.HasSavedState()) StartAsNewPublisher();
                else controller.Start();

                if (!controller.HasChain()) RegisterAtNode();

                CLI.DisplayLine("PublisherHandler ready. Enter " + UIConstants.abortionCode + " to quit.");
                CLI.DisplayLineDelimiter();

                var userInput = CLI.PromptUser(promptNextCommand);

                while (userInput != UIConstants.abortionCode)
                {
                    if (userCommands.ContainsKey(userInput))
                    {
                        userCommands[userInput]();
                    }
                    else
                    {
                        CLI.DisplayLine("Command not found. The following options are available:\n");
                        foreach (string key in userCommands.Keys)
                        {
                            CLI.DisplayLine(key);
                        }

                        CLI.DisplayLineDelimiter();
                    }

                    userInput = CLI.PromptUser(promptNextCommand);
                }
            }
            finally
            {
                controller.ShutDown();
                onCompletion();
            }
        }

        private void GenerateTestTransactions()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Generating test transactions");
            controller.GenerateTestTransactions();
            CLI.DisplayLine("Test transactions generated");
            CLI.DisplayLineDelimiter();
        }

        private void ListBlocks()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Listing current blocks" );
            CLI.DisplayLineDelimiter();
            var blockStatistics = controller.GetBlockSummary();

            if (blockStatistics.Count == 0)
            {
                CLI.DisplayLine("No blocks found");
            }

            foreach (var s in blockStatistics)
            {
                CLI.DisplayLine(s);
            }

            CLI.DisplayLineDelimiter();
        }

        private void ListNodes()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Listing known nodes");
            CLI.DisplayLineDelimiter();
            var knownNodes = controller.GetKnownNodes();

            foreach (var n in knownNodes)
            {
                CLI.DisplayLine(n.FullAddress);
            }

            CLI.DisplayLineDelimiter();
        }

        private void ListTransactions()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Listing current transactions");
            CLI.DisplayLineDelimiter();
            var transactionStatistics = controller.GetTransactionSummary();

            if (transactionStatistics.Count == 0)
            {
                CLI.DisplayLine("No transactions found");
            }

            foreach (var s in transactionStatistics)
            {
                CLI.DisplayLine(s);
            }

            CLI.DisplayLineDelimiter();
        }

        private void ListPatients()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Listing known patients");
            CLI.DisplayLineDelimiter();
            var patients = controller.GetPatients().ToArray();

            if (patients.Length == 0)
            {
                CLI.DisplayLine("No patients found");
            }

            for (int i = 0; i < patients.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + patients[i].Address + ", " + patients[i].Country + ", " + patients[i].Region + ", " + patients[i].Birthyear);
            }

            CLI.DisplayLineDelimiter();
        }

        private void ListPhysicians()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Listing confirmed physicians");
            CLI.DisplayLineDelimiter();
            var confirmedPhysicians = controller.GetConfirmedPhysicians().ToArray();

            if (confirmedPhysicians.Length == 0)
            {
                CLI.DisplayLine("No physicians found");
            }

            for (int i = 0; i < confirmedPhysicians.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + confirmedPhysicians[i].Name + ", " + confirmedPhysicians[i].Country + ", " + confirmedPhysicians[i].Region);
            }

            CLI.DisplayLineDelimiter();
        }

        private void ListPhysiciansProposed()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Listing proposed physicians");
            CLI.DisplayLineDelimiter();
            var proposedPhysicians = controller.GetProposedPhysicians().ToArray();

            if (proposedPhysicians.Length == 0)
            {
                CLI.DisplayLine("No physicians found");
            }

            for (int i = 0; i < proposedPhysicians.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + proposedPhysicians[i].Name + ", " + proposedPhysicians[i].Country + ", " + proposedPhysicians[i].Region);
            }

            CLI.DisplayLineDelimiter();
        }

        private void ListPublishers()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Listing confirmed publishers");
            CLI.DisplayLineDelimiter();
            var proposedPublishers = controller.GetConfirmedPublishers().ToArray();

            if (proposedPublishers.Length == 0)
            {
                CLI.DisplayLine("No publishers found");
            }

            for (int i = 0; i < proposedPublishers.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + proposedPublishers[i].EntityName + ", " + proposedPublishers[i].Country + ", " + proposedPublishers[i].Region);
            }

            CLI.DisplayLineDelimiter();
        }

        private void ListPublishersProposed()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Listing proposed publishers");
            CLI.DisplayLineDelimiter();
            var proposedPublishers = controller.GetProposedPublishers().ToArray();

            if (proposedPublishers.Length == 0)
            {
                CLI.DisplayLine("No publishers found");
            }

            for (int i = 0; i < proposedPublishers.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + proposedPublishers[i].EntityName + ", " + proposedPublishers[i].Country + ", " + proposedPublishers[i].Region);
            }

            CLI.DisplayLineDelimiter();
        }

        private void LoadChain()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Loading chain...");
            var success = controller.LoadChain();

            if (success)
            {
                CLI.DisplayLine("Chain loaded successfully");
            } else
            {
                CLI.DisplayLine("Chain file corrupted. Please use another blockchain file, or load chain from a known node.");
            }

            CLI.DisplayLine("Load finished");
            CLI.DisplayLineDelimiter();
        }

        private void PingNode()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Pinging a known node");
            CLI.DisplayLineDelimiter();

            var ip = CLI.PromptUser("Please provide the IP address of the server to ping:");
            var port = CLI.PromptUser("Please provide the port to ping on destination server:");

            var pingAddress = new ServerAddress
            {
                Ip = ip
                ,
                Port = Int32.Parse(port)
            };

            CLI.DisplayLine("Pinging " + pingAddress.FullAddress);
            var response = controller.PingNode(pingAddress);
            CLI.DisplayLine("Ping response: " + response);
        }

        public void PrepareForUser(string username)
        {
            controller = new PublisherController(username);
        }

        private void RegisterAtNode()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Connection to an existing node required");
            CLI.DisplayLineDelimiter();

            ServerAddress connectionAddress;
            do
            {
                var ip = CLI.PromptUser("Please provide the IP address of an existing node to connect to:");
                var port = CLI.PromptUser("Please provide the port to connect to at the destination:");

                connectionAddress = new ServerAddress()
                {
                    Ip = ip
                    ,
                    Port = Int32.Parse(port)
                };

                CLI.DisplayLine("Attempting connection to " + connectionAddress.FullAddress + "...");
            } while (!controller.RegisterAt(connectionAddress));

            CLI.DisplayLine("Connection succeeded");
            CLI.DisplayLineDelimiter();
        }

        private void ResetServerAddress()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Resetting the server address");
            CLI.DisplayLineDelimiter();
            
            CLI.DisplayLine("Server is currently listening under: " + controller.GetServerAddress());
            var userChoice = CLI.PromptUser("To continue using this address, enter " + UIConstants.abortionCode + " to quit. To change the address, enter C to continue.");
            if (userChoice == "C")
            {
                CLI.DisplayLineDelimiter();
                var newIp = CLI.PromptUser("Please provide the server's new IP address:");
                var newPort = CLI.PromptUser("Please provide the new port to listen on:");

                if (controller.ChangeServerAddress(newIp, Int32.Parse(newPort)))
                {
                    CLI.DisplayLine("Server address reset successfully");
                } else
                {
                    CLI.DisplayLine("Error resetting the server address to " + newIp + ":" + newPort + ". The address remains unchanged.");
                }
            }
        }

        private void SaveChain()
        {
            controller.SaveChain();
        }

        private void StartAsNewPublisher()
        {
            CLI.DisplayLine("You are not a registered publisher yet. Please provide the following data:");
            var country = CLI.PromptUser("Your country:");
            var region = CLI.PromptUser("Your region:");
            var entityName = CLI.PromptUser("Name of your organisation:");

            CLI.DisplayLine("As a publisher, you need to host a server to handle incoming blockchain requests. In order to set up this server, please provide the following data:");
            var serverIp = CLI.PromptUser("IP address of the host server:");
            var serverPort = Int32.Parse(CLI.PromptUser("Port on which to host the server:"));
            ServerAddress selfAddress = new ServerAddress()
            {
                Ip = serverIp
                ,
                Port = serverPort
            };

            ServerAddress initializerAddress = null;
            var connectionSelection = CLI.PromptUser("Please select further: C to connect to an existing blockchain network, or N to start a new blockchain:");
            if (connectionSelection == "C")
            {
                var connectorIp = CLI.PromptUser("Please provide the IP address of an existing node to connect to:");
                var connectorPort = CLI.PromptUser("Please provide the port to connect to at the destination:");

                initializerAddress = new ServerAddress()
                {
                    Ip = connectorIp
                    ,
                    Port = Int32.Parse(connectorPort)
                };
            }

            controller.StartAsNewPublisher(country, region, entityName, selfAddress, initializerAddress);
            CLI.DisplayLine("Publisher initialized");
        }

        private void ValidateChain()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Validating chain");
            CLI.DisplayLineDelimiter();

            var validity = controller.ValidateChain();
            var display = "Current chain status: " + (validity ? "valid" : "not valid");
            CLI.DisplayLine(display);

            CLI.DisplayLineDelimiter();
        }

        private void VotePublisher()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Vote for proposed publishers");
            CLI.DisplayLineDelimiter();
            var proposedPublishers = controller.GetProposedPublishers().ToArray();

            CLI.DisplayLine("The following publishers are available:\n");
            for (int i = 0; i < proposedPublishers.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + proposedPublishers[i].EntityName + ", " + proposedPublishers[i].Country + ", " + proposedPublishers[i].Region);
            }
            CLI.DisplayLineDelimiter();

            var input = CLI.PromptUser("Please enter the index of the publisher you are voting for, or enter Q to quit: ");

            if (input == "Q") return;
            int index;
            if (int.TryParse(input, out index) && index < proposedPublishers.Length)
            {
                var vote = CLI.PromptUser("Please enter y for confirmed, or n for dismiss");
                controller.VoteForPublisher(proposedPublishers[index].Address, vote == "y");
            }
            else
            {
                CLI.DisplayLine("Publisher not found");
            }

            CLI.DisplayLineDelimiter();
        }
    }
}
