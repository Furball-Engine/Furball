using System;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class DoubleConVar : ConVar {
        public Bindable<double> Value;

        public DoubleConVar(string conVarName, double initialValue = 0d) : base(conVarName) => this.Value = new Bindable<double>(initialValue);

        public override (ExecutionResult result, string message) Set(string consoleInput) {
            try {
                this.Value.Value = double.Parse(consoleInput);

                base.Set(string.Empty);

                return (ExecutionResult.Success, $"{this.Name} set to {this.Value.Value}");
            } catch (ArgumentException) {
                return (ExecutionResult.Error, "`consoleInput` was null, how? i have no clue");
            } catch (FormatException) {
                return (ExecutionResult.Error, "Failed to parse input into a +double");
            } catch (OverflowException) {
                return (ExecutionResult.Error, "Number parsed is too big to fit into a +double");
            }
        }

        public override string ToString() => this.Value.ToString();
    }
}
