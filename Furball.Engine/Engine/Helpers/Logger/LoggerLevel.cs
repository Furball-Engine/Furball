namespace Furball.Engine.Engine.Helpers.Logger {
    public class LoggerLevel {
        public virtual string Name => null;

        public static LoggerLevel All = LoggerLevelAll.Instance;
        
        public override string ToString() {
            return this.Name ?? this.GetType().Name;
        }
    }

    public class LoggerLevelUnknown : LoggerLevel {
        public override string Name => "Unknown";
        public static LoggerLevelUnknown Instance = new();
    }
    
    public class LoggerLevelCacheEvent : LoggerLevel {
        public override string Name => "CacheEvent";
        public static LoggerLevelCacheEvent Instance = new();
    }
    
    public class LoggerLevelLoggerInfo : LoggerLevel {
        public override string Name => "LoggerInfo";
        public static LoggerLevelLoggerInfo Instance = new();
    }
    
    public class LoggerLevelEngineInfo : LoggerLevel {
        public override string Name => "EngineInfo";
        public static LoggerLevelEngineInfo Instance = new();
    }

    public class LoggerLevelLocalizationInfo : LoggerLevel {
        public override string Name => "LocalizationInfo";
        public static LoggerLevelLocalizationInfo Instance = new();
    }

    public class LoggerLevelSchedulerInfo : LoggerLevel {
        public override string Name => "SchedulerInfo";
        public static LoggerLevelSchedulerInfo Instance = new();
    }

    public class LoggerLevelAll : LoggerLevel {
        public static LoggerLevelAll Instance = new();
    }
}
