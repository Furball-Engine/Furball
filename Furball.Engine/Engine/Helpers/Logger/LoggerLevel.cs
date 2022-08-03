using Kettu;

namespace Furball.Engine.Engine.Helpers.Logger; 

public class LoggerLevelCacheEvent : LoggerLevel {
    public override string Name => "CacheEvent";
    public static LoggerLevelCacheEvent Instance = new();
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

public class LoggerLevelFurballFormInfo : LoggerLevel {
    private LoggerLevelFurballFormInfo() {}

    public override string Name => "FurballFormInfo";

    public static LoggerLevelFurballFormInfo Instance = new();
}