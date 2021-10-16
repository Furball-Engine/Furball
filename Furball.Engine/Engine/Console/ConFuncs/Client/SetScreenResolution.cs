namespace Furball.Engine.Engine.Console.ConFuncs.Client {
    public class SetScreenResolution : ConFunc {
        public SetScreenResolution() : base("cl_set_screen_resolution") {}

        public override (ExecutionResult result, string message) Run(string consoleInput) {
            if (consoleInput.Trim().Length != 0)
                ConVars.ScreenResolution.Set(consoleInput);

            (int width, int height) = ConVars.ScreenResolution.Value.Value;

            FurballGame.Instance.ChangeScreenSize(width, height);

            return (ExecutionResult.Success, $"Resolution has been set to {width}x{height}");
        }
    }
}
