using Blockchain.Entities;
using Blockchain.Interfaces;
using Blockchain.VotingMechanisms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Utilities
{
    public class ParticipantHandler
    {
        //TODO Ausprogrammieren
        //Der speichert eine Liste aller bekannter Publisher, Physicians und Patienten und kann Transaktionen handeln und validieren

        public List<Publisher> confirmedPublishers = new List<Publisher>();
        public List<Physician> confirmedPhysicians = new List<Physician>();
        public List<Patient> confirmedPatients = new List<Patient>();

        public List<Publisher> proposedPublishers = new List<Publisher>();
        public List<Physician> proposedPhysicians = new List<Physician>();

        public List<Vote> pendingVotes = new List<Vote>();

        private readonly IVotingMechanism votingMechanism = new DefaultVotingMechanism();

        public ParticipantHandler Clone()
        {
            var ret = new ParticipantHandler();
            ret.confirmedPublishers = confirmedPublishers.Select(p => p.Clone()).ToList();
            ret.confirmedPhysicians = confirmedPhysicians.Select(p => p.Clone()).ToList();
            ret.confirmedPatients = confirmedPatients.Select(p => p.Clone()).ToList();
            ret.proposedPublishers = proposedPublishers.Select(p => p.Clone()).ToList();
            ret.proposedPhysicians = proposedPhysicians.Select(p => p.Clone()).ToList();
            ret.pendingVotes = pendingVotes.Select(v => v.Clone()).ToList();

            return ret;
        }

        public void AddPatient(Patient patient)
        {
            confirmedPatients.Add(patient);
        }

        public bool CastVoteAgainstPhysician(Guid physicianAddress, Guid senderAddress)
        {
            return votingMechanism.CastVoteAgainstPhysician(this, physicianAddress, senderAddress);
        }

        public bool CastVoteAgainstPublisher(Guid publisherAddress, Guid senderAddress)
        {
            return votingMechanism.CastVoteAgainstPublisher(this, publisherAddress, senderAddress);
        }

        public bool CastVoteForPhysician(Guid physicianAddress, Guid senderAddress)
        {
            return votingMechanism.CastVoteForPhysician(this, physicianAddress, senderAddress);
        }

        public bool CastVoteForPublisher(Guid publisherAddress, Guid senderAddress)
        {
            return votingMechanism.CastVoteForPublisher(this, publisherAddress, senderAddress);
        }

        public List<Physician> GetConfirmedPhysicians()
        {
            confirmedPhysicians.Sort((x,y) => x.Address.CompareTo(y.Address));
            return confirmedPhysicians;
        }

        public List<Publisher> GetConfirmedPublishers()
        {
            confirmedPublishers.Sort((x,y) => x.Address.CompareTo(y.Address));
            return confirmedPublishers;
        }

        public List<Patient> GetPatients()
        {
            confirmedPatients.Sort((x,y) => x.Address.CompareTo(y.Address));
            return confirmedPatients;
        }

        public RSAParameters GetPhysicianKey(Guid publisher)
        {
            var hit = (from p in confirmedPhysicians
                       where p.Address == publisher
                       select p.PublicKey).FirstOrDefault();

            return hit;
        }

        public List<Physician> GetProposedPhysicians()
        {
            proposedPhysicians.Sort((x, y) => x.Address.CompareTo(y.Address));
            return proposedPhysicians;
        }

        public List<Publisher> GetProposedPublishers()
        {
            proposedPublishers.Sort((x,y) => x.Address.CompareTo(y.Address));
            return proposedPublishers;
        }

        public RSAParameters GetPublisherKey(Guid publisher)
        {
            var hit = (from p in confirmedPublishers
                       where p.Address == publisher
                       select p.PublicKey).FirstOrDefault();

            return hit;
        }

        public RSAParameters GetSenderKey(Guid senderAddress)
        {
            var hitPublisher = from p in confirmedPublishers
                               where p.Address == senderAddress
                               select p.PublicKey;
            var hitPhysician = from p in confirmedPhysicians
                               where p.Address == senderAddress
                               select p.PublicKey;

            return hitPublisher.Count() > 0 ? hitPublisher.First() : hitPhysician.FirstOrDefault();
        }

        public bool HasPatient(Guid patientAddress)
        {
            var hit = from p in confirmedPatients
                      where p.Address == patientAddress
                      select p;

            return hit.Count() > 0;
        }

        public bool HasPhysician(Guid senderAddress)
        {
            var hit = from p in confirmedPhysicians
                      where p.Address == senderAddress
                      select p;

            return hit.Count() > 0;
        }

        public bool HasPublisher(Guid publisher)
        {
            var hit = from p in confirmedPublishers
                      where p.Address == publisher
                      select p;

            return hit.Count() > 0;
        }

        public bool HasSender(Guid senderAddress)
        {
            return HasPublisher(senderAddress) || HasPhysician(senderAddress);
        }

        public bool IsEmpty()
        {
            return confirmedPublishers.Count == 0
                && confirmedPhysicians.Count == 0
                && confirmedPatients.Count == 0;
        }

        public bool IsVotablePublisher(Guid publisher)
        {
            var allPublishers = confirmedPublishers.Union(proposedPublishers);

            var hit = from p in allPublishers
                      where p.Address == publisher
                      select p;

            return hit.Count() > 0;
        }

        public bool IsVotablePhysician(Guid physician)
        {
            var allPhyisicians = confirmedPhysicians.Union(proposedPhysicians);

            var hit = from p in allPhyisicians
                      where p.Address == physician
                      select p;

            return hit.Count() > 0;
        }

        public bool ProcessTransaction(ITransaction t, List<Chain> context)
        {
            return t.ProcessContract(this, context);
        }

        public void ProposePhysician(Physician physician)
        {
            proposedPhysicians.Add(physician);
        }

        public void ProposePublisher(Publisher publisher)
        {
            proposedPublishers.Add(publisher);
        }

    }
}
