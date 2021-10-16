using System;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.DevConsole.Types {
    public class LongConVar : ConVar {
        public Bindable<long> Value;

        public LongConVar(string conVarName, long initialValue = 0, Action onChange = null) : base(conVarName, onChange) => this.Value = new Bindable<long>(initialValue);

        public override ConsoleResult Set(string consoleInput) {
            try {
                this.Value.Value = long.Parse(consoleInput);

                base.Set(string.Empty);

                return new ConsoleResult(ExecutionResult.Success, $"{this.Name} set to {this.Value.Value}");
            } catch (ArgumentException) {
                return new ConsoleResult(ExecutionResult.Error, "`consoleInput` was null, how? i have no clue");
            } catch (FormatException) {
                return new ConsoleResult(ExecutionResult.Error, "Failed to parse input into a +int");
            } catch (OverflowException) {
                return new ConsoleResult(ExecutionResult.Error, "Number parsed is too big to fit into a +int");
            }
        }

        public override string ToString() => this.Value.ToString();
    }
}
