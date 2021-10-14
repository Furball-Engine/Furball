using System.Text.RegularExpressions;
using Furball.Engine.Engine.Console.Types;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Console.ConFuncs.Standard {
    public class CreateVar : ConFunc {
        public CreateVar() : base("create_var") {

        }
        public override string Run(string consoleInput) {
            string[] split = consoleInput.Split(" ");

            string type = split[0];
            string name = split[1];

            if (Regex.IsMatch(name, "/[!@#$%^&*()]/"))
                return "Invalid Variable Name.";

            if (Console.RegisteredConVars.ContainsKey(name))
                return "Variable of this name already exists.";

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
                case "+intint":
                    variable = new IntIntConVar(name, "");
                    break;
                case "+string":
                    variable = new StringConVar(name);
                    break;
                default:
                    return "Invalid Variable Type";
            }

            variable.ScriptCreated = true;

            Console.AddConVar(variable);

            return "Variable Created.";
        }
    }
}
