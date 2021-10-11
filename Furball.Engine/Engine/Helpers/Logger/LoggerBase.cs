using System;
using System.Collections.Generic;

namespace Furball.Engine.Engine.Helpers.Logger {
    public abstract class LoggerBase : IDisposable {
        public LoggerBase(List<LoggerLevel> level = null) {
            level ??= new() {
                LoggerLevel.All
            };
            
            this.Level = level;
        }
        
        public abstract void Send(LoggerLine line);

        public virtual void Initialize() {
            Logger.Log(
            new LoggerLine {
                LoggerLevel = LoggerLevelLoggerInfo.Instance,
                LineData    = $"{this.GetType().Name} initialized!"
            }
            );
        }
        
        public List<LoggerLevel> Level;
        public void              Dispose() {}
    }
}
