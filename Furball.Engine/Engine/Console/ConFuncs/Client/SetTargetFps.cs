using System;

namespace Furball.Engine.Engine.Console.ConFuncs.Client {
    public class SetTargetFps : ConFunc {

        public SetTargetFps() : base("cl_set_target_fps") {}
        public override (ExecutionResult result, string message) Run(string consoleInput) {
            if (consoleInput.Trim().Length != 0) {
                ConVars.TargetFps.DisableOnChange = true;
                var result = ConVars.TargetFps.Set(consoleInput);
                ConVars.TargetFps.DisableOnChange = false;

                if (result.result == ExecutionResult.Error)
                    return result;
            }

            int value = ConVars.TargetFps.Value.Value;

            if (value != -1) {
                FurballGame.Instance.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / (double) value);
                FurballGame.Instance.IsFixedTimeStep   = true;
            } else {
                FurballGame.Instance.TargetElapsedTime = TimeSpan.FromTicks(1);
                FurballGame.Instance.IsFixedTimeStep   = false;
            }

            string fps = value == -1 ? "Unlimited" : value.ToString();

            return (ExecutionResult.Success, $"Set Target FPS to {fps}");
        }
    }
}
