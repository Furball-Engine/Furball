using System;
using System.Runtime.CompilerServices;

namespace Furball.Engine.Engine.Console {
    public abstract class ConVar {
        public string Name { get; init; }

        public ConVar(string conVarName) {
            this.Name = conVarName;
        }

        public abstract void Set(string consoleInput);
    }
}
