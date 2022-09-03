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

public class LoggerLevelEtoInfo : LoggerLevel {
    private LoggerLevelEtoInfo() {}

    public override string Name => "EtoInfo";

    public static LoggerLevelEtoInfo Instance = new();
}

public class LoggerLevelInput : LoggerLevel {
    private LoggerLevelInput(Channel channel) {
        base.Channel = channel.ToString();
    }

    private new enum Channel {
        Info,
        Warning,
        Error
    }
    
    public override string Name => "Input";

    public static LoggerLevelInput InstanceInfo = new(Channel.Info);
    public static LoggerLevelInput InstanceWarning = new(Channel.Warning);
    public static LoggerLevelInput InstanceError = new(Channel.Error);
}