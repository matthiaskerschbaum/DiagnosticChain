﻿using Blockchain;
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
                transactionBuffer.RecordTransaction(initialPublisher);
                transactionBuffer.RecordTransaction(initialVote);

                transactionBuffer.BundleTransactions(this);
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

            //TODO Chain mit aktuellem Stand der Blockchain abgleichen => Collisions resolven (Nicht einfügen wenn aktueller Stand neuer ist als die empfangene Chain)
            if (success)
            {
                success &= chain.ProcessContracts(participantHandler, new List<Chain>() { this.chain });
                chain.ProcessContracts(participantHandler_Buffered, new List<Chain>() { this.chain });
                success &= this.chain.Add(chain);
            }

            //TODO Wenn eingefügt wird: UnpublishedBlocks auflösen (Transaktionen zurück in OpenTransactions stellen)
            //TODO Wenn Chain übernommen wird => Blocks die eventuell aus aktueller Chain rausfliegen, werden von Chain zurückgegeben
            //TODO Wenn Chain eingefügt wird: Transaktionen durchgehen, und alle offenen Transaktionen löschen, die in neuer Chain enthalten sind
            //TODO Wenn Teile der alten Chain wegfliegen: Transaktionen durchgehen und die in offene Transaktionen mit aufnehmen, die nicht in neuer Chain enthalten sind

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
            manipulateBufferedTransactions.WaitOne();
            var peekTransactions = transactionBuffer.Peek();
            if (transaction.ValidateContextual(participantHandler_Buffered, new List<Chain>() { chain, peekTransactions }))
            {
                transactionBuffer.RecordTransaction(transaction);
                success &= transaction.ProcessContract(participantHandler_Buffered, new List<Chain>() { chain, peekTransactions });
            }
            CLI.DisplayLine("Recorded " + transactionBuffer.openTransactions.Count + " transactions " + (success ? "successfully" : "unsuccessfully"));
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
