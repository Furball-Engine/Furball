using System;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.DevConsole.Types {
    public class IntIntConVar : ConVar {
        public Bindable<(int, int)> BindableValue = new((0, 0));
        public (int, int) Value {
            get {
                return this.BindableValue.Value;
            }
            set {
                this.BindableValue.Value = value;
            }
        }

        public IntIntConVar(string conVarName, string defaultValue, Action onChange = null) : base(conVarName, onChange) {
            if (defaultValue.Length != 0) {
                string[] splitInput = defaultValue.Split(" ");

                int x = int.Parse(splitInput[0]);
                int y = int.Parse(splitInput[1]);

                this.Value = (x, y);
            }
        }

        public override ConsoleResult Set(string consoleInput) {
            string[] splitInput = consoleInput.Split(":");

            try {

                int x = int.Parse(splitInput[0]);
                int y = int.Parse(splitInput[1]);

                this.Value = (x, y);

                base.Set(string.Empty);

                return new ConsoleResult(ExecutionResult.Success, $"{this.Name} set to {x}:{y}");
            } catch (ArgumentException) {
                return new ConsoleResult(ExecutionResult.Error, "`consoleInput` was null, how? i have no clue");
            } catch (FormatException) {
                return new ConsoleResult(ExecutionResult.Error, "Failed to parse input into a +intint");
            } catch (OverflowException) {
                return new ConsoleResult(ExecutionResult.Error, "Number parsed is too big to fit into a +int+int");
            }
        }

        public override string ToString() {
            int val1 = this.Value.Item1;
            int val2 = this.Value.Item2;

            return $"{val1}:{val2}";
        }
    }
}
