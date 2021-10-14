using System.Collections.Generic;

namespace Furball.Engine.Engine.Console.ConFuncs.Standard {
    public class DeleteVar : ConFunc {

        public DeleteVar() : base("delete_var") {}
        public override string Run(string consoleInput) {
            ConVar variable = Console.RegisteredConVars.GetValueOrDefault(consoleInput, null);

            if (variable == null)
                return "Variable not found.";

            if (!variable.ScriptCreated)
                return "Variables that have not been created by Script cannot be Removed.";

            Console.RemoveConVar(variable.Name);

            return "Variable Removed successfully";
        }
    }
}
