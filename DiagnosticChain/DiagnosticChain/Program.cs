using Handler;
using Handler.Handlers;
using Handler.Interfaces;
using Shared;
using System;
using System.Collections.Generic;

namespace DiagnosticChain
{
    class Program
    {
        private static Dictionary<string, IHandler> handlers = new Dictionary<string, IHandler> {
            { "publisher", new PublisherHandler() }
            ,{ "physician", new PhysicianHandler() }
            ,{ "query", new QueryingHandler() }
        };

        static void Main(string[] args)
        {
            CLI.DisplayLine("Welcome to the DiagnosticChain!");
            CLI.DisplayLineDelimiter();

            SetupHandler();
        }

        static void SetupHandler()
        {
            //TODO Prompt user for node type (Special "Q" for quit)
            var response = CLI.PromptUser("Please specify this node's type, or press Q to quit");
            if (response == "Q")
            {
                //TODO handle complete shutdown
                return;
            }

            if (!handlers.ContainsKey(response))
            {
                CLI.DisplayLine("Handler type not found, please try again. The following options are available: ");
                foreach (string key in handlers.Keys)
                {
                    CLI.DisplayLine(key);
                }

                CLI.DisplayLineDelimiter();

                SetupHandler();
            }
            else
            {
                CLI.DisplayLineDelimiter();
                CLI.DisplayLineDelimiter();
                CLI.DisplayLineDelimiter();
                //TODO Set up node type according to user input
                var handler = handlers[response];
                handler.StartUp(OnHandlerShutdown);
            }
        }

        static void OnHandlerShutdown()
        {
            //TODO Handle shutdown

            CLI.DisplayLineDelimiter();
            SetupHandler();
        }
    }
}
