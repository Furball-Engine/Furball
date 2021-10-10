namespace Furball.Engine.Engine.Helpers.Logger {
    public class LoggerLevel {
        public virtual string Name => null;

        public static LoggerLevel All = new LoggerLevelAll();

        public override string ToString() {
            return this.Name ?? this.GetType().Name;
        }
    }

    public class LoggerLevelUnknown : LoggerLevel {
        public override string Name => "Unknown";
    }
    
    public class LoggerLevelCacheEvent : LoggerLevel {
        public override string Name => "CacheEvent";
    }
    
    public class LoggerLevelLoggerInfo : LoggerLevel {
        public override string Name => "LoggerInfo";
    }
    
    public class LoggerLevelEngineInfo : LoggerLevel {
        public override string Name => "EngineInfo";
    }

    public class LoggerLevelLocalizationInfo : LoggerLevel {
        public override string Name => "LocalizationInfo";
    }

    public class LoggerLevelSchedulerInfo : LoggerLevel {
        public override string Name => "SchedulerInfo";
    }

    public class LoggerLevelAll : LoggerLevel {
        
    }
}
