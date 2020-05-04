using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticChain.Interfaces
{
    interface IUserInterface
    {
        public void Interact(Action onCompletion);
        public void PrepareForUser(string username);
    }
}
