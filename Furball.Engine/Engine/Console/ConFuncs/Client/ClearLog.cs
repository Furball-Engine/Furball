namespace Furball.Engine.Engine.Console.ConFuncs.Client {
    public class ClearLog : ConFunc {
        public ClearLog() : base("cl_clear_log") {}

        public override (ExecutionResult result, string message) Run(string consoleInput) {
            if (DevConsole.ConsoleLog.Count == 0)
                return (ExecutionResult.Warning, "Console log is already empty!");

            DevConsole.ConsoleLog.Clear();

            return (ExecutionResult.Success, "Console log cleared");
        }
    }
}
