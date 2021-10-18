namespace Furball.Engine.Engine.DevConsole.ConFuncs.Client {
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
