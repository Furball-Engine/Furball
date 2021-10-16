using System;

namespace Furball.Engine.Engine.Console {
    public abstract class ConVar {
        public string Name { get; init; }
        public EventHandler OnChange;
        public bool         ScriptCreated = false;

        public ConVar(string conVarName) {
            this.Name = conVarName;
        }

        public virtual (ExecutionResult result, string message) Set(string consoleInput) {
            this.OnChange?.Invoke(this, null);

            return (ExecutionResult.Success, string.Empty);
        }
    }
}
