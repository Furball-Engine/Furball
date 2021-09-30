using System;

namespace Furball.Engine.Engine.Helpers.Logger {
    public abstract class LoggerBase : IDisposable {
        public LoggerBase(LoggerLevel level = (LoggerLevel)long.MaxValue) {
            this.Level = level;
        }
        
        public abstract void Send(LoggerLine line);

        public virtual void Initialize() {
            Logger.Log(new LoggerLine{Level = LoggerLevel.LoggerInfo, LineData = $"{ this.GetType().Name } initialized!"});
        }
        
        public LoggerLevel Level;
        public void        Dispose() {}
    }
}
