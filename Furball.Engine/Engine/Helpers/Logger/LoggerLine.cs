using System;

namespace Furball.Engine.Engine.Helpers.Logger {
    public struct LoggerLine {
        public string      LineData;
        public LoggerLevel Level;

        public string ToString(LoggerLevel loggerLevel) {
            string loggerLevels = string.Empty;

            Array enumValues = Enum.GetValues(typeof(LoggerLevel));
            foreach (LoggerLevel level in enumValues) {
                if ((this.Level & level) == 0)
                    continue;
                
                loggerLevels += level.ToString();
            }
            
            return $"{this.Level}: {this.LineData}";
        }
    }
}
