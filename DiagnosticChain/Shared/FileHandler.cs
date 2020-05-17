using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared
{
    public class FileHandler
    {
        public static readonly string ChainPath = "Blockchain.xml";
        public static readonly string LogPath = "Log.txt";
        public static readonly string UsersPath = "Users.txt";

        public static readonly string UserState_NodePath = "UserState_Node.xml";
        public static readonly string UserState_ServerPath = "UserState_Server.xml";

        public static void Save(string path, string s)
        {
            File.WriteAllText(path, s);
        }

        public static void Append(string path, string s)
        {
            File.AppendAllText(path, s);
        }

        public static void Log(string s)
        {
            File.AppendAllText(LogPath, s + "\n");
        }

        public static string Read(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                return null;
            }
        }
    }
}
