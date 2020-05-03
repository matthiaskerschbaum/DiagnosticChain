using Blockchain;
using Blockchain.Utilities;
using Grpc.Core;
using Handler.Handlers;
using Shared;
using System;
using System.Collections.Generic;

namespace Handler.IOHandler
{
    class PublisherInputHandler
    {
        private PublisherHandler publisherHandler;
        private Dictionary<string, Action> userCommands;

        public PublisherInputHandler(PublisherHandler publisherHandler)
        {
            this.publisherHandler = publisherHandler;

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

        public void Run()
        {
            var userInput = "";
            while (userInput != "Q")
            {
                userInput = CLI.PromptUser("What do you want me to do now?");

                if (userCommands.ContainsKey(userInput))
                {
                    userCommands[userInput]();
                }
                else if (userInput != "Q")
                {
                    CLI.DisplayLine("Command not found. The following options are available:\n");
                    foreach (string key in userCommands.Keys)
                    {
                        CLI.DisplayLine(key);
                    }

                    CLI.DisplayLineDelimiter();
                }
            }
        }

        private void ListBlocks()
        {
            CLI.DisplayLineDelimiter();
            var blockStatistics = publisherHandler.GetChainStatisics().GetOverviewPerBlock();

            foreach (var s in blockStatistics)
            {
                CLI.DisplayLine(s);
            }

            CLI.DisplayLineDelimiter();
        }

        private void ListTransactions()
        {
            CLI.DisplayLineDelimiter();
            var transactionStatistics = publisherHandler.GetChainStatisics().GetOverviewPerTransaction();

            foreach (var s in transactionStatistics)
            {
                CLI.DisplayLine(s);
            }

            CLI.DisplayLineDelimiter();
        }

        private void ListPatients()
        {
            CLI.DisplayLine("");
            var patients = publisherHandler.participantHandler.GetPatients().ToArray();
            for (int i = 0; i < patients.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + patients[i].Address + ", " + patients[i].Country + ", " + patients[i].Region + ", " + patients[i].Birthyear);
            }
            CLI.DisplayLine("");
        }

        private void ListPhysicians()
        {
            CLI.DisplayLine("");
            var confirmedPhysicians = publisherHandler.participantHandler.GetConfirmedPhysicians().ToArray();
            for (int i = 0; i < confirmedPhysicians.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + confirmedPhysicians[i].Name + ", " + confirmedPhysicians[i].Country + ", " + confirmedPhysicians[i].Region);
            }
            CLI.DisplayLine("");
        }

        private void ListPhysiciansProposed()
        {
            CLI.DisplayLine("");
            var proposedPhysicians = publisherHandler.participantHandler.GetProposedPhysicians().ToArray();
            for (int i = 0; i < proposedPhysicians.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + proposedPhysicians[i].Name + ", " + proposedPhysicians[i].Country + ", " + proposedPhysicians[i].Region);
            }
            CLI.DisplayLine("");
        }

        private void ListPublishers()
        {
            CLI.DisplayLine("");
            var proposedPublishers = publisherHandler.participantHandler.GetConfirmedPublishers().ToArray();
            for (int i = 0; i < proposedPublishers.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + proposedPublishers[i].EntityName + ", " + proposedPublishers[i].Country + ", " + proposedPublishers[i].Region);
            }
            CLI.DisplayLine("");
        }

        private void ListPublishersProposed()
        {
            CLI.DisplayLine("");
            var proposedPublishers = publisherHandler.participantHandler.GetProposedPublishers().ToArray();
            for (int i = 0; i < proposedPublishers.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + proposedPublishers[i].EntityName + ", " + proposedPublishers[i].Country + ", " + proposedPublishers[i].Region);
            }
            CLI.DisplayLine("");
        }

        private void LoadChain()
        {
            publisherHandler.chain = new Chain(FileHandler.Read(FileHandler.ChainPath));
            publisherHandler.participantHandler = new ParticipantHandler();
            publisherHandler.chain.ProcessContracts(publisherHandler.participantHandler, new List<Chain>() { publisherHandler.chain });
        }

        private void SaveChain()
        {
            FileHandler.Save(FileHandler.ChainPath, publisherHandler.chain.AsXML());
        }

        private void TestPing()
        {
            Channel channel = new Channel("127.0.0.1:123456", ChannelCredentials.Insecure);
            var client = new PublisherServer.PublisherServerClient(channel);

            var response = client.Ping(new PingRequest());
            CLI.DisplayLine("Ping result: " + response.Result);

            channel.ShutdownAsync().Wait();
        }

        private void ValidateChain()
        {
            CLI.DisplayLine(publisherHandler.chain.ValidateContextual(new ParticipantHandler(), null).ToString());
        }

        private void VotePublisher()
        {
            CLI.DisplayLine("");
            var proposedPublishers = publisherHandler.participantHandler.GetProposedPublishers().ToArray();

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
                var votingTransaction = publisherHandler.transactionGenerator.GenerateVotingTransaction(proposedPublishers[index].Address, vote == "y");
                publisherHandler.transactionBuffer.RecordTransaction(votingTransaction);
            }
            else
            {
                CLI.DisplayLine("Publisher not found");
            }
        }
    }
}
