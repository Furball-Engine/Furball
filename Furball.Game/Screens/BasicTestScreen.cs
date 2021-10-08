using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Helpers.Logger;
using Microsoft.Xna.Framework;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        public override void Initialize() {
            base.Initialize();
            
            UiButtonDrawable screenSwitchButton = new (new Vector2(FurballGame.Random.Next(0, 1280), FurballGame.Random.Next(0, 720)), "Test Switching Screen", FurballGame.DEFAULT_FONT, 30, Color.Cyan, Color.Red, Color.Black, new Vector2(200, 40));

            screenSwitchButton.OnClick += delegate {
                Logger.Log($"button click event {FurballGame.Time}");
                ScreenManager.ChangeScreen(new BasicTestScreen());
            };

            screenSwitchButton.OnHover += delegate {
                Logger.Log($"button hover event {FurballGame.Time}");
            };
            
            this.Manager.Add(screenSwitchButton);
        }
    }
}
