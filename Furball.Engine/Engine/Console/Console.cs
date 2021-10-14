using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Furball.Engine.Engine.Console {
    public class Console {
        private static readonly Dictionary<string, ConVar>   _conVars   = new();
        private static readonly Dictionary<string, ConFunc> _conFuncs = new();

        public static ReadOnlyDictionary<string, ConVar> RegisteredConVars => new(_conVars);
        public static ReadOnlyDictionary<string, ConFunc> RegisteredFunctions => new(_conFuncs);

        public static void Initialize() {
            AddConVar(ConVars.TestVar);
            AddConVar(ConVars.ScreenResolution);

            AddConFunc(ConVars.QuitFunction);
            AddConFunc(ConVars.ScreenResFunction);
        }

        public static void AddConVar(ConVar conVar) => _conVars.Add(conVar.Name, conVar);
        public static void AddConFunc(ConFunc conFunc) => _conFuncs.Add(conFunc.Name, conFunc);
        
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

                returnString = var?.Set(argumentString) ?? "Unknown Variable! Did you mean to use a function? Prefix it with :";
            } else {
                string functionName   = splitCommand[0].TrimStart(':');
                string argumentString = "";

                for (int i = 1; i != splitCommand.Length; i++)
                    argumentString += splitCommand[i] + " ";

                argumentString = argumentString.Trim();

                ConFunc func = _conFuncs.GetValueOrDefault(functionName, null);

                returnString = func?.Run(argumentString) ?? "Unknown Function! Did you mean to set a variable? Remove the :";
            }

            return returnString;
        }
    }
}
