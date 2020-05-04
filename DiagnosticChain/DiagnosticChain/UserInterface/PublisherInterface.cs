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
                , { "list transactions", ListTransactions }
                , { "list patients", ListPatients }
                , { "list physicians proposed", ListPhysiciansProposed }
                , { "list physicians", ListPhysicians }
                , { "list publishers proposed", ListPublishersProposed }
                , { "list publishers", ListPublishers }
                , { "load chain", LoadChain }
                , { "save chain", SaveChain }
                , { "test ping", TestPing }
                , { "test transactions", GenerateTestTransactions }
                //, { "validate chain", ValidateChain }
                //, { "vote publisher", VotePublisher }
            };
        }

        public void Interact(Action onCompletion)
        {
            CLI.DisplayLine("Welcome to the publishing interface!");
            CLI.DisplayLineDelimiter();

            if (!controller.HasSavedState()) SetupNewPublisher();
            controller.Start();

            CLI.DisplayLine("PublisherHandler ready. Enter " + UIConstants.abortionCode + " to quit.");
            CLI.DisplayLineDelimiter();

            var userInput = CLI.PromptUser(promptNextCommand);

            while (userInput != UIConstants.abortionCode)
            {
                if (userCommands.ContainsKey(userInput))
                {
                    userCommands[userInput]();
                } else
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

            controller.ShutDown();
            onCompletion();
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
            controller.LoadChain();
            CLI.DisplayLine("Chain loaded");
            CLI.DisplayLineDelimiter();
        }

        private void SaveChain()
        {
            controller.SaveChain();
        }

        private void TestPing()
        {
            var response = controller.PingNode("127.0.0.1:123456");
            CLI.DisplayLine(response);
        }

        private void SetupNewPublisher()
        {
            CLI.DisplayLine("You are not a registered publisher yet. Please provide us with the following data:");
            var country = CLI.PromptUser("Your country:");
            var region = CLI.PromptUser("Your region:");
            var entityName = CLI.PromptUser("Name of your organisation:");

            controller.SetupNewPublisher(country, region, entityName);
            CLI.DisplayLine("Publisher initialized");
        }

        public void PrepareForUser(string username)
        {
            controller = new PublisherController(username);
        }
    }
}
