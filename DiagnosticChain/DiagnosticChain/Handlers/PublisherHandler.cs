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

            var keysPublisher = EncryptionHandler.GenerateNewKeys();
            var keysPhysician = EncryptionHandler.GenerateNewKeys();

            Guid publisher = Guid.NewGuid();
            //string privateKey = "privateKey";
            //string publicKeyPublisher = "publicKey";
            //string publicKeyPhysician = "publicKey";

            //List<ITransaction> transactions = new List<ITransaction>();
            TransactionGenerator transactionGenerator = new TransactionGenerator(publisher, keysPublisher.PrivateKey);
            var block = new Block(publisher);

            ITransaction publisherRegistration = transactionGenerator.GeneratePublisherRegistrationTransaction(keysPublisher.PublicKey, "Austria", "Vienna", "Der Horst");
            ITransaction physicianRegistration = transactionGenerator.GeneratePhysicianRegistrationTransaction(keysPhysician.PublicKey, "Austria", "Vienna", "Der Herbert");
            ITransaction patientRegistration = transactionGenerator.GeneratePatientRegistrationTransaction("Austria", "Vienna", "1989");
            ITransaction treatment = transactionGenerator.GenerateTreatmentTransaction(physicianRegistration.TransactionId, patientRegistration.TransactionId);

            block.AddTransaction(publisherRegistration);
            block.AddTransaction(physicianRegistration);
            block.AddTransaction(patientRegistration);
            block.AddTransaction(treatment);
            block.AddTransaction(transactionGenerator.GenerateSymptomTransaction(treatment.TransactionId, new List<string>() { "Headache", "Vertigo" }));
            block.AddTransaction(transactionGenerator.GenerateDiagnosesTransaction(treatment.TransactionId, new List<string>() { "Dehydration" }));
            block.AddTransaction(transactionGenerator.GenerateVotingTransaction(publisherRegistration.TransactionId, true));

            block.Sign(keysPublisher.PrivateKey);

            CLI.DisplayLine(block.AsJSON());
            CLI.DisplayLine(block.AsXML());
            CLI.DisplayLine(block.AsString());

            CLI.DisplayLineDelimiter();

            CLI.DisplayLine(block.Validate(keysPublisher.PublicKey).ToString());
            CLI.DisplayLine(block.Validate(keysPhysician.PublicKey).ToString());

            //transactions.Add(treatment);
            //transactions.Add(transactionGenerator.GenerateSymptomTransaction(treatment.TransactionId, new List<string>() { "Headache", "Vertigo" }));
            //transactions.Add(transactionGenerator.GenerateDiagnosesTransaction(treatment.TransactionId, new List<string>() { "Dehydration" }));
            //transactions.Add(publisher);
            //transactions.Add(physician);
            //transactions.Add(patient);
            //transactions.Add(transactionGenerator.GenerateVotingTransaction(publisher.TransactionId, true));

            //CLI.DisplayLine("Printing transactions: ");

            //foreach (var t in transactions)
            //{
            //    CLI.DisplayLineDelimiter();
            //    //CLI.DisplayLine(t.AsString());
            //    //CLI.DisplayLine(t.AsXML());
            //    CLI.DisplayLine(t.AsJSON());
            //    CLI.DisplayLineDelimiter();
            //}

            //CLI.DisplayLine("Printing validations: ");

            //foreach (var t in transactions)
            //{
            //    CLI.DisplayLineDelimiter();
            //    CLI.DisplayLine(t.Validate(keysPublisher.PublicKey).ToString());
            //    CLI.DisplayLine(t.Validate(keysPhysician.PublicKey).ToString());
            //    CLI.DisplayLineDelimiter();
            //}


            //var message = "Das ist ein Test!";
            //var hash = Convert.ToBase64String( SHA256.Create().ComputeHash(Encoding.Unicode.GetBytes(message)) );

            //CLI.DisplayLine(message);

            //var signature = EncryptionHandler.Sign(message, keys.PrivateKey);
            //var reference = EncryptionHandler.VerifiySignature(message, signature, keys.PublicKey);

            //CLI.DisplayLine(signature);
            //CLI.DisplayLine(reference.ToString());

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
