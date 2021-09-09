using Furball.Engine.Engine;
using Furball.Engine.Engine.Drawables;
using Furball.Engine.Engine.Drawables.Tweens;
using Furball.Engine.Engine.Drawables.Tweens.TweenTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        public override void Initialize() {
            TexturedDrawable whiteTexture = new TexturedDrawable(FurballTestGame.Instance.Content.Load<Texture2D>("white"), new Vector2(240, 240));
            whiteTexture.Tweens.Add(new VectorTween(TweenType.Movement, new Vector2(240, 240), new Vector2(540, 540), 1000,10000, Easing.In));
            this.Manager.Add(whiteTexture);

            base.Initialize();
        }
    }
}
