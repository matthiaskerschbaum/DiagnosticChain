﻿using Blockchain.Interfaces;
using Blockchain.Transactions;
using Shared;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Utilities
{
    public class TransactionGenerator
    {
        public Guid SenderAddress;
        public RSAParameters privateKey;

        public TransactionGenerator()
        {

        }

        public TransactionGenerator(RSAParameters privateKey)
        {
            this.privateKey = privateKey;
        }

        public TransactionGenerator(Guid SenderAddress, RSAParameters privateKey)
        {
            this.SenderAddress = SenderAddress;
            this.privateKey = privateKey;
        }

        public ITransaction InitializeAsNewPublisher(RSAParameters PublicKey, string Country, string Region, string EntityName)
        {
            SenderAddress = Guid.NewGuid();
            ITransaction registration =
                new PublisherRegistrationTransaction()
                {
                    TransactionId = SenderAddress
                    ,
                    Timestamp = DateTime.UtcNow
                    ,
                    Type = TransactionType.PUBLISHER
                    ,
                    PublicKey = PublicKey
                    ,
                    Country = Country
                    ,
                    Region = Region
                    ,
                    EntityName = EntityName
                };

            return SignTransaction(registration);
        }

        public ITransaction InitializeAsNewPhysician(RSAParameters PublicKey, string Country, string Region, string Name)
        {
            SenderAddress = Guid.NewGuid();
            ITransaction registration =
                new PhysicianRegistrationTransaction()
                {
                    TransactionId = SenderAddress
                    ,
                    Timestamp = DateTime.UtcNow
                    ,
                    Type = TransactionType.PHYSICIAN
                    ,
                    PublicKey = PublicKey
                    ,
                    Country = Country
                    ,
                    Region = Region
                    ,
                    Name = Name
                };

            return SignTransaction(registration);
        }

        private ITransaction BasicSetup(ITransaction transaction)
        {
            transaction.TransactionId = Guid.NewGuid();
            transaction.Timestamp = DateTime.UtcNow;
            return transaction;
        }

        private ITransaction SignTransaction(ITransaction transaction)
        {
            transaction.SenderAddress = SenderAddress;
            transaction.Sign(privateKey);
            return transaction;
        }

        public ITransaction GenerateTreatmentTransaction(Guid PhysicianAddress, Guid PatientAddress)
        {
            var transaction = (TreatmentTransaction)BasicSetup(new TreatmentTransaction());
            transaction.Type = TransactionType.TREATMENT;
            transaction.PhysicianAddress = PhysicianAddress;
            transaction.PatientAddress = PatientAddress;
            return SignTransaction(transaction);
        }

        public ITransaction GenerateSymptomTransaction(Guid TreatmentTransactionAddress, List<string> Symptoms)
        {
            var transaction = (SymptomsTransaction)BasicSetup(new SymptomsTransaction());
            transaction.Type = TransactionType.SYMPTOM;
            transaction.TreatmentTransactionAddress = TreatmentTransactionAddress;
            transaction.Symptoms = Symptoms;
            return SignTransaction(transaction);
        }

        public ITransaction GenerateDiagnosesTransaction(Guid TreatmentTransactionAddress, List<string> Diagnoses)
        {
            var transaction = (DiagnosesTransaction)BasicSetup(new DiagnosesTransaction());
            transaction.Type = TransactionType.DIAGNOSES;
            transaction.TreatmentTransactionAddress = TreatmentTransactionAddress;
            transaction.Diagnoses = Diagnoses;
            return SignTransaction(transaction);
        }

        public ITransaction GeneratePublisherRegistrationTransaction(RSAParameters PublicKey, string Country, string Region, string EntityName)
        {
            var transaction = (PublisherRegistrationTransaction)BasicSetup(new PublisherRegistrationTransaction());
            transaction.Type = TransactionType.PUBLISHER;
            transaction.PublicKey = PublicKey;
            transaction.Country = Country;
            transaction.Region = Region;
            transaction.EntityName = EntityName;
            return SignTransaction(transaction);
        }

        public ITransaction GeneratePhysicianRegistrationTransaction(RSAParameters PublicKey, string Country, string Region, string Name)
        {
            var transaction = (PhysicianRegistrationTransaction)BasicSetup(new PhysicianRegistrationTransaction());
            transaction.Type = TransactionType.PHYSICIAN;
            transaction.PublicKey = PublicKey;
            transaction.Country = Country;
            transaction.Region = Region;
            transaction.Name = Name;
            return SignTransaction(transaction);
        }

        public ITransaction GeneratePatientRegistrationTransaction(string Country, string Region, String Birthyear)
        {
            var transaction = (PatientRegistrationTransaction)BasicSetup(new PatientRegistrationTransaction());
            transaction.Type = TransactionType.PATIENT;
            transaction.Country = Country;
            transaction.Region = Region;
            transaction.Birthyear = Birthyear;
            return SignTransaction(transaction);
        }

        public ITransaction GenerateVotingTransaction(Guid TransactionAddress, bool Vote)
        {
            var transaction = (VotingTransaction)BasicSetup(new VotingTransaction());
            transaction.Type = TransactionType.VOTING;
            transaction.TransactionAddress = TransactionAddress;
            transaction.Vote = Vote;
            return SignTransaction(transaction);
        }
    }
}
