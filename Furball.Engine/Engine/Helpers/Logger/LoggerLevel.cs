namespace Furball.Engine.Engine.Helpers.Logger {
    public enum LoggerLevel : long {
        Unknown = 1 << 0,
        CacheEvent = 1 << 1,
        LoggerInfo = 1 << 2,
        EngineInfo = 1 << 3,
    }
}
