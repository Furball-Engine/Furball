using System.Collections.Generic;

namespace Furball.Engine.Engine.Console.ConFuncs.Standard {
    public class Hook : ConFunc {
        public Hook() : base("hook") {

        }
        public override (ExecutionResult result, string message) Run(string consoleInput) {
            string[] arguments = consoleInput.Split(" ");

            if (arguments.Length != 3)
                return (ExecutionResult.Error, "Invalid Syntax. Example: `hook +variable hook_target hook_action`");

            string hookType = arguments[0];
            string hookTarget = arguments[1];
            string hookAction = arguments[2];

            switch (hookType) {
                //TODO: add function hooks
                case "+variable":
                    ConVar variable = Console.RegisteredConVars.GetValueOrDefault(hookTarget, null);
                    ConFunc action = Console.RegisteredFunctions.GetValueOrDefault(hookAction, null);

                    if (variable != null) {
                        if (action != null) {
                            variable.OnChange += delegate {
                                action.Run(string.Empty);
                            };
                        } else return (ExecutionResult.Error, "No such Function Found!");
                    } else return (ExecutionResult.Error, "No such Variable found!");

                    break;
            }

            return (ExecutionResult.Success, "Hook established.");
        }
    }
}
