using Blockchain.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain
{
    public class ParticipantHandler
    {
        //TODO Ausprogrammieren
        //Der speichert eine Liste aller bekannter Publisher, Physicians und Patienten und kann Transaktionen handeln und validieren
        internal bool HasPublisher(Guid publisher)
        {
            //TODO darf nur die bereits bestätigten Publisher zurück geben
            throw new NotImplementedException();
        }

        //Geht Publisher und Physicians durch, und schaut ob die existieren
        internal bool HasSender(Guid senderAddress)
        {
            throw new NotImplementedException();
        }

        internal RSAParameters GetPublisherKey(Guid publisher)
        {
            throw new NotImplementedException();
        }

        internal RSAParameters GetSenderKey(Guid senderAddress)
        {
            throw new NotImplementedException();
        }

        //Verarbeitet je nach Transaktionstyp (Publisher werden hinzugefügt, Votes werden ausgewertet etc.)
        internal bool HandleTransaction(ITransaction t)
        {
            throw new NotImplementedException();
        }
    }
}
