using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;

namespace Furball.Game.Screens {
    public class ScrollingTest : Screen {
        public override void Initialize() {
            base.Initialize();

            TexturedDrawable drawable = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
                Scale = new Vector2(50, 50)
            };
            drawable.MoveTo(new Vector2(1280, 720), 20000);

            this.Manager.Add(drawable);
        }
    }
}
