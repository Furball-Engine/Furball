using System;
using System.Collections.Generic;

namespace Furball.Engine.Engine.DevConsole.ConFuncs.Standard {
    /// <summary>
    /// hook
    /// Establishes a Hook (works in a similiar way to Subscribing to OnChange on a Bindable) on a Function/Variable, upon Calling/Changing the Hooked Variable/Function the Hook Gets Invoked
    /// Syntax: `hook +(variable/function) hook_target hook_action`
    /// </summary>
    public class Hook : ConFunc {
        public Hook() : base("hook") {}

        public override ConsoleResult Run(string[] consoleInput) {
            if (consoleInput.Length != 3)
                return new ConsoleResult(ExecutionResult.Error, "Invalid Syntax. Example: `hook +variable hook_target hook_action`");

            string hookType   = consoleInput[0];
            string hookTarget = consoleInput[1];
            string hookAction = consoleInput[2];

            switch (hookType) {
                //TODO: add function hooks
                case "+variable": {
                        ConVar variable = DevConsole.RegisteredConVars.GetValueOrDefault(hookTarget, null);
                        ConFunc action = DevConsole.RegisteredFunctions.GetValueOrDefault(hookAction, null);

                        if (variable != null) {
                            if (action != null) {
                                variable.OnChange += delegate {
                                    action.Run(Array.Empty<string>());
                                };
                            } else return new ConsoleResult(ExecutionResult.Error, "No such Function Found!");
                        } else return new ConsoleResult(ExecutionResult.Error, "No such Variable found!");

                        break;
                    }
                case "+function": {
                    ConFunc function = DevConsole.RegisteredFunctions.GetValueOrDefault(hookTarget, null);
                    ConFunc action = DevConsole.RegisteredFunctions.GetValueOrDefault(hookAction,   null);

                    if (function != null) {
                        if (action != null) {
                            function.OnCall += delegate {
                                action.Run(Array.Empty<string>());
                            };
                        } else return new ConsoleResult(ExecutionResult.Error, "No such Function Found!");
                    } else return new ConsoleResult(ExecutionResult.Error, "No such Variable found!");

                    break;
                }
            }

            return new ConsoleResult(ExecutionResult.Success, "Hook established.");
        }
    }
}
