using System;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.DevConsole.Types {
    public class StringConVar : ConVar {
        public Bindable<string> Value;

        public StringConVar(string conVarName, string initialValue = "", Action onChange = null) : base(conVarName, onChange) => this.Value = new Bindable<string>(initialValue);

        public override ConsoleResult Set(string consoleInput) {
            this.Value.Value = consoleInput;

            base.Set(string.Empty);

            return new ConsoleResult(ExecutionResult.Success, $"{this.Name} set to {this.Value.Value}");
        }

        public override string ToString() => this.Value;
    }
}
