using Furball.Engine;
using Furball.Engine.Engine.Console;
using Furball.Engine.Engine.Localization;
using Furball.Game.Screens;

namespace Furball.Game {
    public class FurballTestGame : FurballGame {
        public FurballTestGame() : base(new BasicTestScreen()) {
            this.Window.AllowUserResizing = true;
        }

        protected override void Initialize() {
            Console.AddConVarStore(typeof(ConVars));

            base.Initialize();
        }

        public override void InitializeLocalizations() {
            LocalizationManager.AddDefaultTranslation("cat", "Cat");
        }
    }
}
