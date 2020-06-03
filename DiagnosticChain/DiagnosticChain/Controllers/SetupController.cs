using DiagnosticChain.Interfaces;
using DiagnosticChain.UserInterface;
using Shared;
using System.Collections.Generic;
using System.IO;


namespace DiagnosticChain.Controllers
{
    class SetupController
    {
        private Dictionary<string, string> users = new Dictionary<string, string>();
        private static Dictionary<string, IUserInterface> nodeTypes = new Dictionary<string, IUserInterface> {
            { "publisher", new PublisherInterface() }
            ,{ "physician", new PhysicianInterface() }
            ,{ "query", new QueryInterface() }
        };

        public SetupController()
        {
            if (File.Exists(FileHandler.UsersPath))
            {
                var usersRaw = FileHandler.Read(FileHandler.UsersPath);

                foreach (var line in usersRaw.Split('\n'))
                {
                    var lineParts = line.Split('\t');
                    if (lineParts.Length == 2)
                    {
                        users.Add(lineParts[0], lineParts[1]);
                    }
                }
            }
        }

        public void AddUser(string username, string nodetype)
        {
            if (!HasUser(username))
            {
                FileHandler.Append(FileHandler.UsersPath, username + "\t" + nodetype + "\n");
                users.Add(username, nodetype);
            }
        }

        public bool HasUser(string username)
        {
            return users.ContainsKey(username);
        }

        public IUserInterface SetupForUser(string username)
        {
            var nodetype = users[username];

            if (!nodeTypes.ContainsKey(nodetype))
            {
                return new ErrorInterface("Nodetype " + nodetype + " is specified for user " + username + ". This nodetype is not currently available.");
            }

            var ui = nodeTypes[nodetype];
            ui.PrepareForUser(username);
            return ui;
        }

        public List<string> GetNodeTypes()
        {
            List<string> ret = new List<string>();

            foreach (var n in nodeTypes.Keys)
            {
                ret.Add(n);
            }

            return ret;
        }
    }
}
