using System;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.DevConsole.Types {
    public class FloatConVar : ConVar {
        public Bindable<float> Value;

        public FloatConVar(string conVarName, float initialValue = 0f, Action onChange = null) : base(conVarName, onChange) => this.Value = new Bindable<float>(initialValue);

        public override (ExecutionResult result, string message) Set(string consoleInput) {
            try {
                this.Value.Value = float.Parse(consoleInput);

                base.Set(string.Empty);

                return (ExecutionResult.Success, $"{this.Name} set to {this.Value.Value}");
            } catch (ArgumentException) {
                return (ExecutionResult.Error, "`consoleInput` was null, how? i have no clue");
            } catch (FormatException) {
                return (ExecutionResult.Error, "Failed to parse input into a +float");
            } catch (OverflowException) {
                return (ExecutionResult.Error, "Number parsed is too big to fit into a +float");
            }
        }

        public override string ToString() => this.Value.ToString();
    }
}
