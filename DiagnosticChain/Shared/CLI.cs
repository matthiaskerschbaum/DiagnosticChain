using System;

namespace Shared
{
    public static class CLI
    {
        public static void DisplayLine(string line)
        {
            Console.WriteLine(line);
        }

        public static string PromptUser(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }

        public static void DisplayLineDelimiter()
        {
            Console.WriteLine("----------------------------------");
        }
    }
}
