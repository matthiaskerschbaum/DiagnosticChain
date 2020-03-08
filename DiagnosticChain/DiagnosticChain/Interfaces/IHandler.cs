using System;
using System.Collections.Generic;
using System.Text;

namespace Handler.Interfaces
{
    interface IHandler
    {
        void StartUp(Action onShutDown);
        void ShutDown();
    }
}
