using System;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Helpers.Logger;
using Microsoft.Xna.Framework;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        private UiProgressBarDrawable   _progressBar;
        private CirclePrimitiveDrawable cursorTest = new(new Vector2(0,0), 10f, Color.White) {
            CoverClicks = false,
            CoverHovers = false
        };
        
        public override void Update(GameTime gameTime) {
            //this._progressBar.Progress = (gameTime.TotalGameTime.Milliseconds % 1000f) / 1000f;
            this.cursorTest.Position = FurballGame.InputManager.CursorStates[0].Position.ToVector2();
            base.Update(gameTime);
        }
        
        public override void Initialize() {
            //TexturedDrawable whiteTexture = new(ContentReader.LoadMonogameAsset<Texture2D>("white"), new Vector2(240, 240));
//
            //whiteTexture.RotateRelative(1f, 5000, Easing.None);

            this.Manager.Add(this.cursorTest);

            CirclePrimitiveDrawable    drawable  = new CirclePrimitiveDrawable(new Vector2(100,   100), 25f, Color.Red);
            CirclePrimitiveDrawable    drawable2 = new CirclePrimitiveDrawable(new Vector2(400,   400), 45f, Color.Green);
            LinePrimitiveDrawable      drawable3 = new LinePrimitiveDrawable(new Vector2(500,     100), 50, (float)(Math.PI * 1.5f));
            LinePrimitiveDrawable      drawable4 = new LinePrimitiveDrawable(new Vector2(600,     100), 50, (float)(Math.PI * 1.5f));
            RectanglePrimitiveDrawable drawable5 = new RectanglePrimitiveDrawable(new Vector2(10, 600), new Vector2(400, 50), 5f, true);
            RectanglePrimitiveDrawable drawable6 = new RectanglePrimitiveDrawable(new Vector2(300, 500), new Vector2(400, 50), 5f, false) {
                OriginType = OriginType.Center
            };

            this.Manager.Add(drawable);
            this.Manager.Add(drawable2);
            this.Manager.Add(drawable3);
            this.Manager.Add(drawable4);
            this.Manager.Add(drawable5);
            this.Manager.Add(drawable6);

            UiButtonDrawable testButton = new(new(500, 500), "Test button", FurballGame.DEFAULT_FONT, 30, new(100, 100, 100), Color.White, Color.White, new(200, 200)) {
                TextDrawable = {
                    OriginType = OriginType.RightCenter
                }
            };
            
            testButton.OnClick += delegate {
                Logger.Log($"pain peko {FurballGame.Time}");
            };

            this.Manager.Add(testButton);
            
            base.Initialize();
        }
    }
}
