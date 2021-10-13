using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class IntConVar : ConVar {
        public Bindable<int> Value;

        public IntConVar(string conVarName) : base(conVarName) {}

        public override void Set(string consoleInput) {
            this.Value.Value = int.Parse(consoleInput);
        }

        public override string ToString() => this.Value.ToString();
    }
}
