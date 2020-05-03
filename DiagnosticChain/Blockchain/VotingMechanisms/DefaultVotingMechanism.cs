using Blockchain.Entities;
using Blockchain.Interfaces;
using Blockchain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blockchain.VotingMechanisms
{
    class DefaultVotingMechanism : IVotingMechanism
    {
        public bool CastVoteAgainstPhysician(ParticipantHandler participantHandler, Guid physicianAddress, Guid senderAddress)
        {
            //If the publisher is not confirmed, do nothing
            if (!participantHandler.HasPhysician(physicianAddress)) return false;

            //Add Vote to list of votes (if not already present from the current sender)
            var votesFromSender = from v in participantHandler.pendingVotes
                                  where v.VoteFor == physicianAddress && v.VoteFrom == senderAddress
                                  select v;

            if (votesFromSender.Count() > 0)
            {
                var hit = votesFromSender.FirstOrDefault();
                hit.Confirmed = false;
            }
            else
            {
                participantHandler.pendingVotes.Add(new Vote()
                {
                    VoteFor = physicianAddress
                    ,
                    VoteFrom = senderAddress
                    ,
                    Confirmed = false
                });
            }

            //Count list of valid votes against publisher => Valid votes must be more than half of currently confirmed publishers
            //If publisher is dismissed: Delete from List
            var votesAgainstPhysician = from v in participantHandler.pendingVotes
                                        where v.VoteFor == physicianAddress && !v.Confirmed
                                        select v;

            if (votesAgainstPhysician.Count() > Math.Floor(participantHandler.confirmedPublishers.Count() / 2.0))
            {
                participantHandler.confirmedPhysicians.RemoveAll(p => p.Address == physicianAddress);
                participantHandler.pendingVotes.RemoveAll(v => v.VoteFor == physicianAddress);
            }

            return true;
        }

        public bool CastVoteAgainstPublisher(ParticipantHandler participantHandler, Guid publisherAddress, Guid senderAddress)
        {
            //If the publisher is not confirmed, do nothing
            if (!participantHandler.HasPublisher(publisherAddress)) return false;

            //Add Vote to list of votes (if not already present from the current sender)
            var votesFromSender = from v in participantHandler.pendingVotes
                                  where v.VoteFor == publisherAddress && v.VoteFrom == senderAddress
                                  select v;

            if (votesFromSender.Count() > 0)
            {
                var hit = votesFromSender.FirstOrDefault();
                hit.Confirmed = false;
            }
            else
            {
                participantHandler.pendingVotes.Add(new Vote()
                {
                    VoteFor = publisherAddress
                    ,
                    VoteFrom = senderAddress
                    ,
                    Confirmed = false
                });
            }

            //Count list of valid votes against publisher => Valid votes must be more than half of currently confirmed publishers
            //If publisher is dismissed: Delete from List
            var votesAgainstPublisher = from v in participantHandler.pendingVotes
                                        where v.VoteFor == publisherAddress && !v.Confirmed
                                        select v;

            if (votesAgainstPublisher.Count() > Math.Floor(participantHandler.confirmedPublishers.Count() / 2.0))
            {
                participantHandler.confirmedPublishers.RemoveAll(p => p.Address == publisherAddress);
                participantHandler.pendingVotes.RemoveAll(v => v.VoteFor == publisherAddress);
            }

            return true;
        }

        public bool CastVoteForPhysician(ParticipantHandler participantHandler, Guid physicianAddress, Guid senderAddress)
        {
            //If the physician has already been confirmed, do nothing
            if (participantHandler.HasPhysician(physicianAddress)) return true;
            if (!(participantHandler.proposedPhysicians.Where(p => p.Address == physicianAddress).Count() > 0)) return false;

            //If the physician is still proposed
            //Add Vote to list of votes (if not already present from the current sender)
            var votesFromSender = from v in participantHandler.pendingVotes
                                  where v.VoteFor == physicianAddress && v.VoteFrom == senderAddress
                                  select v;

            if (votesFromSender.Count() > 0)
            {
                var hit = votesFromSender.FirstOrDefault();
                hit.Confirmed = true;
            }
            else
            {
                participantHandler.pendingVotes.Add(new Vote()
                {
                    VoteFor = physicianAddress
                    ,
                    VoteFrom = senderAddress
                    ,
                    Confirmed = true
                });
            }

            //Count list of valid votes for physician => Valid votes must be at least 3 in total (or equal to number of confirmed publishers)
            //If publisher is confirmed: Transfer to confirmed publishers, remove all votes from list
            var votesForPhysician = from v in participantHandler.pendingVotes
                                    where v.VoteFor == physicianAddress && v.Confirmed
                                    select v;

            if (votesForPhysician.Count() > 3 || votesForPhysician.Count() == participantHandler.confirmedPublishers.Count())
            {
                var tranfer = participantHandler.proposedPhysicians.Where(p => p.Address == physicianAddress);
                participantHandler.confirmedPhysicians.AddRange(tranfer);
                participantHandler.proposedPhysicians.RemoveAll(p => p.Address == physicianAddress);
                participantHandler.pendingVotes.RemoveAll(v => v.VoteFor == physicianAddress);
            }

            return true;
        }

        public bool CastVoteForPublisher(ParticipantHandler participantHandler, Guid publisherAddress, Guid senderAddress)
        {
            //If the publisher has already been confirmed, do nothing
            if (participantHandler.HasPublisher(publisherAddress)) return true;
            if (!(participantHandler.proposedPublishers.Where(p => p.Address == publisherAddress).Count() > 0)) return false;

            //If the publisher is still proposed
            //Add Vote to list of votes (if not already present from the current sender)
            var votesFromSender = from v in participantHandler.pendingVotes
                                  where v.VoteFor == publisherAddress && v.VoteFrom == senderAddress
                                  select v;

            if (votesFromSender.Count() > 0)
            {
                var hit = votesFromSender.FirstOrDefault();
                hit.Confirmed = true;
            }
            else
            {
                participantHandler.pendingVotes.Add(new Vote()
                {
                    VoteFor = publisherAddress
                    ,
                    VoteFrom = senderAddress
                    ,
                    Confirmed = true
                });
            }

            //Count list of valid votes for publisher => Valid votes must be more than half of currently confirmed publishers
            //If publisher is confirmed: Transfer to confirmed publishers, remove all votes from list
            var votesForPublisher = from v in participantHandler.pendingVotes
                                    where v.VoteFor == publisherAddress && v.Confirmed
                                    select v;

            if (votesForPublisher.Count() > Math.Floor(participantHandler.pendingVotes.Count() / 2.0))
            {
                var tranfer = participantHandler.proposedPublishers.Where(p => p.Address == publisherAddress);
                participantHandler.confirmedPublishers.AddRange(tranfer);
                participantHandler.proposedPublishers.RemoveAll(p => p.Address == publisherAddress);
                participantHandler.pendingVotes.RemoveAll(v => v.VoteFor == publisherAddress);
            }

            return true;
        }
    }
}
