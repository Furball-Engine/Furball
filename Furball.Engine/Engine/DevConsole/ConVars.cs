using Furball.Volpe.Evaluation;

namespace Furball.Engine.Engine.DevConsole; 

public class ConVars {
    public static bool DebugOverlay {
        get {
            DevConsole.VolpeEnvironment.TryGetVariableValue("cl_debug_overlay", out Value? value);

            Value.Boolean? asBool = value as Value.Boolean;

            return asBool != null && asBool.Value;
        }
        set => DevConsole.VolpeEnvironment.SetVariableValue("cl_debug_overlay", new Value.Boolean(value));
    }

    /// <summary>
    /// Whether or not the Console should be logged and saved upon closing the game
    /// </summary>
    public static bool WriteLog {
        get {
            DevConsole.VolpeEnvironment.TryGetVariableValue("cl_console_log", out Value? value);

            Value.Boolean? asBool = value as Value.Boolean;

            return asBool != null && asBool.Value;
        }
        set => DevConsole.VolpeEnvironment.SetVariableValue("cl_console_log", new Value.Boolean(value));
    }

    /// <summary>
    ///     Whether or not to enable tool tips when hovering over things
    /// </summary>
    /// 
    public static bool ToolTips {
        get {
            DevConsole.VolpeEnvironment.TryGetVariableValue("cl_tooltipping", out Value? value);

            Value.Boolean? asBool = value as Value.Boolean;

            return asBool != null && asBool.Value;
        }
        set => DevConsole.VolpeEnvironment.SetVariableValue("cl_tooltipping", new Value.Boolean(value));
    }
}