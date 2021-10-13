using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class IntConVar : ConVar {
        public Bindable<int> Value;

        public IntConVar(string conVarName, int initialValue = 0) : base(conVarName) => this.Value = new(initialValue);

        public override string Set(string consoleInput) {
            this.Value.Value = int.Parse(consoleInput);

            return $"{this.Name} set to {this.Value.Value}";
        }

        public override string ToString() => this.Value.ToString();
    }
}
