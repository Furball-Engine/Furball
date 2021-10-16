namespace Furball.Engine.Engine.Helpers.Logger {
    public class DevConsoleLogger : LoggerBase {
        public override void Send(LoggerLine line) {
            System.Console.WriteLine(line.ToString());
        }
    }
}
