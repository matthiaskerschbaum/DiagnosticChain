using Blockchain.Interfaces;
using Blockchain.Transactions;
using DiagnosticChain.UserInterface;
using Shared;
using System;
using System.IO;
using System.Xml.Serialization;

namespace DiagnosticChain
{
    class Program
    {
        static void Main(string[] args)
        {
            new SetupInterface().Interact(OnShutDown);
        }

        private static void OnShutDown()
        {

        }
    }
}
