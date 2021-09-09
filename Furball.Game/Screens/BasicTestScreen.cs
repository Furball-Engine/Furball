using Furball.Engine.Engine;
using Furball.Engine.Engine.Drawables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        public override void Initialize() {
            TexturedDrawable whiteTexture = new TexturedDrawable(FurballTestGame.Instance.Content.Load<Texture2D>("white"), new Vector2(240, 240));

            this.Manager.Add(whiteTexture);

            base.Initialize();
        }
    }
}
