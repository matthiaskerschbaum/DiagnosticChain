using Handler;
using Handler.Handlers;
using Handler.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DiagnosticChain
{
    class Program
    {
        private static Dictionary<string, IHandler> handlers = new Dictionary<string, IHandler> {
            { "publisher", new PublisherHandler() }
            ,{ "physician", new PhysicianHandler() }
            ,{ "query", new QueryingHandler() }
        };
        private static Dictionary<string, string> users = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            CLI.DisplayLine("Welcome to the DiagnosticChain!");
            CLI.DisplayLineDelimiter();

            ReadUsers();
            SetupHandler();
        }

        private static void ReadUsers()
        {
            var usersRaw = FileHandler.Read(FileHandler.UsersPath);
            
            foreach (var line in usersRaw.Split('\n'))
            {
                var lineParts = line.Split('\t');
                if (lineParts.Length == 2)
                {
                    users.Add(lineParts[0], lineParts[1]);
                }
            }
        }

        static void SetupHandler()
        {
            //TODO Prompt for username, test whether user is already initialized, read in data if present
            var username = CLI.PromptUser("Please provide your username. If you do not have a user on this machine yet, press C to continue. Press Q to quit");

            while (!users.ContainsKey(username) && username != "C" && username != "Q")
            {
                username = CLI.PromptUser("Username not found, try again. If you do not have a user on this machine yet, press C to continue. Press Q to quit");
            }

            if (username == "Q") return;

            if (username == "C")
            {
                username = CLI.PromptUser("Please choose a new username for this client:");

                while (users.ContainsKey(username))
                {
                    username = CLI.PromptUser("This username is already taken. Please choose a different username");
                }

                var handlertype = CLI.PromptUser("Please specify this node's type");
                
                while (!handlers.ContainsKey(handlertype))
                {
                    CLI.DisplayLine("Handler type not found, please try again. The following options are available: ");
                    foreach (string key in handlers.Keys)
                    {
                        CLI.DisplayLine(key);
                    }

                    CLI.DisplayLineDelimiter();
                    handlertype = CLI.PromptUser("");
                }

                FileHandler.Append(FileHandler.UsersPath, username + "\t" + handlertype + "\n");
                users.Add(username, handlertype);
            }

            if (File.Exists(username + FileHandler.StatePath))
            {
                var state = FileHandler.Read(username + FileHandler.StatePath);

                XmlSerializer serializer = new XmlSerializer(handlers[users[username]].GetType());
                IHandler handler = (IHandler)serializer.Deserialize(new StringReader(state));
                handler.StartUp(OnHandlerShutdown);
            } else
            {
                CLI.DisplayLineDelimiter();
                CLI.DisplayLineDelimiter();
                CLI.DisplayLineDelimiter();
                handlers[users[username]].StartUp(OnHandlerShutdown, true, username);
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
