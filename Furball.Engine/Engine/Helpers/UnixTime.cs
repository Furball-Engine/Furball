using System;

namespace Furball.Engine.Engine.Helpers; 

public static class UnixTime {
    /// <summary>
    /// Easier Way to Get the Unix Time Quickly
    /// </summary>
    /// <returns>Unix time Right now</returns>
    public static long Now() {
        return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
    }
}