using Furball.Engine.Engine.Console.Types;

namespace Furball.Engine.Engine.Console.ConFuncs {
    public class SetScreenResolution : ConFunc {
        public SetScreenResolution() : base("set_screen_resolution") {

        }

        public override string Run(string consoleInput) {
            ConVars.ScreenResolution.Set(consoleInput);

            (int width, int height) value = ConVars.ScreenResolution.Value.Value;

            FurballGame.Instance.ChangeScreenSize(value.width, value.height);
            return $"Setting the screen resolution to {value.width}x{value.height}!";
        }
    }
}
