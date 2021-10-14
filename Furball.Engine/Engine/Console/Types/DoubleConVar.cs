using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class DoubleConVar : ConVar {
        public Bindable<double> Value;

        public DoubleConVar(string conVarName, double initialValue = 0d) : base(conVarName) => this.Value = new Bindable<double>(initialValue);

        public override string Set(string consoleInput) {
            this.Value.Value = double.Parse(consoleInput);

            return $"{this.Name} set to {this.Value.Value}";
        }

        public override string ToString() => this.Value.ToString();
    }
}
