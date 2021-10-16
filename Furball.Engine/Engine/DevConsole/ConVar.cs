using System;

namespace Furball.Engine.Engine.DevConsole {
    public abstract class ConVar {
        public string Name { get; init; }
        public EventHandler     OnChange;
        public bool             ScriptCreated   = false;
        public bool             ReadOnly        = false;
        public bool             DisableOnChange = false;
        public bool             Protected       = false;
        public Func<bool, bool> PrivilegeCheck;
        public ConVar(string conVarName, Action onChange = null) {
            this.Name = conVarName;

            if (onChange != null)
                this.OnChange += delegate {
                    onChange();
                };
        }

        public virtual bool CheckPrivileges(bool userRun) => this.PrivilegeCheck.Invoke(userRun);

        public virtual ConsoleResult Set(string consoleInput) {
            if(!this.DisableOnChange)
                this.OnChange?.Invoke(this, null);

            return new ConsoleResult(ExecutionResult.Success, string.Empty);
        }
    }
}
