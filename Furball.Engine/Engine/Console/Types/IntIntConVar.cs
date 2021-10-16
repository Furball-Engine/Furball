using System;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class IntIntConVar : ConVar {
        public Bindable<(int, int)> Value = new((0, 0));

        public IntIntConVar(string conVarName, string defaultValue) : base(conVarName) {
            if (defaultValue.Length != 0) {
                string[] splitInput = defaultValue.Split(" ");

                int x = int.Parse(splitInput[0]);
                int y = int.Parse(splitInput[1]);

                this.Value.Value = (x, y);
            }
        }

        public override (ExecutionResult result, string message) Set(string consoleInput) {
            string[] splitInput = consoleInput.Split(" ");

            try {

                int x = int.Parse(splitInput[0]);
                int y = int.Parse(splitInput[1]);

                this.Value.Value = (x, y);

                base.Set(string.Empty);

                return (ExecutionResult.Success, $"{this.Name} set to {x}:{y}");
            } catch (ArgumentException) {
                return (ExecutionResult.Error, "`consoleInput` was null, how? i have no clue");
            } catch (FormatException) {
                return (ExecutionResult.Error, "Failed to parse input into a +intint");
            } catch (OverflowException) {
                return (ExecutionResult.Error, "Number parsed is too big to fit into a +int+int");
            }
        }

        public override string ToString() => this.Value.ToString();
    }
}
