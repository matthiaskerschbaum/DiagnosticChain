using System;

namespace Shared
{
    public static class CLI
    {
        public static void DisplayLine(string line)
        {
            Console.WriteLine(line);
        }

        public static string InlinePrompt()
        {
            return Console.ReadLine();
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

        public static void ClearCurrentLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
