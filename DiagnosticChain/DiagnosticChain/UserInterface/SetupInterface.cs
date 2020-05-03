using DiagnosticChain.Controllers;
using DiagnosticChain.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticChain.UserInterface
{
    class SetupInterface : IUserInterface
    {
        SetupController controller;

        private string abortionCode = "Q";

        public void Interact()
        {
            controller = new SetupController();
            CLI.DisplayLine("Welcome to the DiagnosticChain!");
            CLI.DisplayLineDelimiter();

            var userInput = PromptUsername();
            if (userInput == abortionCode) return;

            if (userInput == "") userInput = SetupNewUser();
            if (userInput == abortionCode) return;

            controller.SetupForUser(userInput).Interact();
        }

        public void PrepareForUser(string username)
        {
            throw new NotImplementedException();
        }

        public string PromptNodeType()
        {
            var nodetypes = controller.GetNodeTypes();
            var promptMessage = "Please specify this node's type. Enter " + abortionCode + " to quit";
            var response = CLI.PromptUser(promptMessage);

            while (!nodetypes.Contains(response) && response != abortionCode)
            {
                CLI.DisplayLine("Handler type not found, please try again. The following options are available: ");
                foreach (string n in nodetypes)
                {
                    CLI.DisplayLine(n);
                }

                CLI.DisplayLineDelimiter();
                response = CLI.PromptUser(promptMessage);
            }

            return response;
        }

        public string PromptUsername()
        {
            var newUserCode = "C";
            var promptMessage = "Please provide your username. If you do not have a user on this machine yet, enter " + newUserCode + " to continue. Enter " + abortionCode + " to quit";
            var response = CLI.PromptUser(promptMessage);

            while (!controller.HasUser(response) && response != abortionCode) {
                if (response == newUserCode) return "";

                CLI.DisplayLine("User \"" + response + "\" not found");
                response = CLI.PromptUser(promptMessage);
            }

            return response;
        }

        public string SetupNewUser()
        {
            var promptMessage = "Please choose a new username for this client. Enter " + abortionCode + " to quit";
            var response = CLI.PromptUser(promptMessage);

            while (controller.HasUser(response) && response != abortionCode)
            {
                CLI.DisplayLine("Username \"" + response + "\" is already taken");
                response = CLI.PromptUser(promptMessage);
            }

            if (response == abortionCode) return abortionCode;

            var nodetype = PromptNodeType();
            if (nodetype == abortionCode) return abortionCode;

            controller.AddUser(response, nodetype);
            return response;
        }
    }
}
