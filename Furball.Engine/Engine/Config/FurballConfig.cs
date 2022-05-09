using Furball.Engine.Engine.Localization;
using Furball.Volpe.Evaluation;

namespace Furball.Engine.Engine.Config {
    public class FurballConfig : VolpeConfig {
        public override string Name => "furball";

        public int ScreenWidth  => (int)this.Values["screen_width"].ToNumber().Value;
        public int ScreenHeight => (int)this.Values["screen_height"].ToNumber().Value;


        public bool LimitFPS  => this.Values["limit_fps"].ToBoolean().Value;
        public int  TargetFPS => (int)this.Values["target_fps"].ToNumber().Value;

        public bool Fullscreen => this.Values["fullscreen"].ToBoolean().Value;
        public string Language => this.Values["language"].ToStringValue().Value;
        
        public FurballConfig() {
            this.Values["screen_width"]  = new Value.Number(FurballGame.DEFAULT_WINDOW_WIDTH);
            this.Values["screen_height"] = new Value.Number(FurballGame.DEFAULT_WINDOW_HEIGHT);

            this.Values["limit_fps"]  = new Value.Boolean(true);
            this.Values["target_fps"] = new Value.Number(240);

            this.Values["fullscreen"] = new Value.Boolean(false);

            this.Values["language"] = new Value.String(LocalizationManager.DefaultLanguage.Iso6392Code().ToString());
        }

        public static FurballConfig Instance;
    }
}
