using System.Collections.Generic;
using Furball.Engine.Engine.Console.Types;

namespace Furball.Engine.Engine.Console {
    public class Console {
        private static readonly Dictionary<string, ConVar>   _conVars   = new();
        private static readonly Dictionary<string, Function> _functions = new();

        public delegate string Function(string args);

        public static void Initialize() {
            AddConVar(ConVars.TestVar);
            AddConVar(ConVars.ResolutionWidth);
            AddConVar(ConVars.ResolutionHeight);

            AddFunction(
            "quit",
            delegate {
                FurballGame.Instance.Exit();
                return "Exiting game!";
            }
            );
            
            AddFunction(
            "set_screen_resolution",
            delegate {
                int width  = ((IntConVar)_conVars.GetValueOrDefault("screen_resolution_width",  new IntConVar("a", FurballGame.DEFAULT_WINDOW_WIDTH))).Value.Value;
                int height = ((IntConVar)_conVars.GetValueOrDefault("screen_resolution_height", new IntConVar("a", FurballGame.DEFAULT_WINDOW_HEIGHT))).Value.Value;
                
                FurballGame.Instance.ChangeScreenSize(width, height);
                return $"Setting the screen resolution to {width}x{height}!";
            }
            );
        }

        public static void AddConVar(ConVar   conVar)                  => _conVars.Add(conVar.Name, conVar);
        public static void AddFunction(string name, Function function) => _functions.Add(name, function); 
        
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

                Function function = _functions.GetValueOrDefault(functionName, delegate { return "Unknown Function!"; });

                returnString = function?.Invoke(argumentString) ?? "Unknown Function! Did you mean to set a variable? Remove the :";
            }

            return returnString;
        }
    }
}
