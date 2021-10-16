using System;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class StringConVar : ConVar {
        public Bindable<string> Value;

        public StringConVar(string conVarName, string initialValue = "", Action onChange = null) : base(conVarName, onChange) => this.Value = new Bindable<string>(initialValue);

        public override (ExecutionResult result, string message) Set(string consoleInput) {
            this.Value.Value = consoleInput;

            base.Set(string.Empty);

            return (ExecutionResult.Success, $"{this.Name} set to {this.Value.Value}");
        }

        public override string ToString() => this.Value;
    }
}
