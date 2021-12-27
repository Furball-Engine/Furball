using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Helpers;
using Furball.Volpe.Evaluation;
using Furball.Volpe.Exceptions;
using Furball.Volpe.LexicalAnalysis;
using Furball.Volpe.SyntaxAnalysis;
using Jace;
using Jace.Execution;
using JetBrains.Annotations;

namespace Furball.Engine.Engine.DevConsole {
    public class DevConsole {
        public static readonly ObservableCollection<(string input, ConsoleResult)> ConsoleLog = new();

        public static string ScriptPath = "scripts";
        public static string LogPath = "logs";

        private static readonly Volpe.Evaluation.Environment VolpeEnvironment 
            = new Volpe.Evaluation.Environment(DefaultBuiltins.Core.Concat(Builtins.Collection).ToArray());
        
        public static void Initialize() {
            if (!Directory.Exists(ScriptPath)) Directory.CreateDirectory(ScriptPath);
            if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);

            AddMessage("DevConsole is initialized!");
        }
        
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
        
        public static void Run(string input, bool userRun = true, bool disableLog = false) {
            input = input.Trim('\0');
            AddMessage("] " + input, ExecutionResult.Message);
            
            IEnumerable<Token> tokenStream = new Lexer(input).GetTokenEnumerator();
            Parser parser = new Parser(tokenStream);

            try {
                while (parser.TryParseNextExpression(out Expression expression)) {
                    new EvaluatorContext(expression!, VolpeEnvironment).Evaluate();
                }
            }
            catch (VolpeException exception) {
                AddMessage("Error: " + exception.Description, ExecutionResult.Error);
            }
        }

        #region Console Log Helper

        public static void AddMessage(string message, ExecutionResult execResult = ExecutionResult.Message, string action = "") => 
            ConsoleLog.Add((action, new ConsoleResult(execResult, message)));

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

        public static string WriteLog() {
            if (!ConVars.WriteLog) {
                throw new InvalidOperationException("Writing Logs disabled! change `cl_console_log` to 1 to enable Console Logging");
            }

            string filename = Path.Combine(LogPath, $"{UnixTime.Now()}-console.txt");

            try {
                File.WriteAllLines(filename, GetLog());
            }
            catch {
                throw new Exception("Something went wrong. Make sure the `log` directory exists, and is not write protected");
            }

            return filename;
        }
    }
}
