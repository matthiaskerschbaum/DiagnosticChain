using Blockchain;
using Blockchain.Interfaces;
using Handler.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Handler.Handlers
{
    class PublisherHandler : IHandler
    {
        //Delegates
        private Action onShutDown;

        public void StartUp(Action onShutDown)
        {
            CLI.DisplayLine("Starting PublisherHandler...");

            //Initialize Handler
            this.onShutDown = onShutDown;

            //TODO Prompt user for username
            //TODO Set up node (Store alle blockchain data) => Read from disk if present, prompt for URL else
            //TODO Start listening to incoming requests
            //TODO Start publishsing job
            //TODO Start listening to user input

            CLI.DisplayLine("PublisherHandler started");


            #region Testing

            Guid Sender = Guid.NewGuid();
            string privateKey = "privateKey";
            string publicKeyPublisher = "publicKey";
            string publicKeyPhysician = "publicKey";

            List<ITransaction> transactions = new List<ITransaction>();
            TransactionGenerator transactionGenerator = new TransactionGenerator(Sender, privateKey);

            ITransaction publisher = transactionGenerator.GeneratePublisherRegistrationTransaction(publicKeyPublisher, "Austria", "Vienna", "Der Horst");
            ITransaction physician = transactionGenerator.GeneratePhysicianRegistrationTransaction(publicKeyPhysician, "Austria", "Vienna", "Der Herbert");
            ITransaction patient = transactionGenerator.GeneratePatientRegistrationTransaction("Austria", "Vienna", "1989");
            ITransaction treatment = transactionGenerator.GenerateTreatmentTransaction(physician.TransactionId, patient.TransactionId);

            transactions.Add(treatment);
            transactions.Add(transactionGenerator.GenerateSymptomTransaction(treatment.TransactionId, new List<string>() { "Headache", "Vertigo" }));
            transactions.Add(transactionGenerator.GenerateDiagnosesTransaction(treatment.TransactionId, new List<string>() { "Dehydration" }));
            transactions.Add(publisher);
            transactions.Add(physician);
            transactions.Add(patient);
            transactions.Add(transactionGenerator.GenerateVotingTransaction(publisher.TransactionId, true));

            CLI.DisplayLine("Printing transactions: ");

            foreach (var t in transactions)
            {
                CLI.DisplayLineDelimiter();
                CLI.DisplayLine(t.AsString());
                CLI.DisplayLine(t.AsXML());
                CLI.DisplayLine(t.AsJSON());
                CLI.DisplayLineDelimiter();
            }

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            CLI.DisplayLine(RSA.ToXmlString(false));
            CLI.DisplayLine(RSA.ToXmlString(true));
            #endregion

            //TODO Replace with actual logic
            ShutDown();
        }

        public void ShutDown()
        {
            CLI.DisplayLine("Shutting down PublisherHandler...");

            //TODO stop listening to incoming requests
            //TODO stop publishing job
            //TODO Save blockchain to file

            CLI.DisplayLine("PublisherHandler shut down");

            //Invoke callback
            onShutDown();
        }

        public void RegisterPublisher(string publicKey, string country, string region, string entityName)
        {
            throw new NotImplementedException();
            //TODO Generate new RegistrationTransaction and add it to the blockchain
        }

        public void HandleUserInput()
        {
            throw new NotImplementedException();
            //TODO Read in user input and handle it accordingly
        }

        public void HandleIncomingRequests()
        {
            throw new NotImplementedException();
            //TODO Set up node to handle incoming requests
            //TODO Set up node to handle incoming requests
        }

        public void HandlePublishing()
        {
            throw new NotImplementedException();
            //TODO Set up node to publish periodically
        }
    }
}
