using System;
using System.Collections.Generic;
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
        public static readonly Dictionary<string, ConVar>  RegisteredConVars   = new();
        public static readonly Dictionary<string, ConFunc> RegisteredFunctions = new();

        public static readonly List<(string input, ExecutionResult result, string message)> ConsoleLog = new();

        public static void Initialize() {
            AddConVar(ConVars.ScreenResolution);
            AddConVar(ConVars.DebugOverlay);

            AddConFunc(ConVars.QuitFunction);
            AddConFunc(ConVars.ScreenResFunction);
            AddConFunc(ConVars.PrintFunction);
            AddConFunc(ConVars.ClearContentCache);
            AddConFunc(ConVars.CreateVariable);
            AddConFunc(ConVars.DeleteVariable);
            AddConFunc(ConVars.Hook);

            var what = Console.Run(":hook +variable cl_screen_resolution cl_set_screen_resolution");

            if (!Directory.Exists(ScriptPath)) Directory.CreateDirectory(ScriptPath);
        }

        public static void AddConVar(ConVar conVar) => RegisteredConVars.Add(conVar.Name, conVar);
        public static void RemoveConVar(string name) => RegisteredConVars.Remove(name);
        public static void AddConFunc(ConFunc conFunc) => RegisteredFunctions.Add(conFunc.Name, conFunc);
        public static void RemoveConFunc(string name) => RegisteredFunctions.Remove(name);


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
        
        public static (ExecutionResult result, string message) Run(string input) {
            (ExecutionResult result, string message) returnResult = (ExecutionResult.Error, "");

            if (input.Length == 0) {
                returnResult = (ExecutionResult.Error, returnResult.message);
                goto ConsoleEnd;
            }

            string[] splitCommand = input.Split(" ");

            if (splitCommand.Length == 0) {
                returnResult =  (ExecutionResult.Error, "Invalid Syntax.");
                goto ConsoleEnd;
            }

            bool variableAssign = input[0] != ':';

            string argumentString = "";

            //Check for Variable refrences
            for (int i = 1; i != splitCommand.Length; i++) {
                string toConcat = splitCommand[i];

                int index;
                do {
                    index = toConcat.IndexOf('$');
                    if (index == -1) break;

                    string subString = toConcat[index..];

                    Match  match         = Regex.Match(subString, "^([\\S]+)");
                    string matchedString = match.Groups[0].Value;
                    string variableName = matchedString.TrimStart('$');

                    ConVar value = RegisteredConVars.GetValueOrDefault(variableName, null);

                    if (value == null) {
                        returnResult = (ExecutionResult.Error, $"Variable of name `{variableName}` not found!");
                        goto ConsoleEnd;
                    }

                    toConcat = toConcat.Replace(matchedString, value.ToString());

                } while (true);

                argumentString += toConcat + " ";
            }

            argumentString = argumentString.Trim();

            CalculationEngine jaceEngine = new(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            bool run = true;
            int evalIndex = 0;
            //Check for evaluatable Code Blocks
            do {
                try {
                    string match = (argumentString + " ").SubstringWithEnds("%(", ")");
                    string code = match.Substring("%(", ")");

                    (ExecutionResult result, string message) result = Run(code);

                    if (result.result == ExecutionResult.Error) {
                        returnResult =  (ExecutionResult.Error, $"Eval at index {evalIndex} failed to finish. Error Message: \"{result.message}\"");
                        goto ConsoleEnd;
                    }

                    argumentString = argumentString.Replace(match, result.message);

                    evalIndex++;
                }
                catch (ArgumentException ex) {
                    run = false;
                }
            } while (run);

            run = true;

            //Check for Math Expressions
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

                ConVar var = RegisteredConVars.GetValueOrDefault(variableName, null);

                if (var != null) {
                    if (var.ReadOnly)
                        returnResult = (ExecutionResult.Error, "Variable is read-only!");
                    else {
                        (ExecutionResult result, string message) result = var.Set(argumentString);

                        returnResult = result;
                    }
                } else {
                    returnResult = (ExecutionResult.Error, "Unknown Variable! Did you mean to use a function? Prefix it with :");
                }

            } else {
                string functionName   = splitCommand[0].TrimStart(':');

                ConFunc func = RegisteredFunctions.GetValueOrDefault(functionName, null);

                if (func != null) {
                    (ExecutionResult result, string message) result = func.Run(argumentString);
                    returnResult = result;

                    func.CallOnCall(result.message);
                } else {
                    returnResult = (ExecutionResult.Error, "Unknown Function! Did you mean to set a variable? Remove the :");
                }
            }

            ConsoleEnd: ;

            ConsoleLog.Add((input, returnResult.result, returnResult.message));

            return returnResult;
        }

        public void WriteLog() {
            //Will soon do
        }
    }
}
