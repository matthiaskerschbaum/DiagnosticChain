using Blockchain;
using Blockchain.Utilities;
using NodeManagement.Entities;
using Shared;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeManagement
{
    public class Node : IServerAddressRepository
    {
        //User data
        public UserProperties User;

        //Blockchain data
        protected Chain chain = new Chain();
        public ParticipantHandler participantHandler = new ParticipantHandler();

        //Known nodes
        public List<ServerAddress> knownNodes = new List<ServerAddress>();

        public void AddServerAddress(ServerAddress address)
        {
            if (!knownNodes.Contains(address))
            {
                knownNodes.Add(address);
                //TODO Propagate node to all known nodes
            }
        }

        public void DeleteServerAddress(ServerAddress address)
        {
            knownNodes.Remove(address);
        }

        public ChainStatistics GetChainStatisics()
        {
            return new ChainStatistics(chain);
        }

        public List<Blockchain.Entities.Physician> GetConfirmedPhysicians()
        {
            return participantHandler.GetConfirmedPhysicians();
        }

        public List<Blockchain.Entities.Publisher> GetConfirmedPublishers()
        {
            return participantHandler.GetConfirmedPublishers();
        }

        public List<ServerAddress> GetKnownNodes()
        {
            return knownNodes;
        }

        public List<Blockchain.Entities.Patient> GetPatients()
        {
            return participantHandler.GetPatients();
        }

        public List<Blockchain.Entities.Physician> GetProposedPhysicians()
        {
            return participantHandler.GetProposedPhysicians();
        }

        public List<Blockchain.Entities.Publisher> GetProposedPublishers()
        {
            return participantHandler.GetProposedPublishers();
        }

        public List<ServerAddress> GetServerAddresses()
        {
            return knownNodes;
        }

        public bool HasServerAddress(ServerAddress address)
        {
            return knownNodes.Contains(address);
        }

        public bool IsChainInitialized()
        {
            return !chain.IsEmpty();
        }

        public bool LoadChain()
        {
            chain = new Chain(FileHandler.Read(User.Username + FileHandler.ChainPath));
            participantHandler = new ParticipantHandler();
            var success = chain.ProcessContracts(participantHandler, new List<Chain>() { chain });
            return success;
        }

        public void RequestChainAt(Uri node)
        {
            throw new NotImplementedException();
            //TODO Verbindung zu angegebenem Node aufbauen und eine SetupMessage schicken
            //TODO ErrorHandling, wenn Node nicht erreichbar ist
            //TODO Antwort von RequestingNode => Komplette Chain. Objekt aufbauen und in chain abspeicher
            //TODO Node zu KnownNodes hinzufügen, wenn Verbindung und Anfrage erfolgreich
        }

        public void SaveChain()
        {
            FileHandler.Save(User.Username + FileHandler.ChainPath, chain.AsXML());
        }

        public bool ValidateChain()
        {
            return chain.ValidateContextual(new ParticipantHandler(), new List<Chain>() { chain });
        }
    }
}
