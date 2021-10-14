using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Helpers;
using Jace;
using Jace.Execution;

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
            AddConFunc(ConVars.ClearContentCache);

            if (!Directory.Exists(ScriptPath)) Directory.CreateDirectory(ScriptPath);
        }

        public static void AddConVar(ConVar conVar) => _conVars.Add(conVar.Name, conVar);
        public static void AddConFunc(ConFunc conFunc) => _conFuncs.Add(conFunc.Name, conFunc);

        public static string ScriptPath = "scripts";

        public static async Task RunFile(string filename) {
            byte[] data = ContentManager.LoadRawAsset(Path.Combine(ScriptPath, filename), ContentSource.External, true);

            string file = Encoding.Default.GetString(data);

            await Task.Run(
            () => {
                string[] split = file.Replace("\r", "").Split("\n");

                foreach (string line in split)
                    Run(line);
            }
            );
        }
        
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

                int index;
                do {
                    index = toConcat.IndexOf('$');
                    if (index == -1) break;

                    string subString = toConcat[index..];

                    Match  match         = Regex.Match(subString, "^([\\S]+)");
                    string matchedString = match.Groups[0].Value;

                    ConVar value = _conVars.GetValueOrDefault(matchedString.TrimStart('$'), null);

                    toConcat = toConcat.Replace(matchedString, value?.ToString() ?? "~~Error: Variable not found!");

                } while (true);

                argumentString += toConcat + " ";
            }

            argumentString = argumentString.Trim();

            CalculationEngine jaceEngine = new(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            bool run = true;
            do {
                try {
                    string match = argumentString.SubstringWithEnds("#(", ")");

                    argumentString = argumentString.Replace(match, jaceEngine.Calculate(argumentString.Substring("#(", ")")).ToString(CultureInfo.InvariantCulture));
                }
                catch (ArgumentException ex) {
                    run = false;
                }
            } while (run);

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
