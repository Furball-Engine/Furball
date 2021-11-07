using Furball.Engine;
using Furball.Engine.Engine.Localization;
using Furball.Game.Screens;
using Silk.NET.Windowing;

namespace Furball.Game {
    public class FurballTestGame : FurballGame {
        public FurballTestGame() : base(new ScreenSelector(), WindowOptions.Default) {
            //this.Window.AllowUserResizing = true;
        }

        public override void InitializeLocalizations() {
            LocalizationManager.AddDefaultTranslation("cat", "Cat");
        }
    }
}
