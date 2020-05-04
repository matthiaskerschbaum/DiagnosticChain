using DiagnosticChain.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticChain.UserInterface
{
    class ErrorInterface : IUserInterface
    {
        private string message = "";

        public ErrorInterface()
        {

        }

        public ErrorInterface(string message)
        {
            this.message = message;
        }

        public void Interact(Action onCompletion)
        {
            CLI.DisplayLine(message == "" ? "A fatal error occured" : message);
            CLI.DisplayLine("DiagnosticChain is shutting down. Bye!");
        }

        public void PrepareForUser(string username)
        {
            throw new NotImplementedException();
        }
    }
}
