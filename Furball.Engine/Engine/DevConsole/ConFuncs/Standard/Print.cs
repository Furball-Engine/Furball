namespace Furball.Engine.Engine.DevConsole.ConFuncs.Standard {
    /// <summary>
    /// `print`
    /// Prints a line to console
    /// </summary>
    public class Print : ConFunc {
        public Print() : base("print") {}

        public override ConsoleResult Run(string consoleInput) {
            return new ConsoleResult(ExecutionResult.Success, consoleInput);
        }
    }
}
