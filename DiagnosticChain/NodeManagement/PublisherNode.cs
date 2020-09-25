using Blockchain;
using Blockchain.Interfaces;
using Blockchain.Transactions;
using Blockchain.Utilities;
using Grpc.Core;
using NetworkingFacilities.Clients;
using NodeManagement.Entities;
using Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace NodeManagement
{
    public class PublisherNode : Node, IChainManipulator
    {
        //Blockchain utilities
        internal TransactionBuffer transactionBuffer = new TransactionBuffer();
        
        //Buffering for transaction handling
        private ParticipantHandler participantHandler_Buffered = new ParticipantHandler(); //For Processing of transactions that have been received, but not published yet
        Semaphore manipulateBufferedTransactions = new Semaphore(1, 1);
        Semaphore manipulateChain = new Semaphore(1, 1);

        //Parallel running tasks
        internal Timer blockGenerator;
        internal Timer blockProcessor;

        public PublisherNode() :base()
        {

        }

        public PublisherNode(ServerAddress selfAddress) : base(selfAddress)
        {

        }

        public void BundleOpenTransactionsManually()
        {
            transactionBuffer.BundleTransactions(this);
        }

        public void EvaluateParkedTransactions()
        {
            List<ITransaction> readyToProcess = participantHandler.EvaluateParkedTransactions();

            manipulateBufferedTransactions.WaitOne();

            foreach (var t in readyToProcess)
            {
                transactionBuffer.RecordTransaction(t);
            }

            manipulateBufferedTransactions.Release();
        }

        public Chain GetChain()
        {
            return chain;
        }

        public Chain GetChainDelta(long currentIndex)
        {
            var blockDelta = (from b in chain.GetBlocks()
                              where b.Index > currentIndex
                              select b).ToList();
            blockDelta.Sort((x, y) => x.Index.CompareTo(y.Index));
            
            var ret = new Chain();
            foreach (var b in blockDelta)
            {
                ret.Add(b);
            }

            return ret;
        }

        public long GetCurrentChainIndex()
        {
            return chain.Blockhead == null ? -1 : chain.Blockhead.Index;
        }

        public List<Blockchain.Entities.Physician> GetPendingPhysicians()
        {
            return participantHandler.proposedPhysicians;
        }

        public void InitializeEmptyChain(ITransaction initialPublisher, ITransaction initialVote)
        {
            if (chain.IsEmpty())
            {
                manipulateBufferedTransactions.WaitOne();

                transactionBuffer.RecordTransaction(initialPublisher);
                transactionBuffer.RecordTransaction(initialVote);

                transactionBuffer.BundleTransactions(this);

                manipulateBufferedTransactions.Release();

                PublishOpenBlocks(this);
            }
        }

        new public bool LoadChain()
        {
            var success = base.LoadChain();
            if (success) participantHandler_Buffered = participantHandler.Clone();
            return success;
        }

        //Returns whether or not the chain has been added to the blockchain
        public bool OnReceiveChain(Chain chain)
        {
            var success = false;
            var transactions = chain.GetTransactions();

            manipulateBufferedTransactions.WaitOne();
            manipulateChain.WaitOne();

            var participantHandler_Clone = participantHandler.Clone();
            success = chain.ValidateContextual(participantHandler_Clone, new List<Chain>() { this.chain });

            if (success)
            {
                var deletedBlocks = this.chain.Add(chain);
                var bufferedTransactions = transactionBuffer.Peek().GetTransactions();
                var receivedTransaction = chain.GetTransactions();

                foreach (var b in deletedBlocks)
                {
                    foreach (var t in b.TransactionList)
                    {
                        if (!bufferedTransactions.Where(bt => bt.TransactionId == t.TransactionId).Any()
                            || !receivedTransaction.Where(rt => rt.TransactionId == t.TransactionId).Any())
                        {
                            transactionBuffer.RecordTransaction(t);
                        }
                    }
                }

                var transactionsToUnbuffer = bufferedTransactions.Where(t => receivedTransaction.Where(rt => rt.TransactionId == t.TransactionId).Any());
                foreach (var t in transactionsToUnbuffer)
                {
                    transactionBuffer.UnrecordTransaction(t);
                }

                participantHandler = new ParticipantHandler();
                this.chain.ProcessContracts(participantHandler, new List<Chain>() { });

                participantHandler_Buffered = participantHandler.Clone();
                transactionBuffer.Peek().ProcessContracts(participantHandler_Buffered, new List<Chain>() { this.chain });
            }

            manipulateChain.Release();
            manipulateBufferedTransactions.Release();

            CLI.DisplayLine("Added " + transactions.Count + " transactions " + (success ? "successfully" : "unsuccessfully"));
            foreach (var t in transactions)
            {
                CLI.DisplayLine("\t" + t.AsString());
            }
            return success;
        }

        public bool OnReceiveTransaction(ITransaction transaction)
        {
            var success = true;
            var parked = false;

            manipulateBufferedTransactions.WaitOne();

            var peekTransactions = transactionBuffer.Peek();
            if (transaction.ValidateContextual(participantHandler_Buffered, new List<Chain>() { chain, peekTransactions }))
            {
                
                try
                {
                    success &= transaction.ProcessContract(participantHandler_Buffered, new List<Chain>() { chain, peekTransactions });
                    if (success) transactionBuffer.RecordTransaction(transaction);
                } catch (TransactionParkedException)
                {
                    success = false;
                    parked = true;

                    if (transaction.GetType() == typeof(PatientRegistrationTransaction)) participantHandler.parkedPatients.Add((PatientRegistrationTransaction)transaction);
                    else if (transaction.GetType() == typeof(TreatmentTransaction)) participantHandler.parkedTreatments.Add((TreatmentTransaction)transaction);
                    else if (transaction.GetType() == typeof(SymptomsTransaction)) participantHandler.parkedSymptoms.Add((SymptomsTransaction)transaction);
                }
            }
            if (parked)
            {
                var count = participantHandler_Buffered.parkedPatients.Count()
                    + participantHandler_Buffered.parkedTreatments.Count()
                    + participantHandler_Buffered.parkedSymptoms.Count();

                CLI.DisplayLine("Parked " + count + " transactions ");
            }
            else
            {
                CLI.DisplayLine("Recorded " + transactionBuffer.openTransactions.Count + " transactions " + (success ? "successfully" : "unsuccessfully"));
            }

            manipulateBufferedTransactions.Release();

            return success;
        }

        public void PublishChain(Chain chain)
        {
            throw new NotImplementedException();
            //TODO Chain an alle KnownNodes schicken
        }

        public void PublishOpenBlocks(object state)
        {
            //TODO Überprüfen, ob Node überhaupt publishen darf (Registrierung eines neuen Publishers muss durchgehen)

            manipulateBufferedTransactions.WaitOne();
            manipulateChain.WaitOne();
            Chain appendix = null;
            if (transactionBuffer.HasBlocks())
            {
                appendix = new Chain();

                while (transactionBuffer.HasBlocks())
                {
                    var nextBlock = transactionBuffer.GetNextBlock();
                    nextBlock.Publisher = User.UserAddress;

                    if (chain.IsEmpty())
                    {
                        nextBlock.Index = 0;
                        nextBlock.PreviousBlock = null;
                        nextBlock.PreviousHash = null;
                    }
                    else if (appendix.IsEmpty())
                    {
                        nextBlock.Index = chain.Blockhead.Index + 1;
                        nextBlock.PreviousHash = chain.Blockhead.Hash;
                    }
                    else
                    {
                        nextBlock.Index = appendix.Blockhead.Index + 1;
                        nextBlock.PreviousBlock = appendix.Blockhead;
                        nextBlock.PreviousHash = appendix.Blockhead.Hash;
                    }

                    nextBlock.Sign(User.Keys.PrivateKey);
                    appendix.Add(nextBlock);
                }

                var success = appendix.ProcessContracts(participantHandler, new List<Chain>() { chain });
                participantHandler_Buffered = participantHandler.Clone();

                foreach (var n in knownNodes)
                {
                    try
                    {
                        new PublisherClient(n, selfAddress).SendChain(appendix);
                    }
                    catch (RpcException)
                    { //TODO Nodes flaggen, von denen keine Antwort kommt 
                    }
                }

                CLI.DisplayLine("Published " + appendix.GetTransactions().Count + " transactions " + (success ? "successfully" : "unsuccessfully"));
                chain.Add(appendix);
            }

            manipulateChain.Release();
            manipulateBufferedTransactions.Release();
        }

        public void StartPublishing()
        {
            blockGenerator = new Timer(transactionBuffer.BundleTransactions, null, 5000, 5000);
            blockProcessor = new Timer(PublishOpenBlocks, null, 7000, 5000);
        }

        public void StopPublishing()
        {
            blockGenerator.Dispose();
            blockProcessor.Dispose();
        }
    }
}
