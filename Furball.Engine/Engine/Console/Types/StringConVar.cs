using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class StringConVar : ConVar {
        public Bindable<string> Value;

        public StringConVar(string conVarName) : base(conVarName) {}

        public override void Set(string consoleInput) {
            this.Value.Value = consoleInput;
        }

        public override string ToString() => this.Value;
    }
}
