using System;

namespace Furball.Engine.Engine.Console {
    public abstract class ConFunc {
        public string Name { get; init; }
        public EventHandler<string> OnCall;

        public ConFunc(string conFuncName) {
            this.Name = conFuncName;
        }

        public abstract (ExecutionResult result, string message) Run(string consoleInput);

        public void CallOnCall(string message) => this.OnCall?.Invoke(this, message);
    }
}
