namespace Furball.Engine.Engine.Console.Types {
    public class StringConVar : ConVar {
        public string Value { get; private set; }

        public StringConVar(string conVarName) : base(conVarName) {}

        public override void Set(string consoleInput) {
            this.Value = consoleInput;
        }

        public override string ToString() => this.Value;
    }
}
