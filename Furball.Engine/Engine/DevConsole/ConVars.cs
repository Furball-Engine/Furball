using Furball.Volpe.Evaluation;

namespace Furball.Engine.Engine.DevConsole; 

public class ConVars {
    public static bool DebugOverlay {
        get {
            if (!DevConsole.VolpeEnvironment.TryGetVariable("cl_debug_overlay", out IVariable value))
                return false;

            Value.Boolean asBool = value!.ToVariable().RawValue as Value.Boolean;

            return asBool != null && asBool.Value;
        }
        set {
            if (!DevConsole.VolpeEnvironment.TryGetVariable("cl_debug_overlay", out IVariable variable))
                return;

            variable!.ToTypedVariable<Value.Boolean>().Value = new Value.Boolean(value);
        }
    }

    /// <summary>
    /// Whether or not the Console should be logged and saved upon closing the game
    /// </summary>
    public static bool WriteLog {
        get {
            if (!DevConsole.VolpeEnvironment.TryGetVariable("cl_console_log", out IVariable value))
                return false;

            Value.Boolean asBool = value!.ToTypedVariable<Value.Boolean>().Value;

            return asBool.Value;
        }
        set {
            if (!DevConsole.VolpeEnvironment.TryGetVariable("cl_console_log", out IVariable variable))
                return;

            variable!.ToTypedVariable<Value.Boolean>().Value = new Value.Boolean(value);
        }
    }

    /// <summary>
    ///     Whether or not to enable tool tips when hovering over things
    /// </summary>
    /// 
    public static bool ToolTips {
        get {
            if (!DevConsole.VolpeEnvironment.TryGetVariable("cl_tooltipping", out IVariable value))
                return false;

            Value.Boolean asBool = value!.ToTypedVariable<Value.Boolean>().Value;

            return asBool.Value;
        }
        set {
            if (!DevConsole.VolpeEnvironment.TryGetVariable("cl_tooltipping", out IVariable variable))
                return;

            variable!.ToTypedVariable<Value.Boolean>().Value = new Value.Boolean(value);
        }
    }
}