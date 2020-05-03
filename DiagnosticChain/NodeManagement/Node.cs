using Blockchain;
using Blockchain.Interfaces;
using Blockchain.Utilities;
using NodeManagement.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NodeManagement
{
    public class Node
    {
        //User data
        public UserProperties User;

        //Blockchain data
        private Chain chain;
        private TimeSpan archiveTime;

        //Blockchain utilities
        internal TransactionBuffer transactionBuffer;
        public ParticipantHandler participantHandler;

        //Parallel running tasks
        internal Timer blockGenerator;
        internal Timer blockProcessor;

        public void RegisterAt(Uri node)
        {
            throw new NotImplementedException();
            //TODO Verbindung zu angegebenem Node aufbauen und eine RegistrationMessage schicken
            //TODO ErrorHandling, wenn Node nicht erreichbar ist
            //TODO Node zu KnownNodes hinzufügen, wenn Verbindung und Anfrage erfolgreich
        }

        public void RequestChainAt(Uri node)
        {
            throw new NotImplementedException();
            //TODO Verbindung zu angegebenem Node aufbauen und eine SetupMessage schicken
            //TODO ErrorHandling, wenn Node nicht erreichbar ist
            //TODO Antwort von RequestingNode => Komplette Chain. Objekt aufbauen und in chain abspeicher
            //TODO Node zu KnownNodes hinzufügen, wenn Verbindung und Anfrage erfolgreich
        }

        public void OnReceiveRegistrationRequest(Uri requestingNode)
        {
            throw new NotImplementedException();
            //TODO Node zu KnownNodes hinzufügen
            //TODO ACK-Message zurück schicken
            //TODO Neuen Node an alle KnownNodes verbreiten
        }

        public bool IsChainInitialized()
        {
            return !chain.IsEmpty();
        }

        public void OnReceiveSetupRequest(Uri requestingNode)
        {
            throw new NotImplementedException();
            //TODO Node zu KnownNodes hinzufügen
            //TODO Blockchain über's Netz verschicken (synchron)
            //TODO Neuen Node an alle KnownNodes verbreiten
        }

        public void OnReceiveNewNode(Uri senderId, Uri newNode)
        {
            throw new NotImplementedException();
            //TODO Überprüfen, ob Sender ein KnownNode ist
            //TODO Node zu KnownNodes hinzufügen wenn ja
        }

        public void OnReveiceTransaction(ITransaction transaction)
        {
            if (transaction.ValidateContextual(participantHandler, new List<Chain>() { chain }))
            {
                transactionBuffer.RecordTransaction(transaction);
            }
        }

        public void OnReceiveChain(Chain chain)
        {
            throw new NotImplementedException();
            //TODO Publish Blocks stoppen
            //TODO Chain validieren (Stimmen Publisher, Physicians, Patients etc.)
            //TODO Chain mit aktuellem Stand der Blockchain abgleichen => Collisions resolven (Nicht einfügen wenn aktueller Stand neuer ist als die empfangene Chain)
            //TODO Wenn eingefügt wird: UnpublishedBlocks auflösen (Transaktionen zurück in OpenTransactions stellen)
            //TODO Wenn Chain übernommen wird => Blocks die eventuell aus aktueller Chain rausfliegen, werden von Chain zurückgegeben
            //TODO Wenn Chain eingefügt wird: Transaktionen durchgehen, und alle offenen Transaktionen löschen, die in neuer Chain enthalten sind
            //TODO Wenn Teile der alten Chain wegfliegen: Transaktionen durchgehen und die in offene Transaktionen mit aufnehmen, die nicht in neuer Chain enthalten sind
        }

        public void PublishTransaction(ITransaction transaction)
        {
            throw new NotImplementedException();
            //TODO Transaction an alle KnownNodes schicken
            //TODO Nodes flaggen, von denen keine Antwort kommt (Sind nicht mehr erreichbar)
        }

        public void PublishChain(Chain chain)
        {
            throw new NotImplementedException();
            //TODO Chain an alle KnownNodes schicken
        }

        public void RequestChainInitialization(ITransaction initialPublisher, ITransaction initialVote)
        {
            if (chain.IsEmpty())
            {
                transactionBuffer.RecordTransaction(initialPublisher);
                transactionBuffer.RecordTransaction(initialVote);
            }
        }

        public void StartPublishing()
        {
            blockGenerator = new Timer(transactionBuffer.BundleTransactions, null, 5000, 5000);
            blockProcessor = new Timer(PublishOpenBlocks, null, 7000, 5000);
        }

        public void PublishOpenBlocks(object state)
        {
            //TODO Überprüfen, ob Node überhaupt publishen darf (Registrierung eines neuen Publishers muss durchgehen)

            if (transactionBuffer.HasBlocks())
            {
                Chain appendix = new Chain();

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
                chain.Add(appendix);
            }

            //TODO Appendix an alle KnownNodes verschicken
            //TODO Nodes flaggen, von denen keine Antwort kommt
        }

        public List<Treatment> GetTreatments_FullList()
        {
            throw new NotImplementedException();
            //TODO Komplette Blockchain durchgehen, und daraus Liste an (zum jeweiligen Zeitpunkt gültigen) Treatments erstellen
            //TODO Währenddessen müssen Publishers, Physicians etc. berücksichtigt werden
        }

        public List<Treatment> GetTreatments_For_Timespan(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
            //TODO Wie FullList, nur bei Transactions auf Zeitstempel achten
        }

        public List<Treatment> GetTreatments_For_Area(string country, string region)
        {
            throw new NotImplementedException();
            //TODO Wie FullList, nur bei Treatments auf Region des Patienten achten
        }
    }
}
