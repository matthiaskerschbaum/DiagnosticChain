using Blockchain;
using Blockchain.Transactions;
using DiagnosticChain.Entities;
using Grpc.Core;
using NetworkingFacilities.Clients;
using NodeManagement;
using NodeManagement.Entities;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DiagnosticChain.Controllers
{
    class QueryController
    {
        private string currentUser = "";

        private QueryNode node;

        public QueryController()
        {
            node = new QueryNode();
        }

        public QueryController(string currentUser) : this()
        {
            this.currentUser = currentUser;
            node.User = new UserProperties()
            {
                Username = currentUser
            };
        }

        internal void AddNode(ServerAddress address)
        {
            node.AddServerAddress(address);
        }

        internal void ExtractTreatmentDataByCountry(string filepath, List<string> countries)
        {
            var data = node.ExtractData((PatientRegistrationTransaction p) => countries.Contains(p.Country),
                                        (TreatmentTransaction t) => true,
                                        (SymptomsTransaction s) => true,
                                        (DiagnosesTransaction d) => true);

            foreach (var d in data)
            {
                FileHandler.Append(filepath, d.ForExport("\t") + "\n");
            }
        }

        internal void ExtractTreatmentDataFull(string filepath)
        {
            var data = node.ExtractData((PatientRegistrationTransaction p) => true,
                                        (TreatmentTransaction t) => true,
                                        (SymptomsTransaction s) => true,
                                        (DiagnosesTransaction d) => true);

            foreach (var d in data)
            {
                FileHandler.Append(filepath, d.ForExport("\t") + "\n");
            }
        }

        private bool HasSavedState()
        {
            return File.Exists(currentUser + FileHandler.UserState_NodePath);
        }

        private void LoadState()
        {
            if (HasSavedState())
            {
                //Load Node state
                var nodeState = FileHandler.Read(currentUser + FileHandler.UserState_NodePath);
                XmlSerializer serializer = new XmlSerializer(node.GetType());
                node = (QueryNode)serializer.Deserialize(new StringReader(nodeState));
                currentUser = node.User.Username;
            }
        }

        internal bool HasBlockchainConnection()
        {
            return node.HasBlockchainConnection();
        }

        internal void Start()
        {
            LoadState();
            node.LoadChain();
            UpdateKnownNodes();
        }

        internal void SaveState()
        {
            //Saving node state
            XmlSerializer xsSubmit = new XmlSerializer(node.GetType());
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, node);
                    xml = sww.ToString();
                }
            }

            FileHandler.Save(currentUser + FileHandler.UserState_NodePath, xml);
        }

        internal void ShutDown()
        {
            SaveState();
            node.SaveChain();
        }

        internal void UpdateChain()
        {
            node.UpdateChain();
        }

        private void UpdateKnownNodes()
        {
            var nodes2Add = new List<ServerAddress>();

            foreach (var n in node.knownNodes)
            {
                try
                {
                    var nodeList = new QueryClient(n).RequestNodes();

                    foreach (var newN in nodeList)
                    {
                        if (!node.knownNodes.Contains(newN) && !nodes2Add.Contains(newN))
                        {
                            nodes2Add.Add(newN);
                        }
                    }
                }
                catch (RpcException) { }
            }

            foreach (var n in nodes2Add)
            {
                node.AddServerAddress(n);
            }
        }
    }
}
