using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Blockchain.Interfaces;
using Blockchain.Utilities;
using DiagnosticChain.Entities;
using Grpc.Core;
using NetworkingFacilities.Clients;
using NodeManagement;
using NodeManagement.Entities;
using Shared;

namespace DiagnosticChain.Controllers
{
    class PhysicianController
    {
        private string currentUser = "";

        private PhysicianNode node;
        private TransactionGenerator transactionGenerator;

        public List<Entities.Patient> patients = new List<Entities.Patient>();

        public PhysicianController()
        {
            node = new PhysicianNode();
            transactionGenerator = new TransactionGenerator();
        }

        public PhysicianController(string currentUser) : this()
        {
            this.currentUser = currentUser;
        }

        internal void AddDiagnosesForTreatment(Guid treatmentAddress, string diagnoses)
        {
            if (patients.Where(x => x.Treatments.Where(t => t.TreatmentAddress == treatmentAddress).Any()).Any())
            {
                var diagnosesTransaction = transactionGenerator.GenerateDiagnosesTransaction(treatmentAddress, new List<string>() { diagnoses });

                if (node.SendTransaction(diagnosesTransaction))
                {
                    var treatment = from p in patients
                                    where p.Treatments.Where(t => t.TreatmentAddress == treatmentAddress).Any()
                                    select p.Treatments.Where(t => t.TreatmentAddress == treatmentAddress).First();

                    treatment.First().Diagnoses.Add(diagnoses);
                }
            }
        }

        internal void AddNewPatient(string name, string country, string region, string birthyear)
        {
            var patientRegistration = transactionGenerator.GeneratePatientRegistrationTransaction(country, region, birthyear);

            if (node.SendTransaction(patientRegistration))
            {
                patients.Add(new Entities.Patient()
                {
                    PatientAddress = patientRegistration.TransactionId
                    ,
                    Name = name
                    ,
                    Country = country
                    ,
                    Region = region
                    ,
                    Birthyear = birthyear
                    ,
                    Treatments = new List<Entities.Treatment>()
                });
            }
        }

        internal void AddTreatmentForPatient(Guid patientAddress)
        {
            if (patients.Where(x => x.PatientAddress == patientAddress).Any())
            {
                var treatment = transactionGenerator.GenerateTreatmentTransaction(node.User.UserAddress, patientAddress);

                if (node.SendTransaction(treatment))
                {
                    patients.Where(x => x.PatientAddress == patientAddress).First()
                        .Treatments.Add(new Entities.Treatment() { TreatmentAddress = treatment.TransactionId, Created = DateTime.Now });
                }
            }
        }

        internal void AddSymptomForTreatment(Guid treatmentAddress, string symptom)
        {
            if (patients.Where(x => x.Treatments.Where(t => t.TreatmentAddress == treatmentAddress).Any()).Any())
            {
                var symptomTransaction = transactionGenerator.GenerateSymptomTransaction(treatmentAddress, new List<string>() { symptom });

                if (node.SendTransaction(symptomTransaction))
                {
                    var treatment = from p in patients
                                    where p.Treatments.Where(t => t.TreatmentAddress == treatmentAddress).Any()
                                    select p.Treatments.Where(t => t.TreatmentAddress == treatmentAddress).First();

                    treatment.First().Symptoms.Add(symptom);
                }
            }
        }

        internal List<Blockchain.Entities.Physician> GetPendingPhysicians()
        {
            return node.GetPendingPhysicians();
        }

        internal List<Entities.Patient> GetRegisteredPatients()
        {
            return patients;
        }

        internal bool HasSavedState()
        {
            return File.Exists(currentUser + FileHandler.UserState_NodePath)
                && File.Exists(currentUser + FileHandler.UserState_PatientPath);
        }

        private void LoadState()
        {
            if (HasSavedState())
            {
                //Load Node state
                var nodeState = FileHandler.Read(currentUser + FileHandler.UserState_NodePath);
                XmlSerializer serializer = new XmlSerializer(node.GetType());
                node = (PhysicianNode)serializer.Deserialize(new StringReader(nodeState));
                currentUser = node.User.Username;
                transactionGenerator = new TransactionGenerator(node.User.UserAddress, node.User.Keys.PrivateKey);

                //Load Patient file
                var patientState = FileHandler.Read(currentUser + FileHandler.UserState_PatientPath);
                serializer = new XmlSerializer(patients.GetType());
                patients = (List<Entities.Patient>)serializer.Deserialize(new StringReader(patientState));
            }
        }

        private void SaveState()
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

            //Saving patient state
            xsSubmit = new XmlSerializer(patients.GetType());
            xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, patients);
                    xml = sww.ToString();
                }
            }

            FileHandler.Save(currentUser + FileHandler.UserState_PatientPath, xml);
        }

        internal void ShutDown()
        {
            SaveState();
        }

        internal void Start()
        {
            LoadState();
            UpdateKnownNodes();
            UpdatePendingPhysicians();
        }

        internal void StartAsNewPhysician(string country, string region, string fullname, ServerAddress initializerAddress)
        {
            var keys = EncryptionHandler.GenerateNewKeys();
            transactionGenerator = new TransactionGenerator(keys.PrivateKey);
            ITransaction registration = transactionGenerator.InitializeAsNewPhysician(keys.PublicKey, country, region, fullname);

            node.User = new UserProperties()
            {
                Username = currentUser
                ,
                Keys = keys
                ,
                UserAddress = registration.TransactionId
            };

            Start();

            node.AddServerAddress(initializerAddress);
            UpdateKnownNodes();
            UpdatePendingPhysicians();

            node.SendTransaction(registration);
        }

        private void UpdateKnownNodes()
        {
            var nodes2Add = new List<ServerAddress>();

            foreach (var n in node.knownNodes)
            {
                try
                {
                    var nodeList = new PhysicianClient(n).RequestNodes();

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

        internal void UpdatePendingPhysicians()
        {
            node.UpdatePendingPhysicians();
        }

        internal void VoteFor(Guid address, bool vote)
        {
            var votingTransaction = transactionGenerator.GenerateVotingTransaction(address, vote);
            node.SendTransaction(votingTransaction);
        }
    }
}
