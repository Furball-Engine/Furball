namespace Furball.Engine.Engine.Console.ConFuncs.Client {
    public class FlushLog : ConFunc {
        public FlushLog() : base("cl_flush_log") {}

        public override (ExecutionResult result, string message) Run(string consoleInput) {
            if (Console.ConsoleLog.Count == 0)
                return (ExecutionResult.Warning, "No log to flush!");

            var result = Console.WriteLog();

            return result;
        }
    }
}
