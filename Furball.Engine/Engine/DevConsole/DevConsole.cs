using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Input;
using Volpe;
using Volpe.Evaluation;
using Volpe.Exceptions;
using Volpe.LexicalAnalysis;
using Volpe.SyntaxAnalysis;

namespace Furball.Engine.Engine.DevConsole {
    public class DevConsole {
        public static readonly Dictionary<string, ConVar>  RegisteredConVars   = new();
        public static readonly Dictionary<string, ConFunc> RegisteredFunctions = new();

        public static readonly List<(string input, ConsoleResult)> ConsoleLog = new();

        public static string ScriptPath = "scripts";
        public static string LogPath = "logs";

        private static Scope _scope;

        private static Queue<string> _outputQueue = new Queue<string>();

        public static void Initialize() {
            if (!Directory.Exists(ScriptPath)) Directory.CreateDirectory(ScriptPath);
            if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);
            
            _scope = new Scope(
                new BuiltinFunction[] {
                    new("print", 1,
                        (_, values) => {
                            _outputQueue.Enqueue(values[0].Representation);

                            return Value.DefaultVoid;
                        }
                    ),
                    
                    new("cl_get_game_time", 0, (_, _) => 
                            new Value.Number(FurballGame.GameTimeSource.GetCurrentTime())),
                    
                    new("cl_set_target_fps", 1, (context, values) => {
                        if (values[0] is not Value.Number(var n))
                            throw new InvalidValueTypeException(typeof(Value.Number), values[0].GetType(), context.Expression.PositionInText);

                        int ni = (int) n;
                        
                        FurballGame.Instance.SetTargetFps(ni);

                        _outputQueue.Enqueue($"Set Target FPS to {ni}");
                        
                        return Value.DefaultVoid;
                    }),

                    new("cl_set_screen_resolution", 2, (context, values) => {
                        if (values[0] is not Value.Number(var width))
                            throw new InvalidValueTypeException(typeof(Value.Number), values[0].GetType(), context.Expression.PositionInText);

                        if (values[1] is not Value.Number(var height))
                            throw new InvalidValueTypeException(typeof(Value.Number), values[1].GetType(), context.Expression.PositionInText);

                        int widthi = (int) width;
                        int heighti = (int) height;
                        
                        FurballGame.Instance.ChangeScreenSize(widthi, heighti);

                        _outputQueue.Enqueue($"Resolution has been set to {widthi}x{heighti}");
                        
                        return Value.DefaultVoid;
                    }),
                    
                    new("quit", 0,
                        (_, _) => {
                            FurballGame.Instance.Exit();

                            _outputQueue.Enqueue("Exiting game.");
                            
                            return Value.DefaultVoid;
                        }),
                    
                    new ("cmr_clear_cache", 0, (_, _) => {
                        Graphics.ContentManager.ClearCache();
                        
                        return Value.DefaultVoid;
                    }),
                    
                    new("im_input_methods", 0,
                        (_, _) => {
                            StringBuilder result = new StringBuilder();

                            for (int i = 0; i != FurballGame.InputManager.RegisteredInputMethods.Count; i++) {
                                InputMethod currentMethod = FurballGame.InputManager.RegisteredInputMethods[i];

                                string typeName = currentMethod.GetType().Name;

                                result.Append($"[{i}] {typeName}\n");
                            }
                            
                            return new Value.String(result.ToString());
                        })
                }.Concat(DefaultBuiltins.Math).Concat(DefaultBuiltins.Core).ToArray());
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
            lock (_outputQueue) {
                try {
                    Parser parser = new Parser(new Lexer(input).GetTokenEnumerator().ToImmutableArray());

                    foreach (Expression expression in parser.GetExpressionEnumerator()) {
                        new EvaluatorContext(expression, _scope).Evaluate();
                    }
                }
                catch (VolpeException exception) {
                    return new ConsoleResult(ExecutionResult.Error, exception.Message);
                }

                string output = _outputQueue.Aggregate(new StringBuilder(), (builder, s) => builder.AppendLine(s)).ToString();
                _outputQueue.Clear();

                return new ConsoleResult(ExecutionResult.Success, output);
            }
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
            catch {
                return new ConsoleResult(ExecutionResult.Error, "Something went wrong. Make sure the `log` directory exists, and is not write protected.");
            }

            return new ConsoleResult(ExecutionResult.Success, $"Log successfully written to `{filename}`");
        }
    }
}
