using System;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.DevConsole.Types {
    public class DoubleConVar : ConVar {
        public Bindable<double> BindableValue;
        public double Value {
            get {
                return this.BindableValue.Value;
            }
            set {
                this.BindableValue.Value = value;
            }
        }

        public DoubleConVar(string conVarName, double initialValue = 0d, Action onChange = null) : base(conVarName, onChange) => this.BindableValue = new Bindable<double>(initialValue);

        public override ConsoleResult Set(string consoleInput) {
            try {
                this.Value = double.Parse(consoleInput);

                base.Set(string.Empty);

                return new ConsoleResult(ExecutionResult.Success, $"{this.Name} set to {this.Value}");
            } catch (ArgumentException) {
                return new ConsoleResult(ExecutionResult.Error, "`consoleInput` was null, how? i have no clue");
            } catch (FormatException) {
                return new ConsoleResult(ExecutionResult.Error, "Failed to parse input into a +double");
            } catch (OverflowException) {
                return new ConsoleResult(ExecutionResult.Error, "Number parsed is too big to fit into a +double");
            }
        }

        public override string ToString() => this.Value.ToString();
    }
}
