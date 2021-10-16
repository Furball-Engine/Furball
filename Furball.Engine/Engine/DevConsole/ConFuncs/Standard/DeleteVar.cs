using System.Collections.Generic;

namespace Furball.Engine.Engine.DevConsole.ConFuncs.Standard {
    public class DeleteVar : ConFunc {
        /// <summary>
        /// delete_var
        /// Deletes a Variable
        /// Syntax: `delete_var variable_name`
        /// <remarks>Only Variables that have been created with `create_var` are allowed to be deleted</remarks>
        /// </summary>
        public DeleteVar() : base("delete_var") {}

        public override ConsoleResult Run(string consoleInput) {
            ConVar variable = DevConsole.RegisteredConVars.GetValueOrDefault(consoleInput, null);

            if (variable == null)
                return new ConsoleResult(ExecutionResult.Error, "Variable not found.");

            if (!variable.ScriptCreated)
                return new ConsoleResult(ExecutionResult.Error, "Variables that have not been created by a script cannot be removed.");

            DevConsole.RemoveConVar(variable.Name);

            return new ConsoleResult(ExecutionResult.Success, "Variable Removed successfully");
        }
    }
}
