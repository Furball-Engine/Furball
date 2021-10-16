using Furball.Engine.Engine.DevConsole;

namespace Furball.Engine.Engine.Helpers.Logger {
    public class DevConsoleLogger : LoggerBase {
        public override void Send(LoggerLine line) {
            DevConsole.DevConsole.ConsoleLog.Add((string.Empty, ExecutionResult.Log, line.LineData));
        }
    }
}
