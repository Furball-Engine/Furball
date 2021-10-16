using System;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class IntConVar : ConVar {
        public Bindable<int> Value;

        public IntConVar(string conVarName, int initialValue = 0, Action onChange = null) : base(conVarName, onChange) => this.Value = new Bindable<int>(initialValue);

        public override (ExecutionResult result, string message) Set(string consoleInput) {
            try {
                this.Value.Value = int.Parse(consoleInput);

                base.Set(string.Empty);

                return (ExecutionResult.Success, $"{this.Name} set to {this.Value.Value}");
            } catch (ArgumentException) {
                return (ExecutionResult.Error, "`consoleInput` was null, how? i have no clue");
            } catch (FormatException) {
                return (ExecutionResult.Error, "Failed to parse input into a +int");
            } catch (OverflowException) {
                return (ExecutionResult.Error, "Number parsed is too big to fit into a +int");
            }
        }

        public override string ToString() => this.Value.ToString();
    }
}
