namespace Furball.Engine.Engine.Console {
    public abstract class ConVar {
        public string Name { get; init; }

        public ConVar(string conVarName) {
            this.Name = conVarName;
        }

        public abstract string Set(string consoleInput);
    }
}
