using Furball.Engine;
using Furball.Engine.Engine.Localization;
using Furball.Game.Screens;

namespace Furball.Game {
    public class FurballTestGame : FurballGame {
        public FurballTestGame() : base(new ScreenSelector()) {
            //this.Window.AllowUserResizing = true;
        }

        protected override void InitializeLocalizations() {
            LocalizationManager.AddDefaultTranslation("cat", "Cat");
        }
    }
}
