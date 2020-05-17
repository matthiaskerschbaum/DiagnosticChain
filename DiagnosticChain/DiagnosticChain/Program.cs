using DiagnosticChain.UserInterface;

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
