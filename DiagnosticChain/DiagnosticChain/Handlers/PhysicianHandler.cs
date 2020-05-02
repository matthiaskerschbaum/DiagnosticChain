using Handler.Entities;
using Handler.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Handler.Handlers
{
    public class PhysicianHandler : IHandler
    {
        //Properties
        private List<Patient> Patients = new List<Patient>();

        public override void StartUp(Action onShutDown, bool registerNew = false, string username = null)
        {
            CLI.DisplayLine("Starting PhysicianHandler...");

            //Initialize Handler
            this.onShutDown = onShutDown;

            //TODO Prompt user for username, or new user
            //TODO Read in patients from disk if present
            //TODO Set up node (store no blockchain data)
            //TODO Check if patient data exists, read in if yes

            //TODO Start listening to user input

            CLI.DisplayLine("PhysicianHandler started");

            //TODO Replace with actual logic
            ShutDown();
        }

        public override void ShutDown()
        {
            CLI.DisplayLine("Shutting down PublisherHandler...");

            //TODO stop listening to user input
            //TODO save patient data to file
            //TODO Save blockchain to file

            CLI.DisplayLine("PublisherHandler shut down");

            //Invoke callback
            onShutDown();
        }

        public void HandleUserInput()
        {
            throw new NotImplementedException();
            //TODO Read in user input, generate new Transactions and send them to node according to input
        }

        public void RegisterPhysician(string publicKey, string country, string region, string name)
        {
            throw new NotImplementedException();
            //TODO Generate new RegistrationTransaction and add it to the blockchain
        }

        public void RegisterPatient()
        {
            throw new NotImplementedException();
            //TODO Generate new RegistrationTransaction and add it to the blockchain
        }

        public void AddTreatment(Guid Patient)
        {
            throw new NotImplementedException();
            //TODO Generate new TreatmentTransaction and add it to the blockchain
        }

        public void AddSymptomToTreatment(Guid Treatment)
        {
            throw new NotImplementedException();
            //TODO Generate new SymptomTransaction and add it to the blockchain
        }

        public void AddDiagnosesToTreatment(Guid Treatment)
        {
            throw new NotImplementedException();
            //TODO Generate new DiagnosesTransaction and add it to the blockchain
        }
    }
}
