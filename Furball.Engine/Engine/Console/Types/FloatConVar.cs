using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class FloatConVar : ConVar {
        public Bindable<float> Value;

        public FloatConVar(string conVarName, float initialValue = 0f) : base(conVarName) => this.Value = new Bindable<float>(initialValue);

        public override string Set(string consoleInput) {
            this.Value.Value = float.Parse(consoleInput);

            return $"{this.Name} set to {this.Value.Value}";
        }

        public override string ToString() => this.Value.ToString();
    }
}
