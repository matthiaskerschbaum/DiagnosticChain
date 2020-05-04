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
        private Action onCompletion;

        public void Interact(Action onCompletion)
        {
            this.onCompletion = onCompletion;
            controller = new SetupController();
            CLI.DisplayLine("Welcome to the DiagnosticChain!");
            CLI.DisplayLineDelimiter();

            var userInput = PromptUsername();
            if (userInput == UIConstants.abortionCode) return;

            if (userInput == "") userInput = SetupNewUser();
            if (userInput == UIConstants.abortionCode) return;

            controller.SetupForUser(userInput).Interact(OnControllerCompletion);
        }

        private void OnControllerCompletion()
        {
            Interact(onCompletion);
        }

        public void PrepareForUser(string username)
        {
            throw new NotImplementedException();
        }

        public string PromptNodeType()
        {
            var nodetypes = controller.GetNodeTypes();
            var promptMessage = "Please specify this node's type. Enter " + UIConstants.abortionCode + " to quit";
            var response = CLI.PromptUser(promptMessage);

            while (!nodetypes.Contains(response) && response != UIConstants.abortionCode)
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
            var promptMessage = "Please provide your username. If you do not have a user on this machine yet, enter " + newUserCode + " to continue. Enter " + UIConstants.abortionCode + " to quit";
            var response = CLI.PromptUser(promptMessage);

            while (!controller.HasUser(response) && response != UIConstants.abortionCode) {
                if (response == newUserCode) return "";

                CLI.DisplayLine("User \"" + response + "\" not found");
                response = CLI.PromptUser(promptMessage);
            }

            return response;
        }

        public string SetupNewUser()
        {
            var promptMessage = "Please choose a new username for this client. Enter " + UIConstants.abortionCode + " to quit";
            var response = CLI.PromptUser(promptMessage);

            while (controller.HasUser(response) && response != UIConstants.abortionCode)
            {
                CLI.DisplayLine("Username \"" + response + "\" is already taken");
                response = CLI.PromptUser(promptMessage);
            }

            if (response == UIConstants.abortionCode) return UIConstants.abortionCode;

            var nodetype = PromptNodeType();
            if (nodetype == UIConstants.abortionCode) return UIConstants.abortionCode;

            controller.AddUser(response, nodetype);
            return response;
        }
    }
}
