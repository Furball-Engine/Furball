using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Microsoft.Xna.Framework;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        private UiProgressBarDrawable   _progressBar;
        private CirclePrimitiveDrawable cursorTest = new CirclePrimitiveDrawable(new Vector2(0,0), 10f, 2f, Color.White);
        
        public override void Update(GameTime gameTime) {
            //this._progressBar.Progress = (gameTime.TotalGameTime.Milliseconds % 1000f) / 1000f;
            this.cursorTest.Position = FurballGame.InputManager.CursorStates[0].Position.ToVector2() / FurballGame.VerticalRatio;
            base.Update(gameTime);
        }
        
        public override void Initialize() {
            //TexturedDrawable whiteTexture = new(ContentReader.LoadMonogameAsset<Texture2D>("white"), new Vector2(240, 240));
//
            //whiteTexture.RotateRelative(1f, 5000, Easing.None);

            this.Manager.Add(this.cursorTest);

            CirclePrimitiveDrawable drawable = new CirclePrimitiveDrawable(new Vector2(100,       100), 25f, 6f,  Color.Red);
            CirclePrimitiveDrawable drawable2 = new CirclePrimitiveDrawable(new Vector2(400,      400), 45f, 10f, Color.Green);
            LinePrimitiveDrawable drawable3 = new LinePrimitiveDrawable(new Vector2(500,          100), new(550, 50), 10f);
            LinePrimitiveDrawable drawable4 = new LinePrimitiveDrawable(new Vector2(600,          100), new(650, 50), 10f);
            RectanglePrimitiveDrawable drawable5 = new RectanglePrimitiveDrawable(new Vector2(10, 600), new Vector2(400, 50), 5f, true);
            RectanglePrimitiveDrawable drawable6 = new RectanglePrimitiveDrawable(new Vector2(10, 500), new Vector2(400, 50), 5f, false);

            this.Manager.Add(drawable);
            this.Manager.Add(drawable2);
            this.Manager.Add(drawable3);
            this.Manager.Add(drawable4);
            this.Manager.Add(drawable5);
            this.Manager.Add(drawable6);

            UiTextBoxDrawable textBox = new(FurballGame.DEFAULT_FONT, "input pog?", 30, 200) {
                Position = new(500, 500)
            };

            this.Manager.Add(textBox);
            
            base.Initialize();
        }
    }
}
