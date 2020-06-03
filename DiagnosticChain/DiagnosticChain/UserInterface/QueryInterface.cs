using DiagnosticChain.Controllers;
using DiagnosticChain.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticChain.UserInterface
{
    class QueryInterface : IUserInterface
    {
        QueryController controller;
        private Dictionary<string, Action> userCommands;

        private static readonly string promptNextCommand = "Please specify the type of query you want to perform. Enter " + UIConstants.abortionCode + " to quit.";

        public QueryInterface()
        {
            controller = new QueryController();

            userCommands = new Dictionary<string, Action>() {
                { "country", QueryCountry }
                , { "full", QueryFull }
            };
        }

        private void AddNode()
        {
            CLI.DisplayLine("Connection to an existing DiagnosticChain publisher is required. Please provide the following data:");
            var connectorIp = CLI.PromptUser("Please provide the IP address of an existing node to connect to:");
            var connectorPort = CLI.PromptUser("Please provide the port to connect to at the destination:");

            var initializerAddress = new ServerAddress()
            {
                Ip = connectorIp
                    ,
                Port = Int32.Parse(connectorPort)
            };

            controller.AddNode(initializerAddress);
            CLI.DisplayLine("Node added");
            CLI.DisplayLineDelimiter();
        }

        public void Interact(Action onCompletion)
        {
            try
            {
                CLI.DisplayLine("Welcome to the query interface!");
                CLI.DisplayLineDelimiter();

                controller.Start();

                if (!controller.HasBlockchainConnection())
                {
                    AddNode();
                }

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

        public void PrepareForUser(string username)
        {
            controller = new QueryController(username);
        }

        private void QueryCountry()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Querying blockchain data by countries");
            CLI.DisplayLineDelimiter();

            CLI.DisplayLine("Updating blockchain...");
            controller.UpdateChain();
            CLI.DisplayLine("Blockchain updated...");

            var filepath = CLI.PromptUser("Please specify a file path where to store the file:");

            List<string> countries = new List<string>();
            var c = CLI.PromptUser("Please specify countries for which to query data. Seperate countries with Enter. End input by typing F:");

            while (c != "F")
            {
                countries.Add(c);
                c = CLI.InlinePrompt();
            }

            CLI.DisplayLine("Extracting data...");
            controller.ExtractTreatmentDataByCountry(filepath, countries);
            CLI.DisplayLine("Data extracted...");

            CLI.DisplayLineDelimiter();
        }

        private void QueryFull()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Querying full blockchain data");
            CLI.DisplayLineDelimiter();

            CLI.DisplayLine("Updating blockchain...");
            controller.UpdateChain();
            CLI.DisplayLine("Blockchain updated...");

            var filepath = CLI.PromptUser("Please specify a file path where to store the file:");

            CLI.DisplayLine("Extracting data...");
            controller.ExtractTreatmentDataFull(filepath);
            CLI.DisplayLine("Data extracted...");

            CLI.DisplayLineDelimiter();
        }
    }
}
