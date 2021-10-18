namespace Furball.Engine.Engine.DevConsole.ConFuncs.Client {
    /// <summary>
    /// cl_set_fps_unfocused_scale
    /// Sets the Unfocused FPS Multiplier for when the window is out of focus
    /// Example:
    /// If the FPS limiter is set to 1000fps, and the Unfocused multiplier is set to 0.25,
    /// the FPS when the window is unfocused is gonna be 250fps
    /// </summary>
    public class SetFpsUnfocusedScale : ConFunc {
        public SetFpsUnfocusedScale() : base("cl_set_fps_unfocused_scale") {}

        public override ConsoleResult Run(string consoleInput) {
            if (consoleInput.Trim().Length != 0) {
                //Done to prevent a stack overflow
                ConVars.TargetFpsUnfocusedScale.DisableOnChange = true;
                var result = ConVars.TargetFpsUnfocusedScale.Set(consoleInput);
                ConVars.TargetFpsUnfocusedScale.DisableOnChange = false;

                if (result.Result == ExecutionResult.Error)
                    return result;
            }

            double value = ConVars.TargetFpsUnfocusedScale.Value;

            FurballGame.Instance.SetTargetFps(ConVars.TargetFps.Value, value);

            return new ConsoleResult(ExecutionResult.Success, $"Set FPS Unfocused Scale to {value}");
        }
    }
}
