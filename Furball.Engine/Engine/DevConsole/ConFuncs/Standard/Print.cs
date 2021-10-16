namespace Furball.Engine.Engine.DevConsole.ConFuncs.Standard {
    /// <summary>
    /// `print`
    /// Prints a line to console
    /// </summary>
    public class Print : ConFunc {
        public Print() : base("print") {}

        public override (ExecutionResult result, string message) Run(string consoleInput) {
            return (ExecutionResult.Success, consoleInput);
        }
    }
}
