using Blockchain.Entities;
using Blockchain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain
{
    public class ParticipantHandler
    {
        //TODO Ausprogrammieren
        //Der speichert eine Liste aller bekannter Publisher, Physicians und Patienten und kann Transaktionen handeln und validieren

        private List<Publisher> confirmedPublishers = new List<Publisher>();
        private List<Physician> confirmedPhysicians = new List<Physician>(); //UserId, Public Key

        private List<Patient> confirmedPatients = new List<Patient>();

        private List<Publisher> proposedPublishers = new List<Publisher>(); //UserId, Public Key 
        private List<Physician> proposedPhysicians = new List<Physician>(); //UserId, Public Key

        private List<Vote> pendingVotes = new List<Vote>();

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

        internal void AddPatient(Patient patient)
        {
            confirmedPatients.Add(patient);
        }

        internal void ProposePhysician(Physician physician)
        {
            proposedPhysicians.Add(physician);
        }

        internal void ProposePublisher(Publisher publisher)
        {
            proposedPublishers.Add(publisher);
        }

        internal bool HasPublisher(Guid publisher)
        {
            var hit = from p in confirmedPublishers
                      where p.Address == publisher
                      select p;

            return hit.Count() > 0;
        }

        //Geht Physicians durch, und schaut ob die existieren
        internal bool HasPhysician(Guid senderAddress)
        {
            var hit = from p in confirmedPhysicians
                      where p.Address == senderAddress
                      select p;

            return hit.Count() > 0;
        }

        //Geht Publisher und Physicians durch, und schaut ob die existieren
        internal bool HasSender(Guid senderAddress)
        {
            var hitPublisher = from p in confirmedPublishers
                               where p.Address == senderAddress
                               select p;
            var hitPhysician = from p in confirmedPhysicians
                               where p.Address == senderAddress
                               select p;

            return hitPublisher.Count() > 0 || hitPhysician.Count() > 0;
        }

        internal bool HasPatient(Guid patientAddress)
        {
            var hit = from p in confirmedPatients
                      where p.Address == patientAddress
                      select p;

            return hit.Count() > 0;
        }

        internal RSAParameters GetPublisherKey(Guid publisher)
        {
            var hit = (from p in confirmedPublishers
                       where p.Address == publisher
                       select p.PublicKey).FirstOrDefault();

            return hit;
        }

        internal RSAParameters GetPhysicianKey(Guid publisher)
        {
            var hit = (from p in confirmedPhysicians
                       where p.Address == publisher
                       select p.PublicKey).FirstOrDefault();

            return hit;
        }

        internal RSAParameters GetSenderKey(Guid senderAddress)
        {
            var hitPublisher = from p in confirmedPublishers
                               where p.Address == senderAddress
                               select p.PublicKey;
            var hitPhysician = from p in confirmedPublishers
                               where p.Address == senderAddress
                               select p.PublicKey;

            return hitPublisher.Count() > 0 ? hitPublisher.First() : hitPhysician.FirstOrDefault();
        }

        //Verarbeitet je nach Transaktionstyp (Publisher werden hinzugefügt, Votes werden ausgewertet etc.)
        internal bool HandleTransaction(ITransaction t, List<Chain> context)
        {
            return t.HandleContextual(this, context);
        }

        internal bool IsVotablePublisher(Guid publisher)
        {
            var allPublishers = confirmedPublishers.Union(proposedPublishers);

            var hit = from p in allPublishers
                      where p.Address == publisher
                      select p;

            return hit.Count() > 0;
        }

        internal bool IsVotablePhysician(Guid physician)
        {
            var allPhyisicians = confirmedPhysicians.Union(proposedPhysicians);

            var hit = from p in allPhyisicians
                      where p.Address == physician
                      select p;

            return hit.Count() > 0;
        }

        internal bool CastVoteForPublisher(Guid publisher, Guid senderAddress)
        {
            //If the publisher has already been confirmed, do nothing
            if (HasPublisher(publisher)) return true;
            if (!(proposedPublishers.Where(p => p.Address == publisher).Count() > 0)) return false;

            //If the publisher is still proposed
            //Add Vote to list of votes (if not already present from the current sender)
            var votesFromSender = from v in pendingVotes
                                  where v.VoteFor == publisher && v.VoteFrom == senderAddress
                                  select v;

            if (votesFromSender.Count() > 0)
            {
                var hit = votesFromSender.FirstOrDefault();
                hit.Confirmed = true;
            } else
            {
                pendingVotes.Add(new Vote()
                {
                    VoteFor = publisher
                    ,VoteFrom = senderAddress
                    ,Confirmed = true
                });
            }

            //Count list of valid votes for publisher => Valid votes must be more than half of currently confirmed publishers
            //If publisher is confirmed: Transfer to confirmed publishers, remove all votes from list
            var votesForPublisher = from v in pendingVotes
                                    where v.VoteFor == publisher && v.Confirmed
                                    select v;

            if (votesForPublisher.Count() > Math.Floor(pendingVotes.Count() / 2.0))
            {
                var tranfer = proposedPublishers.Where(p => p.Address == publisher);
                confirmedPublishers.AddRange(tranfer);
                proposedPublishers.RemoveAll(p => p.Address == publisher);
                pendingVotes.RemoveAll(v => v.VoteFor == publisher);
            }

            return true;
        }

        internal bool CastVoteAgainstPublisher(Guid publisher, Guid senderAddress)
        {
            //If the publisher is not confirmed, do nothing
            if (!HasPublisher(publisher)) return false;

            //Add Vote to list of votes (if not already present from the current sender)
            var votesFromSender = from v in pendingVotes
                                  where v.VoteFor == publisher && v.VoteFrom == senderAddress
                                  select v;

            if (votesFromSender.Count() > 0)
            {
                var hit = votesFromSender.FirstOrDefault();
                hit.Confirmed = false;
            }
            else
            {
                pendingVotes.Add(new Vote()
                {
                    VoteFor = publisher
                    ,
                    VoteFrom = senderAddress
                    ,
                    Confirmed = false
                });
            }

            //Count list of valid votes against publisher => Valid votes must be more than half of currently confirmed publishers
            //If publisher is dismissed: Delete from List
            var votesAgainstPublisher = from v in pendingVotes
                                    where v.VoteFor == publisher && !v.Confirmed
                                    select v;

            if (votesAgainstPublisher.Count() > Math.Floor(confirmedPublishers.Count() / 2.0))
            {
                confirmedPublishers.RemoveAll(p => p.Address == publisher);
                pendingVotes.RemoveAll(v => v.VoteFor == publisher);
            }

            return true;
        }

        internal bool CastVoteForPhysician(Guid physician, Guid senderAddress)
        {
            //If the physician has already been confirmed, do nothing
            if (HasPhysician(physician)) return true;
            if (!(proposedPhysicians.Where(p => p.Address == physician).Count() > 0)) return false;

            //If the physician is still proposed
            //Add Vote to list of votes (if not already present from the current sender)
            var votesFromSender = from v in pendingVotes
                                  where v.VoteFor == physician && v.VoteFrom == senderAddress
                                  select v;

            if (votesFromSender.Count() > 0)
            {
                var hit = votesFromSender.FirstOrDefault();
                hit.Confirmed = true;
            }
            else
            {
                pendingVotes.Add(new Vote()
                {
                    VoteFor = physician
                    ,
                    VoteFrom = senderAddress
                    ,
                    Confirmed = true
                });
            }

            //Count list of valid votes for physician => Valid votes must be at least 3 in total (or equal to number of confirmed publishers)
            //If publisher is confirmed: Transfer to confirmed publishers, remove all votes from list
            var votesForPhysician = from v in pendingVotes
                                    where v.VoteFor == physician && v.Confirmed
                                    select v;

            if (votesForPhysician.Count() > 3 || votesForPhysician.Count() == confirmedPublishers.Count())
            {
                var tranfer = proposedPhysicians.Where(p => p.Address == physician);
                confirmedPhysicians.AddRange(tranfer);
                proposedPhysicians.RemoveAll(p => p.Address == physician);
                pendingVotes.RemoveAll(v => v.VoteFor == physician);
            }

            return true;
        }

        internal bool CastVoteAgainstPhysician(Guid physician, Guid senderAddress)
        {
            //If the publisher is not confirmed, do nothing
            if (!HasPhysician(physician)) return false;

            //Add Vote to list of votes (if not already present from the current sender)
            var votesFromSender = from v in pendingVotes
                                  where v.VoteFor == physician && v.VoteFrom == senderAddress
                                  select v;

            if (votesFromSender.Count() > 0)
            {
                var hit = votesFromSender.FirstOrDefault();
                hit.Confirmed = false;
            }
            else
            {
                pendingVotes.Add(new Vote()
                {
                    VoteFor = physician
                    ,
                    VoteFrom = senderAddress
                    ,
                    Confirmed = false
                });
            }

            //Count list of valid votes against publisher => Valid votes must be more than half of currently confirmed publishers
            //If publisher is dismissed: Delete from List
            var votesAgainstPhysician = from v in pendingVotes
                                        where v.VoteFor == physician && !v.Confirmed
                                        select v;

            if (votesAgainstPhysician.Count() > Math.Floor(confirmedPublishers.Count() / 2.0))
            {
                confirmedPhysicians.RemoveAll(p => p.Address == physician);
                pendingVotes.RemoveAll(v => v.VoteFor == physician);
            }

            return true;
        }

    }
}
