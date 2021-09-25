using Furball.Engine;
using Furball.Game.Screens;

namespace Furball.Game {
    public class FurballTestGame : FurballGame {
        public FurballTestGame() : base(new BasicTestScreen()) {
            this.Window.AllowUserResizing = true;
        }

    }
}
