namespace Furball.Engine.Engine.Console.Types {
    public class IntConVar : ConVar {
        public int Value { get; private set; }

        public IntConVar(string conVarName) : base(conVarName) {}

        public override void Set(string consoleInput) {
            this.Value = int.Parse(consoleInput);
        }

        public override string ToString() => this.Value.ToString();
    }
}
