using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        private UiProgressBarDrawable   _progressBar;
        private CirclePrimitiveDrawable cursorTest = new CirclePrimitiveDrawable(new Vector2(0,0), 10f, 2f, Color.White);
        
        public override void Update(GameTime gameTime) {
            //this._progressBar.Progress = (gameTime.TotalGameTime.Milliseconds % 1000f) / 1000f;
            this.cursorTest.Position = FurballGame.InputManager.CursorStates[0].State.Position.ToVector2() / FurballGame.VerticalRatio;
            base.Update(gameTime);
        }
        
        public override void Initialize() {
            TexturedDrawable whiteTexture = new(ContentReader.LoadMonogameAsset<Texture2D>("white"), new Vector2(240, 240));

            whiteTexture.RotateRelative(1f, 5000, Easing.None);

            this.Manager.Add(this.cursorTest);
            this.Manager.Add(whiteTexture);

            base.Initialize();
        }
    }
}
