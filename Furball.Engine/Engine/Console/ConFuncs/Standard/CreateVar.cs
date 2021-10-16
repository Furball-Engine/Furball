using System.Text.RegularExpressions;
using Furball.Engine.Engine.Console.Types;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Console.ConFuncs.Standard {
    /// <summary>
    /// create_var
    /// Creates a Variable
    /// Syntax: `create_var variable_type variable_name`
    /// </summary>
    public class CreateVar : ConFunc {
        public CreateVar() : base("create_var") {}

        public override (ExecutionResult result, string message) Run(string consoleInput) {
            string[] split = consoleInput.Split(" ");

            string type = split[0];
            string name = split[1];

            if (Regex.IsMatch(name, "/[!@#$%^&*()]/"))
                return (ExecutionResult.Error, "Invalid Variable Name.");

            if (Console.RegisteredConVars.ContainsKey(name))
                return (ExecutionResult.Error, "Variable of this name already exists.");

            ConVar variable = null;

            switch (type) {
                case "+color":
                    variable = new ColorConVar(name, Color.White);
                    break;
                case "+double":
                    variable = new DoubleConVar(name);
                    break;
                case "+float":
                    variable = new FloatConVar(name);
                    break;
                case "+int":
                    variable = new IntConVar(name);
                    break;
                case "+long":
                    variable = new LongConVar(name);
                    break;
                case "+intint":
                    variable = new IntIntConVar(name, "");
                    break;
                case "+string":
                    variable = new StringConVar(name);
                    break;
                default:
                    return (ExecutionResult.Error, "Invalid Variable type.");
            }

            variable.ScriptCreated = true;

            Console.AddConVar(variable);

            return (ExecutionResult.Success, $"Variable of name `{name}` has been created.");
        }
    }
}
