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
                , { "test transactions", publisherHandler.GenerateTestTransactions }
                , { "validate chain", ValidateChain }
                , { "vote publisher", VotePublisher }
            };
        }

        public void Interact()
        {
            CLI.DisplayLine("Welcome to the publishing interface!");
            CLI.DisplayLineDelimiter();

            if (!controller.HasSavedState()) SetupNewPublisher();
            controller.LoadState();

            CLI.DisplayLine("Starting publishing jobs...");
            controller.StartPublishing();
            CLI.DisplayLine("Publishing jobs started");

            CLI.DisplayLine("PublisherHandler ready. Enter Q to quit.");
            CLI.DisplayLineDelimiter();
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
