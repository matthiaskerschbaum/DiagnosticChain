using Handler.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Handler.Handlers
{
    public class QueryingHandler : IHandler
    {
        //Delegates
        private Action onShutDown;

        public void StartUp(Action onShutDown)
        {
            CLI.DisplayLine("Starting QueryingHandler...");

            //Initialize Handler
            this.onShutDown = onShutDown;

            //TODO Set up node (store all blockchain data) => Read from disk if present, prompt for URL else
            //TODO Start listening to user input

            CLI.DisplayLine("QueryingHandler started");

            //TODO Replace with actual logic
            ShutDown();
        }

        public void ShutDown()
        {
            CLI.DisplayLine("Shutting down QueryingHandler...");

            //TODO stop listening to user input
            //TODO Save blockchain to file

            CLI.DisplayLine("QueryingHandler shut down");

            //Invoke callback
            onShutDown();
        }

        public void HandleUserInput()
        {
            throw new NotImplementedException();
            //TODO Read in and handle user input
        }

        public void QueryFull(string outputPath)
        {
            throw new NotImplementedException();
            //TODO Get Full Blockchain data and save it to a specified file
        }

        public void QueryTimespan(DateTime from, DateTime to, string outputPath)
        {
            throw new NotImplementedException();
            //TODO Get Blockchain data for timespan and save it to a specified file
        }

        public void QueryArea(string country, string region, string outputPath)
        {
            throw new NotImplementedException();
            //TODO Get Blockchain data for area and save it to a specified file
        }
    }
}
