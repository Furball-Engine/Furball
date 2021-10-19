using System;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.DevConsole.Types {
    public class StringConVar : ConVar {
        public Bindable<string> BindableValue;

        public string Value {
            get {
                return this.BindableValue.Value;
            }
            set {
                this.BindableValue.Value = value;
            }
        }

        public StringConVar(string conVarName, string initialValue = "", Action onChange = null) : base(conVarName, onChange) => this.BindableValue = new Bindable<string>(initialValue);

        public override ConsoleResult Set(string consoleInput) {
            this.Value = consoleInput;

            base.Set(string.Empty);

            return new ConsoleResult(ExecutionResult.Success, $"{this.Name} set to {this.Value}");
        }

        public override string ToString() => this.Value;
    }
}
