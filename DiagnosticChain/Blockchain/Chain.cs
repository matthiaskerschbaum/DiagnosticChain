using Blockchain.Interfaces;
using Blockchain.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Blockchain
{
    public class Chain
    {
        public Block Blockhead { get; set; }

        public Chain()
        {

        }

        public Chain(string xml)
        {
            if (xml != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var nodes = doc.SelectNodes("Chain/Block");

                var blockList = new List<Block>();

                foreach (XmlNode node in nodes)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Block), new Type[] { typeof(ITransaction) });
                    Block block = (Block)serializer.Deserialize(new StringReader(node.OuterXml));
                    blockList.Add(block);
                }

                blockList.Sort((x, y) => x.Index.CompareTo(y.Index));

                foreach (var b in blockList)
                {
                    Add(b);
                }
            }
        }

        public bool Add(Block block)
        {
            if (Blockhead == null || (block.PreviousHash == Blockhead.Hash && block.Index == Blockhead.Index + 1))
            {
                block.PreviousBlock = Blockhead;
                block.ValidateSequence();
                Blockhead = block;

                return true;
            }

            return false;
        }

        //TODO Überprüfung einfügen, ob Chain eingefügt werden soll oder nicht (Collision Management)
        //TODO Rückgabewert auf Chain ändern, und alle Blocks zurück geben, die gelöscht werden
        public bool Add(Chain chain)
        {
            //If chain is currently empty, set Blockhead to inserted chain's Blockhead and return true
            if (Blockhead == null)
            {
                Blockhead = chain.Blockhead;
                return true;
            }

            //Get first block in chain
            var firstBlock = chain.Blockhead;
            while (firstBlock.PreviousBlock != null) firstBlock = firstBlock.PreviousBlock;

            //Add chain directly if hashes and indexes match to Blockhead
            if (Blockhead == null || (firstBlock.PreviousHash == Blockhead.Hash && firstBlock.Index == Blockhead.Index + 1))
            {
                firstBlock.PreviousBlock = Blockhead;
                Blockhead = chain.Blockhead;

                return true;
            }
            else
            {
                //Search matching block if the chain replaces previous blocks
                var referenceBlock = Blockhead;

                while (referenceBlock.PreviousBlock != null
                    && firstBlock.PreviousHash != referenceBlock.PreviousBlock.Hash
                    && firstBlock.Index >= referenceBlock.Index)
                {
                    referenceBlock = referenceBlock.PreviousBlock;
                }

                //Attach chain at correct position if possible
                if (referenceBlock != null &&
                    firstBlock.PreviousHash == referenceBlock.PreviousBlock.Hash
                    && firstBlock.Index == referenceBlock.Index)
                {
                    firstBlock.PreviousBlock = referenceBlock.PreviousBlock;
                    firstBlock.ValidateSequence();
                    Blockhead = chain.Blockhead;

                    return true;
                }
            }

            //If no matching position is found in the blockchain, return false
            return false;
        }

        public string AsXML()
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(Block), new Type[] { typeof(ITransaction) });
            List<Block> blockList;
            if (Blockhead != null)
            {
                blockList = GetBlocks();
            }
            else
            {
                blockList = new List<Block>();
            }

            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Chain");

                    foreach (var b in blockList)
                    {
                        xsSubmit.Serialize(writer, b);
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                    writer.Close();

                    xml = sww.ToString();
                }
            }

            return xml;
        }

        public List<Block> GetBlocks()
        {
            List<Block> ret = new List<Block>();
            var currentBlock = Blockhead;

            while (currentBlock != null)
            {
                ret.Add(currentBlock);
                currentBlock = currentBlock.PreviousBlock;
            }

            ret.Sort((x, y) => x.Index.CompareTo(y.Index));
            return ret;
        }

        public List<ITransaction> GetTransactions()
        {
            List<ITransaction> ret = new List<ITransaction>();
            var currentBlock = Blockhead;

            while (currentBlock != null)
            {
                ret.AddRange(currentBlock.TransactionList);
                currentBlock = currentBlock.PreviousBlock;
            }

            ret.Sort((x, y) => x.Timestamp.CompareTo(y.Timestamp));
            return ret;
        }

        internal bool HasTransaction(Guid address)
        {
            var ret = false;
            var currentBlock = Blockhead;

            while (currentBlock != null)
            {
                ret |= currentBlock.HasTransaction(address);

                currentBlock = currentBlock.PreviousBlock;
            }

            return ret;
        }

        public bool IsEmpty()
        {
            return Blockhead == null;
        }

        public bool ProcessContracts(ParticipantHandler participantHandler, List<Chain> context)
        {
            var ret = true;
            var blockList = GetBlocks();

            foreach (var b in blockList)
            {
                ret &= b.ProcessContracts(participantHandler, context);
            }

            return ret;
        }

        public bool ValidateContextual(ParticipantHandler participantHandler, List<Chain> context) //context = Hauptchain, falls gerade eine neu Empfangen Teilchain verarbeitet wird
        {
            context = (context == null) ? new List<Chain>() : context;
            context.Add(this);

            var ret = true;
            var blockList = GetBlocks();

            foreach (var b in blockList)
            {
                ret &= b.ValidateContextual(participantHandler, context);
            }

            return ret;
        }

        public bool ValidateSequence()
        {
            var result = true;
            var currentBlock = Blockhead;

            while (currentBlock != null)
            {
                result &= currentBlock.ValidateSequence();
                currentBlock = currentBlock.PreviousBlock;
            }

            return result;
        }
    }
}
