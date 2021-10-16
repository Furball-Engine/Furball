using System;

namespace Furball.Engine.Engine.DevConsole.ConFuncs.Client {
    /// <summary>
    /// cl_set_target_fps
    /// Sets the Target Frame Rate
    /// Syntax: `cl_set_target_fps target_frame_rate`
    /// <remarks>if called without parameters, it will set the frame rate to whatever `cl_target_fps` is</remarks>
    /// <remarks>if called with the value of -1, it sets it to unlimited</remarks>
    /// </summary>
    public class SetTargetFps : ConFunc {

        public SetTargetFps() : base("cl_set_target_fps") {}
        public override (ExecutionResult result, string message) Run(string consoleInput) {
            if (consoleInput.Trim().Length != 0) {
                //Done to prevent a stack overflow
                ConVars.TargetFps.DisableOnChange = true;
                var result = ConVars.TargetFps.Set(consoleInput);
                ConVars.TargetFps.DisableOnChange = false;

                if (result.result == ExecutionResult.Error)
                    return result;
            }

            int value = ConVars.TargetFps.Value.Value;

            FurballGame.Instance.SetTargetFps(value);

            string fps = value == -1 ? "Unlimited" : value.ToString();

            return (ExecutionResult.Success, $"Set Target FPS to {fps}");
        }
    }
}
