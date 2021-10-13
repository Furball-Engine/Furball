using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class DoubleConVar : ConVar {
        public Bindable<double> Value;

        public DoubleConVar(string conVarName) : base(conVarName) {}

        public override void Set(string consoleInput) {
            this.Value.Value = double.Parse(consoleInput);
        }

        public override string ToString() => this.Value.ToString();
    }
}
