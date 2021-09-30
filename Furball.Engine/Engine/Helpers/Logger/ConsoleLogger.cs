using System;

namespace Furball.Engine.Engine.Helpers.Logger {
    public class ConsoleLogger : ILogger {
        public override void Send(LoggerLine line) {
            Console.WriteLine(line.ToString(this.Level));
        }
    }
}
