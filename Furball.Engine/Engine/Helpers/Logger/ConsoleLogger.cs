using System;

namespace Furball.Engine.Engine.Helpers.Logger {
    public class ConsoleLogger : LoggerBase {
        public override void Send(LoggerLine line) {
            System.Console.WriteLine(line.ToString());
        }
    }
}
