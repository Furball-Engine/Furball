using Furball.Volpe.Evaluation;

namespace Furball.Engine.Engine.Config {
    public class FurballConfig : VolpeConfig {
        public override string Name => "furball";

        public FurballConfig() {
            this.Values["screen_width"] = new Value.Number(FurballGame.DEFAULT_WINDOW_WIDTH);
            this.Values["screen_height"] = new Value.Number(FurballGame.DEFAULT_WINDOW_HEIGHT);

            this.Values["limit_fps"]  = new Value.Boolean(true);
            this.Values["target_fps"] = new Value.Number(240);

            this.Values["fullscreen"] = new Value.Boolean(false);
        }

        public static FurballConfig Instance;
    }
}
