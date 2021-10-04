using System;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Audio;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Helpers.Logger;
using Microsoft.Xna.Framework;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        public override void Initialize() {
            Random random = new Random();
            UiButtonDrawable screenSwitchButton = new UiButtonDrawable(new Vector2(random.Next(0, 1280), random.Next(0, 720)), "Test Switching Screen", FurballGame.DEFAULT_FONT, 14f, Color.Cyan, Color.White, Color.Black, new Vector2(200, 40));

            screenSwitchButton.OnClick += (sender, point) => {
                ScreenManager.ChangeScreen(new BasicTestScreen());
            };

            this.Manager.Add(screenSwitchButton);
            
            base.Initialize();
        }
    }
}
