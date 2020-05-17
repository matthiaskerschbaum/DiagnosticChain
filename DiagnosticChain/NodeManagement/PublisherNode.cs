using Blockchain;
using Blockchain.Interfaces;
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
        Semaphore mainpulateChain = new Semaphore(1, 1);

        //Parallel running tasks
        internal Timer blockGenerator;
        internal Timer blockProcessor;

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

            //TODO Publish Blocks stoppen
            mainpulateChain.WaitOne();

            //TODO Chain validieren (Stimmen Publisher, Physicians, Patients etc.)
            success = chain.ValidateContextual(participantHandler, new List<Chain>() { this.chain });

            //TODO Chain mit aktuellem Stand der Blockchain abgleichen => Collisions resolven (Nicht einfügen wenn aktueller Stand neuer ist als die empfangene Chain)
            if (success)
            {
                success &= this.chain.Add(chain);
            }

            //TODO Wenn eingefügt wird: UnpublishedBlocks auflösen (Transaktionen zurück in OpenTransactions stellen)
            //TODO Wenn Chain übernommen wird => Blocks die eventuell aus aktueller Chain rausfliegen, werden von Chain zurückgegeben
            //TODO Wenn Chain eingefügt wird: Transaktionen durchgehen, und alle offenen Transaktionen löschen, die in neuer Chain enthalten sind
            //TODO Wenn Teile der alten Chain wegfliegen: Transaktionen durchgehen und die in offene Transaktionen mit aufnehmen, die nicht in neuer Chain enthalten sind

            mainpulateChain.Release();

            return success;
        }

        public void OnReceiveRegistrationRequest(Uri requestingNode)
        {
            throw new NotImplementedException();
            //TODO Node zu KnownNodes hinzufügen
            //TODO ACK-Message zurück schicken
            //TODO Neuen Node an alle KnownNodes verbreiten
        }

        public void OnReceiveSetupRequest(Uri requestingNode)
        {
            throw new NotImplementedException();
            //TODO Node zu KnownNodes hinzufügen
            //TODO Blockchain über's Netz verschicken (synchron)
            //TODO Neuen Node an alle KnownNodes verbreiten
        }

        public void OnReceiveTransaction(ITransaction transaction)
        {
            manipulateBufferedTransactions.WaitOne();
            var peekTransactions = transactionBuffer.Peek();
            if (transaction.ValidateContextual(participantHandler_Buffered, new List<Chain>() { chain, peekTransactions }))
            {
                transactionBuffer.RecordTransaction(transaction);
                transaction.ProcessContract(participantHandler_Buffered, new List<Chain>() { chain, peekTransactions });
            }
            manipulateBufferedTransactions.Release();
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
            mainpulateChain.WaitOne();
            Chain appendix = null;
            if (transactionBuffer.HasBlocks())
            {
                appendix = new Chain();

                while (transactionBuffer.HasBlocks())
                {
                    var nextBlock = transactionBuffer.GetNextBlock();
                    nextBlock.Publisher = User.PublisherAddress;

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

                appendix.ProcessContracts(participantHandler, new List<Chain>() { chain });
                participantHandler_Buffered = participantHandler.Clone();

                foreach (var n in knownNodes)
                {
                    try
                    {
                        new PublisherClient(n).SendChain(appendix);
                    }
                    catch (RpcException)
                    { //TODO Nodes flaggen, von denen keine Antwort kommt 
                    }
                }

                chain.Add(appendix);
            }

            mainpulateChain.Release();
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
