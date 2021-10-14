namespace Furball.Engine.Engine.Console.ConFuncs {
    public class SetScreenResolution : ConFunc {
        public SetScreenResolution() : base("cl_set_screen_resolution") {

        }

        public override string Run(string consoleInput) {
            if (consoleInput.Trim().Length != 0)
                ConVars.ScreenResolution.Set(consoleInput);

            (int width, int height) = ConVars.ScreenResolution.Value.Value;

            FurballGame.Instance.ChangeScreenSize(width, height);

            return $"Setting the screen resolution to {width}x{height}!";
        }
    }
}
