using DiagnosticChain.Controllers;
using DiagnosticChain.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticChain.UserInterface
{
    class PhysicianInterface : IUserInterface
    {
        PhysicianController controller;
        private Dictionary<string, Action> userCommands;

        private static readonly string promptNextCommand = "What do you want me to do now?";

        public PhysicianInterface()
        {
            controller = new PhysicianController();

            userCommands = new Dictionary<string, Action>()
            {
                { "add diagnoses", AddDiagnoses }
                ,{ "add patient", AddPatient }
                , { "add symptom", AddSymptom }
                ,{ "add treatment", AddTreatment }
                ,{ "list patients", ListPatients }
                ,{ "list physicians pending", ListPendingPhysicians }
                ,{ "list treatments", ListTreatments }
                ,{ "vote physician", VotePhysician }
            };
        }

        private void AddDiagnoses()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Adding a new diagnoses");
            CLI.DisplayLineDelimiter();

            var patients = controller.GetRegisteredPatients().ToArray();

            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Please select a patient for which to add a symptom");
            CLI.DisplayLineDelimiter();

            if (patients.Length == 0)
            {
                CLI.DisplayLine("No patients found");
            }

            for (int i = 0; i < patients.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + patients[i].Name + ", " + patients[i].Birthyear + ", "
                    + patients[i].Country + ", " + patients[i].Region + "\t" + patients[i].Treatments.Count + " treatments");
            }

            CLI.DisplayLineDelimiter();
            var input = CLI.PromptUser("Please enter the index of the patient you are adding a symptom for, or enter Q to quit: ");

            CLI.DisplayLineDelimiter();

            if (input == "Q") return;
            int index;
            if (int.TryParse(input, out index) && index < patients.Length)
            {
                var p = patients[index];
                CLI.DisplayLineDelimiter();
                CLI.DisplayLine("Please select a treatment for which to add a symptom");
                CLI.DisplayLineDelimiter();

                if (p.Treatments.Count == 0)
                {
                    CLI.DisplayLine("No treatments found");
                    CLI.DisplayLineDelimiter();
                    return;
                }

                CLI.DisplayLineDelimiter();

                var ts = p.Treatments.ToArray();
                for (int i = 0; i < ts.Length; i++)
                {
                    CLI.DisplayLine(i + "\t" + ts[i].Created.ToString("yyyy-MM-dd hh:mm:ss"));
                }

                input = CLI.PromptUser("Please enter the index of the treatment you are adding a diagnoses for, or enter Q to quit: ");
                CLI.DisplayLineDelimiter();
                if (input == "Q") return;
                if (int.TryParse(input, out index) && index < ts.Length)
                {
                    var t = ts[index];

                    var diagnoses = CLI.PromptUser("Enter diagnoses to add:");
                    controller.AddDiagnosesForTreatment(t.TreatmentAddress, diagnoses);
                }
                else
                {
                    CLI.DisplayLine("Treatment not found");
                }
            }
            else
            {
                CLI.DisplayLine("Patient not found");
            }
        }

        private void AddPatient()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Adding a new patient");
            CLI.DisplayLineDelimiter();

            CLI.DisplayLine("Please provide the following data: ");
            var name = CLI.PromptUser("Name (or identifier) of the patient: ");
            var country = CLI.PromptUser("Patient's country of residence: ");
            var region = CLI.PromptUser("Region of patient's residence: ");
            var birthyear = CLI.PromptUser("Year of the patient's birth: ");

            controller.AddNewPatient(name, country, region, birthyear);
            CLI.DisplayLine("Patient added");
            CLI.DisplayLineDelimiter();
        }

        private void AddSymptom()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Adding a new symptom");
            CLI.DisplayLineDelimiter();

            var patients = controller.GetRegisteredPatients().ToArray();

            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Please select a patient for which to add a symptom");
            CLI.DisplayLineDelimiter();

            if (patients.Length == 0)
            {
                CLI.DisplayLine("No patients found");
            }

            for (int i = 0; i < patients.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + patients[i].Name + ", " + patients[i].Birthyear + ", "
                    + patients[i].Country + ", " + patients[i].Region + "\t" + patients[i].Treatments.Count + " treatments");
            }

            CLI.DisplayLineDelimiter();
            var input = CLI.PromptUser("Please enter the index of the patient you are adding a symptom for, or enter Q to quit: ");

            if (input == "Q") return;
            int index;
            if (int.TryParse(input, out index) && index < patients.Length)
            {
                var p = patients[index];
                CLI.DisplayLineDelimiter();
                CLI.DisplayLine("Please select a treatment for which to add a symptom");
                CLI.DisplayLineDelimiter();

                if (p.Treatments.Count == 0)
                {
                    CLI.DisplayLine("No treatments found");
                    return;
                }

                CLI.DisplayLineDelimiter();

                var ts = p.Treatments.ToArray();
                for (int i = 0; i < ts.Length; i++)
                {
                    CLI.DisplayLine(i + "\t" + ts[i].Created.ToString("yyyy-MM-dd hh:mm:ss"));
                }

                input = CLI.PromptUser("Please enter the index of the treatment you are adding a symptom for, or enter Q to quit: ");
                if (input == "Q") return;
                if (int.TryParse(input, out index) && index < ts.Length)
                {
                    var t = ts[index];

                    var symptom = CLI.PromptUser("Enter symptom to add:");
                    controller.AddSymptomForTreatment(t.TreatmentAddress, symptom);
                } else
                {
                    CLI.DisplayLine("Treatment not found");
                }
            }
            else
            {
                CLI.DisplayLine("Patient not found");
            }
        }

        private void AddTreatment()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Adding a new treatment to a patient");
            CLI.DisplayLineDelimiter();

            var patients = controller.GetRegisteredPatients().ToArray();

            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Please select a patient for which to add a treatment");
            CLI.DisplayLineDelimiter();

            if (patients.Length == 0)
            {
                CLI.DisplayLine("No patients found");
            }

            for (int i = 0; i < patients.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + patients[i].Name + ", " + patients[i].Birthyear + ", "
                    + patients[i].Country + ", " + patients[i].Region + "\t" + patients[i].Treatments.Count + " treatments");
            }

            CLI.DisplayLineDelimiter();
            var input = CLI.PromptUser("Please enter the index of the patient you are adding a treatment for, or enter Q to quit: ");

            if (input == "Q") return;
            int index;
            if (int.TryParse(input, out index) && index < patients.Length)
            {
                controller.AddTreatmentForPatient(patients[index].PatientAddress);
            }
            else
            {
                CLI.DisplayLine("Patient not found");
            }

            CLI.DisplayLineDelimiter();
        }

        public void Interact(Action onCompletion)
        {
            try
            {
                CLI.DisplayLine("Welcome to the physician interface!");
                CLI.DisplayLineDelimiter();

                if (!controller.HasSavedState()) StartAsNewPhysician();
                else controller.Start();

                CLI.DisplayLine("PhysicianHandler ready. Enter " + UIConstants.abortionCode + " to quit.");
                CLI.DisplayLineDelimiter();

                var userInput = CLI.PromptUser(promptNextCommand);

                while (userInput != UIConstants.abortionCode)
                {
                    if (userCommands.ContainsKey(userInput))
                    {
                        userCommands[userInput]();
                    }
                    else
                    {
                        CLI.DisplayLine("Command not found. The following options are available:\n");
                        foreach (string key in userCommands.Keys)
                        {
                            CLI.DisplayLine(key);
                        }

                        CLI.DisplayLineDelimiter();
                    }

                    userInput = CLI.PromptUser(promptNextCommand);
                }
            } finally
            {
                controller.ShutDown();
                onCompletion();
            }
        }

        private void ListPatients()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Listing your patients");
            CLI.DisplayLineDelimiter();

            var patients = controller.GetRegisteredPatients().ToArray();

            if (patients.Length == 0)
            {
                CLI.DisplayLine("No patients found");
            }

            for (int i = 0; i < patients.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + patients[i].Name + ", " + patients[i].Birthyear + ", "
                    + patients[i].Country + ", " + patients[i].Region + "\t" + patients[i].Treatments.Count + " treatments");
            }

            CLI.DisplayLineDelimiter();
        }

        private void ListPendingPhysicians()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Listing pending physicians");
            CLI.DisplayLineDelimiter();

            CLI.DisplayLine("Updating physicians...");
            controller.UpdatePendingPhysicians();
            CLI.DisplayLine("Physicians updated");
            CLI.DisplayLineDelimiter();

            var physicians = controller.GetPendingPhysicians().ToArray();

            if (physicians.Length == 0)
            {
                CLI.DisplayLine("No physicians found");
            }

            for (int i = 0; i < physicians.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + physicians[i].PhysicianIdentifier + ", " + physicians[i].Country + ", " + physicians[i].Region + "\t");
            }

            CLI.DisplayLineDelimiter();
        }

        private void ListTreatments()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Listing patient treatments");
            CLI.DisplayLineDelimiter();

            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Please select a patient for which to list treatments");
            CLI.DisplayLineDelimiter();

            var patients = controller.GetRegisteredPatients().ToArray();

            if (patients.Length == 0)
            {
                CLI.DisplayLine("No patients found");
            }

            for (int i = 0; i < patients.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + patients[i].Name + ", " + patients[i].Birthyear + ", "
                    + patients[i].Country + ", " + patients[i].Region + "\t" + patients[i].Treatments.Count + " treatments");
            }

            CLI.DisplayLineDelimiter();
            var input = CLI.PromptUser("Please enter the index of the patient you want to list treatments for, or enter Q to quit: ");

            if (input == "Q") return;
            int index;
            if (int.TryParse(input, out index) && index < patients.Length)
            {
                var p = patients[index];
                p.Treatments.Sort((x,y) => x.Created.CompareTo(y.Created));

                foreach (var t in p.Treatments)
                {
                    CLI.DisplayLine(t.Created.ToString("yyyy-MM-dd hh:mm:ss"));
                    CLI.DisplayLine("\t" + t.Symptoms.Count + " Symptoms");
                    foreach (var s in t.Symptoms)
                    {
                        CLI.DisplayLine("\t\t" + s);
                    }
                    CLI.DisplayLine("\t" + t.Diagnoses.Count + " Diagnoses");
                    foreach (var d in t.Diagnoses)
                    {
                        CLI.DisplayLine("\t\t" + d);
                    }
                }
            }
            else
            {
                CLI.DisplayLine("Patient not found");
            }
        }

        public void PrepareForUser(string username)
        {
            controller = new PhysicianController(username);
        }

        private void StartAsNewPhysician()
        {
            CLI.DisplayLine("You are not a registered physician yet. Please provide the following data:");
            var country = CLI.PromptUser("Your country:");
            var region = CLI.PromptUser("Your region:");
            var fullname = CLI.PromptUser("Your full name:");

            CLI.DisplayLine("In order to participate in the DiagnosticChain, you will need to register at an existing publishing node. Please provide the following data:");
            var connectorIp = CLI.PromptUser("Please provide the IP address of an existing node to connect to:");
            var connectorPort = CLI.PromptUser("Please provide the port to connect to at the destination:");

            var initializerAddress = new ServerAddress()
            {
                Ip = connectorIp
                    ,
                Port = Int32.Parse(connectorPort)
            };

            controller.StartAsNewPhysician(country, region, fullname, initializerAddress);
            CLI.DisplayLine("Physician initialized");
        }

        private void VotePhysician()
        {
            CLI.DisplayLineDelimiter();
            CLI.DisplayLine("Vote for proposed physician");
            CLI.DisplayLineDelimiter();

            CLI.DisplayLine("Updating physicians...");
            controller.UpdatePendingPhysicians();
            CLI.DisplayLine("Physicians updated");
            CLI.DisplayLineDelimiter();

            var proposedPhysicians = controller.GetPendingPhysicians().ToArray();

            CLI.DisplayLine("The following physicians are available:\n");
            for (int i = 0; i < proposedPhysicians.Length; i++)
            {
                CLI.DisplayLine(i + "\t" + proposedPhysicians[i].PhysicianIdentifier + ", " + proposedPhysicians[i].Country + ", " + proposedPhysicians[i].Region);
            }
            CLI.DisplayLineDelimiter();

            var input = CLI.PromptUser("Please enter the index of the publisher you are voting for, or enter Q to quit: ");

            if (input == "Q") return;
            int index;
            if (int.TryParse(input, out index) && index < proposedPhysicians.Length)
            {
                var vote = CLI.PromptUser("Please enter y for confirmed, or n for dismiss");
                controller.VoteFor(proposedPhysicians[index].Address, vote == "y");
            }
            else
            {
                CLI.DisplayLine("Physician not found");
            }

            CLI.DisplayLineDelimiter();
        }
    }
}
