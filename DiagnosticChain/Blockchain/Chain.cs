using Shared;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain
{
    public class Chain
    {
        public Block Blockhead { get; set; }

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

        public bool Validate(ParticipantHandler participantHandler)
        {
            var blockIsValid = true;
            var currentBlock = Blockhead;

            while (currentBlock != null && blockIsValid)
            {
                if (participantHandler.HasPublisher(currentBlock.Publisher))
                {
                    blockIsValid &= currentBlock.Validate(participantHandler.GetPublisherKey(currentBlock.Publisher));
                    foreach (var t in currentBlock.Transactions)
                    {
                        if (participantHandler.HasSender(t.SenderAddress))
                        {
                            blockIsValid &= t.Validate(participantHandler.GetSenderKey(t.SenderAddress));
                            blockIsValid &= participantHandler.HandleTransaction(t);
                        } else
                        {
                            blockIsValid = false;
                        }
                    }
                    currentBlock = currentBlock.PreviousBlock;
                } else if (currentBlock.Index == 0) //Initializing block does not need to be validated
                {
                    blockIsValid &= true;
                } else
                {
                    blockIsValid = false;
                }
            }

            return blockIsValid;
        }

        public bool Add(Block block)
        {
            if (block.PreviousHash == Blockhead.Hash && block.Index == Blockhead.Index + 1)
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
            //Get first block in chain
            var firstBlock = chain.Blockhead;
            while (firstBlock != null) firstBlock = firstBlock.PreviousBlock;

            //Add chain directly if hashes and indexes match to Blockhead
            if (firstBlock.PreviousHash == Blockhead.Hash && firstBlock.Index == Blockhead.Index + 1)
            {
                firstBlock.PreviousBlock = Blockhead;
                firstBlock.CalculateHash();
                Blockhead = chain.Blockhead;

                return true;
            } else
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
    }
}
