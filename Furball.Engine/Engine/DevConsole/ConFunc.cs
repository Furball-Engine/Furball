using System;

namespace Furball.Engine.Engine.DevConsole {
    public abstract class ConFunc {
        public string Name { get; init; }
        public EventHandler<string> OnCall;
        public bool DisableOnCall = false;

        public ConFunc(string conFuncName) {
            this.Name = conFuncName;
        }

        public abstract ConsoleResult Run(string[] consoleInput);

        public void CallOnCall(string message) {
            if(!this.DisableOnCall)
                this.OnCall?.Invoke(this, message);
        }
    }
}
