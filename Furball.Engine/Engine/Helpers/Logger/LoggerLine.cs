namespace Furball.Engine.Engine.Helpers.Logger {
    public struct LoggerLine {
        public string      LineData;
        public LoggerLevel LoggerLevel;

        public override string ToString() {
            return $"{this.LoggerLevel.ToString()}: {this.LineData}";
        }
    }
}
