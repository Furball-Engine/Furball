using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class StringConVar : ConVar {
        public Bindable<string> Value;

        public StringConVar(string conVarName, string initialValue = "") : base(conVarName) => this.Value = new(initialValue);

        public override string Set(string consoleInput) {
            this.Value.Value = consoleInput;

            return $"{this.Name} set to {this.Value.Value}";
        }

        public override string ToString() => this.Value;
    }
}
