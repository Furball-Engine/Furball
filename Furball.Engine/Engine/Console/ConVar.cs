using System;

namespace Furball.Engine.Engine.DevConsole {
    public abstract class ConVar {
        public string Name { get; init; }
        public EventHandler     OnChange;
        public bool             ScriptCreated   = false;
        public bool             ReadOnly        = false;
        public bool             DisableOnChange = false;
        public bool             Protected       = false;
        public Func<bool, bool> PrivledgeCheck;
        public ConVar(string conVarName, Action onChange = null) {
            this.Name = conVarName;

            if (onChange != null)
                this.OnChange += delegate {
                    onChange();
                };
        }

        public virtual bool CheckPrivledges(bool userRun) => this.PrivledgeCheck.Invoke(userRun);

        public virtual (ExecutionResult result, string message) Set(string consoleInput) {
            if(!this.DisableOnChange)
                this.OnChange?.Invoke(this, null);

            return (ExecutionResult.Success, string.Empty);
        }
    }
}
