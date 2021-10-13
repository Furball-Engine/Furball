using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class FloatConVar : ConVar {
        public Bindable<float> Value;

        public FloatConVar(string conVarName) : base(conVarName) {}

        public override void Set(string consoleInput) {
            this.Value.Value = float.Parse(consoleInput);
        }

        public override string ToString() => this.Value.ToString();
    }
}
