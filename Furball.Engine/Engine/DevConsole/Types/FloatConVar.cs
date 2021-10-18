using System;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.DevConsole.Types {
    public class FloatConVar : ConVar {
        public Bindable<float> BindableValue;
        public float Value {
            get {
                return this.BindableValue.Value;
            }
            set {
                this.BindableValue.Value = value;
            }
        }

        public FloatConVar(string conVarName, float initialValue = 0f, Action onChange = null) : base(conVarName, onChange) => this.BindableValue = new Bindable<float>(initialValue);

        public override ConsoleResult Set(string consoleInput) {
            try {
                this.Value = float.Parse(consoleInput);

                base.Set(string.Empty);

                return new ConsoleResult(ExecutionResult.Success, $"{this.Name} set to {this.Value}");
            } catch (ArgumentException) {
                return new ConsoleResult(ExecutionResult.Error, "`consoleInput` was null, how? i have no clue");
            } catch (FormatException) {
                return new ConsoleResult(ExecutionResult.Error, "Failed to parse input into a +float");
            } catch (OverflowException) {
                return new ConsoleResult(ExecutionResult.Error, "Number parsed is too big to fit into a +float");
            }
        }

        public override string ToString() => this.Value.ToString();
    }
}
