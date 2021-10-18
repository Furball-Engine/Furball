namespace Furball.Engine.Engine.DevConsole.ConFuncs.Client {
    /// <summary>
    /// cl_flush_log
    /// Forces the log to be written
    /// </summary>
    public class FlushLog : ConFunc {
        public FlushLog() : base("cl_flush_log") {}

        public override ConsoleResult Run(string consoleInput) {
            if (DevConsole.ConsoleLog.Count == 0)
                return new ConsoleResult(ExecutionResult.Warning, "No log to flush!");

            var result = DevConsole.WriteLog();

            return result;
        }
    }
}
