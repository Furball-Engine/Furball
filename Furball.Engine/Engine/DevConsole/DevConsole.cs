using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Helpers;
using Jace;
using Jace.Execution;

namespace Furball.Engine.Engine.DevConsole {
    public class DevConsole {
        public static readonly Dictionary<string, ConVar>  RegisteredConVars   = new();
        public static readonly Dictionary<string, ConFunc> RegisteredFunctions = new();

        public static readonly List<(string input, ConsoleResult)> ConsoleLog = new();

        public static string ScriptPath = "scripts";
        public static string LogPath = "logs";

        private static readonly string[] AutoRun = new[] {
            //Hooks cl_screen_resolution to cl_set_screen_resolution
            //so that when cl_screen_resolution gets changed it automatically calls cl_set_screen_resolution
            ":hook +variable cl_screen_resolution cl_set_screen_resolution",
            //Hooks cl_target_fps to cl_set_target_fps
            //So that when cl_target_fps gets changed it automatically calls cl_set_target_fps
            ":hook +variable cl_target_fps cl_set_target_fps",
            //Hooks cl_fps_unfocused_scale to cl_set_fps_unfocused_scale
            //So that when cl_fps_unfocused_scale gets changed it automatically calls cl_set_fps_unfocused_scale
            ":hook +variable cl_fps_unfocused_scale cl_set_fps_unfocused_scale",
            //Sets target framerate to cl_target_fps
            ":cl_set_target_fps $cl_target_fps",
            //Sets the Unfocused Scale to cl_fps_unfocused_scale
            ":cl_set_fps_unfocused_scale $cl_fps_unfocused_scale"
        };

        public static void Initialize() {
            if (!Directory.Exists(ScriptPath)) Directory.CreateDirectory(ScriptPath);
            if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);

            //Get all ConVars defined in `ConVars`
            FieldInfo[] fields = typeof(ConVars).GetFields();

            for (int i = 0; i != fields.Length; i++) {
                FieldInfo currentField = fields[i];

                if(currentField.FieldType.IsSubclassOf(typeof(ConVar))) {
                    //apperantly when the field is static u can use null in GetValue
                    AddConVar((ConVar)currentField.GetValue(null));
                }
            }

            AddMessage($"Found all Engine ConVars, {RegisteredConVars.Count} ConVars found");

            //Get all classes that Inherit from `ConFunc` in all Loaded Assemblies
            List<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsSubclassOf(typeof(ConFunc)))
                .ToList();

            for (int i = 0; i != types.Count; i++) {
                Type currentType = types[i];

                ConFunc function = (ConFunc)Activator.CreateInstance(currentType);

                AddConFunc(function);
            }

            AddMessage($"Found all ConFuncs, {RegisteredFunctions.Count} ConFuncs found");

            AddMessage("Executing AutoRun...");

            //Run AutoRun
            for (int i = 0; i != AutoRun.Length; i++) {
                Run(AutoRun[i]);
            }

            AddMessage("AutoRun script complete!");
        }
        //Because the fields are required to be static this is the only way that i can think of for devs to add their own ConVarStores like `ConVars`
        public static void AddConVarStore(Type store) {
            if (!store.IsSubclassOf(typeof(ConVarStore)))
                throw new Exception("What you are trying to add is not a ConVarStore!");

            FieldInfo[] fields = store.GetFields();

            int count = 0;

            for (int i = 0; i != fields.Length; i++) {
                FieldInfo currentField = fields[i];

                if(currentField.FieldType.IsSubclassOf(typeof(ConVar))) {
                    //apperantly when the field is static u can use null in GetValue
                    AddConVar((ConVar)currentField.GetValue(null));

                    count++;
                }
            }

            AddMessage($"Added ConVarStore {store.Name}, {count} ConVars found. {RegisteredConVars.Count} total");
        }

        public static void AddConVar(ConVar conVar) => RegisteredConVars.Add(conVar.Name, conVar);
        public static void RemoveConVar(string name) => RegisteredConVars.Remove(name);
        public static void AddConFunc(ConFunc conFunc) => RegisteredFunctions.Add(conFunc.Name, conFunc);
        public static void RemoveConFunc(string name) => RegisteredFunctions.Remove(name);

        public static async Task RunFile(string filename) {
            byte[] data = ContentManager.LoadRawAsset(Path.Combine(ScriptPath, filename), ContentSource.External, true);

            string file = Encoding.Default.GetString(data);

            AddMessage($"Running file { Path.GetFileName(filename) }");

            await Task.Run(
                () => {
                    string[] split = file.Replace("\r", "").Split("\n");

                    foreach (string line in split)
                        Run(line);
                }
            );
        }
        
        public static ConsoleResult Run(string input, bool userRun = true, bool disableLog = false) {
            ConsoleResult returnResult = new ConsoleResult(ExecutionResult.Success, "");

            List<(string, ConsoleResult)> postExecResults = new();

            if (input.Length == 0) {
                returnResult = new ConsoleResult(ExecutionResult.Error, returnResult.Message);
                goto ConsoleEnd;
            }

            string[] splitCommand = input.Split(" ");

            if (splitCommand.Length == 0) {
                returnResult = new ConsoleResult(ExecutionResult.Error, "Invalid Syntax.");
                goto ConsoleEnd;
            }

            bool variableAssign = input[0] != ':';

            string argumentString = "";

            //Check for Variable refrences
            for (int i = 1; i != splitCommand.Length; i++) {
                string toConcat = splitCommand[i];

                argumentString += toConcat + " ";
            }

            argumentString = argumentString.Trim();

            CalculationEngine jaceEngine = new(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

            bool run = true;
            int evalIndex = 0;

            #region Strict Eval Blocks (Breaks when Eval returns an Error)

            do {
                try {
                    string match = (argumentString + " ").SubstringWithEnds("%(", ")");
                    string code = match.Substring("%(", ")");

                    ConsoleResult result = Run(code, userRun, true);

                    if (result.Result == ExecutionResult.Error) {
                        returnResult = new ConsoleResult(ExecutionResult.Error, $"Eval at index {evalIndex} failed to finish. Error Message: \"{result.Message}\"");
                        goto ConsoleEnd;
                    }

                    argumentString = argumentString.Replace(match, result.Message);

                    evalIndex++;
                }
                catch (ArgumentException ex) {
                    run = false;
                }
            } while (run);

            #endregion

            #region Lenient Eval Blocks (Dont break when Eval returns an error, instead just returns string.Empty)

            run = true;

            do {
                try {
                    string match = (argumentString + " ").SubstringWithEnds("&(", ")");
                    string code = match.Substring("&(", ")");

                    ConsoleResult result = Run(code, userRun, true);

                    if (result.Result == ExecutionResult.Error) {
                        postExecResults.Add((string.Empty, new ConsoleResult(ExecutionResult.Warning, $"Eval at index {evalIndex} failed to finish. Error Message: \"{result.Message}\"")));
                        result.Message = "";
                    }

                    argumentString = argumentString.Replace(match, result.Message);

                    evalIndex++;
                }
                catch (ArgumentException ex) {
                    run = false;
                }
            } while (run);


            #endregion

            #region Math Blocks

            run = true;

            do {
                try {
                    string match = argumentString.SubstringWithEnds("#(", ")");

                    argumentString = argumentString.Replace(match, jaceEngine.Calculate(argumentString.Substring("#(", ")")).ToString(CultureInfo.InvariantCulture));
                }
                catch (ArgumentException ex) {
                    run = false;
                }
            } while (run);

            #endregion

            #region Variable Evals

            int index;

            do {
                index = argumentString.IndexOf('$');
                if (index == -1) break;

                string subString = argumentString[index..];

                Match  match         = Regex.Match(subString, "^([\\S]+)");
                string matchedString = match.Groups[0].Value;
                string variableName = matchedString.TrimStart('$');

                ConVar value = RegisteredConVars.GetValueOrDefault(variableName, null);

                if (value == null) {
                    returnResult = new ConsoleResult(ExecutionResult.Error, $"Variable of name `{variableName}` not found!");
                    goto ConsoleEnd;
                }

                if (value.Protected && value.CheckPrivileges(userRun) == false) {
                    returnResult = new ConsoleResult(ExecutionResult.Error, $"Variable of name `{variableName}` is protected and you are not privledged to access it.");
                    goto ConsoleEnd;

                }

                argumentString = argumentString.Replace(matchedString, value.ToString());

            } while (true);

            #endregion

            if (variableAssign) {
                string variableName = splitCommand[0];

                ConVar var = RegisteredConVars.GetValueOrDefault(variableName, null);

                if (var != null) {
                    if (var.ReadOnly)
                        returnResult = new ConsoleResult(ExecutionResult.Error, "Variable is read-only!");
                    else if (var.Protected && var.CheckPrivileges(userRun) == false) {
                        returnResult = new ConsoleResult(ExecutionResult.Error, "Variable is protected and you are not privledged to access it.");
                    }
                    else {
                        ConsoleResult result = var.Set(argumentString);

                        returnResult = result;
                    }
                } else {
                    returnResult = new ConsoleResult(ExecutionResult.Error, "Unknown Variable! Did you mean to use a function? Prefix it with :");
                }

            } else {
                string functionName = splitCommand[0].TrimStart(':');

                ConFunc func = RegisteredFunctions.GetValueOrDefault(functionName, null);

                if (func != null) {
                    ConsoleResult result = func.Run(argumentString);
                    returnResult = result;

                    func.CallOnCall(result.Message);
                } else {
                    returnResult = new ConsoleResult(ExecutionResult.Error, "Unknown Function! Did you mean to set a variable? Remove the :");
                }
            }

            ConsoleEnd: ;

            if (!disableLog) {
                ConsoleLog.Add((input, returnResult));

                if (postExecResults.Count != 0) {
                    AddMessage("Post Execution Warnings/Messages:");
                    ConsoleLog.AddRange(postExecResults);
                }
            }

            return returnResult;
        }

        #region Console Log Helper

        public static void AddMessage(string message) => ConsoleLog.Add((string.Empty, new ConsoleResult(ExecutionResult.Message, message)));

        #endregion

        public static string[] GetLog() {
            List<string> lines = new List<string>();

            foreach ((string input, ConsoleResult result) action in ConsoleLog) {
                lines.AddRange(FormatActionLine(action));
            }

            return lines.ToArray();
        }

        public static List<string> FormatActionLine((string input, ConsoleResult result) action) {
            List<string> lines = new List<string>();

            if(action.input != string.Empty)
                lines.Add($"] {action.input}");

            switch (action.result.Result) {
                case ExecutionResult.Message:
                    lines.Add(action.result.Message);
                    break;
                case ExecutionResult.Log:
                    lines.Add($"::[Log] {action.result.Message}");
                    break;
                case ExecutionResult.Success:
                case ExecutionResult.Warning:
                case ExecutionResult.Error:
                    lines.Add($"[{action.result.Result}] {action.result.Message}");
                    break;
            }

            return lines;
        }

        public static ConsoleResult WriteLog() {
            if (ConVars.WriteLog.Value != 1)
                return new ConsoleResult(ExecutionResult.Error, "Writing Logs disabled! change `cl_console_log` to 1 to enable Console Logging");

            string filename = Path.Combine(LogPath, $"{UnixTime.Now()}-console.txt");

            try {
                File.WriteAllLines(filename, GetLog());
            }
            catch (Exception e) {
                return new ConsoleResult(ExecutionResult.Error, "Something went wrong. Make sure the `log` directory exists, and is not write protected.");
            }

            return new ConsoleResult(ExecutionResult.Success, $"Log successfully written to `{filename}`");
        }
    }
}
