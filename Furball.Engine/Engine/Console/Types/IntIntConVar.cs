using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Console.Types {
    public class IntIntConVar : ConVar {
        public Bindable<(int, int)> Value = new((0, 0));

        public IntIntConVar(string conVarName, string defaultValue) : base(conVarName) {
            string[] splitInput = defaultValue.Split(" ");

            int x = int.Parse(splitInput[0]);
            int y = int.Parse(splitInput[1]);

            this.Value.Value = (x, y);
        }

        public override string Set(string consoleInput) {
            string[] splitInput = consoleInput.Split(" ");

            int x = int.Parse(splitInput[0]);
            int y = int.Parse(splitInput[1]);

            this.Value.Value = (x, y);

            return $"{this.Name} set to {x}:{y}";
        }

        public override string ToString() => this.Value.ToString();
    }
}
