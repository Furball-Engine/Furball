using System.Collections.Generic;

namespace Furball.Engine.Engine.Console {
    public class Console {
        private static Dictionary<string, ConVar> _conVars = new();

        public static void Initialize() {
            AddConVar(ConVars.TestVar);
        }

        public static void AddConVar(ConVar conVar) => _conVars.Add(conVar.Name, conVar);

        public static string Run(string input) {
            string returnString = "";
            
            if (input.Length == 0)
                return returnString;

            string[] splitCommand = input.Split(" ");

            if (splitCommand.Length == 0)
                return "Invalid Syntax.";

            bool variableAssign = input[0] != ':';

            if (variableAssign) {
                string variableName = splitCommand[0];
                string argumentString = "";

                for (int i = 1; i != splitCommand.Length; i++) {
                    argumentString += splitCommand[i] + " ";
                }

                argumentString = argumentString.Trim();

                ConVar var = _conVars.GetValueOrDefault(variableName, null);

                returnString = var?.Set(argumentString);
            }

            return returnString;
        }
    }
}
