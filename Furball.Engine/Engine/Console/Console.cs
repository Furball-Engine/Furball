using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

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
            AddConFunc(ConVars.PrintFunction);
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

            string argumentString = "";

            for (int i = 1; i != splitCommand.Length; i++) {
                string toConcat = splitCommand[i];

                if (toConcat[0] == '$') {
                    string variableName = toConcat.TrimStart('$');

                    ConVar value = _conVars.GetValueOrDefault(variableName, null);

                    toConcat = value != null ? value.ToString() : "~~Error: Variable not found!";
                }

                argumentString += toConcat + " ";
            }

            argumentString = argumentString.Trim();


            if (variableAssign) {
                string variableName = splitCommand[0];

                ConVar var = _conVars.GetValueOrDefault(variableName, null);

                returnString = var?.Set(argumentString) ?? "Unknown Variable! Did you mean to use a function? Prefix it with :";
            } else {
                string functionName   = splitCommand[0].TrimStart(':');

                ConFunc func = _conFuncs.GetValueOrDefault(functionName, null);

                returnString = func?.Run(argumentString) ?? "Unknown Function! Did you mean to set a variable? Remove the :";
            }

            return returnString;
        }
    }
}
