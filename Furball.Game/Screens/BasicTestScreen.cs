using System;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Microsoft.Xna.Framework;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        public override void Initialize() {
            TexturedDrawable background = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
                ColorOverride = Color.BlueViolet,
                Scale = new Vector2(1280, 720),
                Depth = 1f
            };
            this.Manager.Add(background);

            Random random = new Random();
            UiButtonDrawable screenSwitchButton = new UiButtonDrawable(new Vector2(random.Next(0, 1280), random.Next(0, 720)), "Test Switching Screen", FurballGame.DEFAULT_FONT, 30, Color.Cyan, Color.Red, Color.Black, new Vector2(200, 40));

            screenSwitchButton.OnClick += (sender, point) => {
                ScreenManager.ChangeScreen(new BasicTestScreen());
            };

            this.Manager.Add(screenSwitchButton);
            
            base.Initialize();
        }
    }
}
