namespace Furball.Engine.Engine.Console {
    public abstract class ConFunc {
        public string Name { get; init; }

        public ConFunc(string conFuncName) {
            this.Name = conFuncName;
        }

        public abstract (ExecutionResult result, string message) Run(string consoleInput);
    }
}
