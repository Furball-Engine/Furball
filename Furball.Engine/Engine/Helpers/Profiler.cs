using System.Collections.Generic;
using System.Diagnostics;
using Kettu;

namespace Furball.Engine.Engine.Helpers; 

public static class Profiler {
    private static readonly Dictionary<string, double> _Profiles = new();

    [Conditional("DEBUG")]
    public static void StartProfile(string name) {
        _Profiles[name] = (double)Stopwatch.GetTimestamp() / Stopwatch.Frequency;
    }

    [Conditional("DEBUG")]
    public static void EndProfileAndPrint(string name) {
        double time = EndProfile(name);

        Kettu.Logger.Log($"Profile {name} took {time}ms!", LoggerLevelProfiler.Instance);
    }

    public static double EndProfile(string name) => ((double)Stopwatch.GetTimestamp() / Stopwatch.Frequency - _Profiles[name]) * 1000d;
}

internal class LoggerLevelProfiler : LoggerLevel {
    public override string Name => "Profiler";

    private LoggerLevelProfiler() {}

    public static LoggerLevel Instance = new LoggerLevelProfiler();
}