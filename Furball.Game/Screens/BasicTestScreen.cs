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
            EventHandler<Point> onClick = (sender, point) => {
                AudioStream stream = AudioEngine.LoadFile("audio.mp3");
                stream.TempoFrequencyLock = false;
                stream.AudioRate          = 1.5f;
                stream.Volume             = 0.25f;
                stream.Play();
            };

            UiButtonDrawable screenSwitchButton = new UiButtonDrawable(new Vector2(40, 40), "Test Switching Screen", FurballGame.DEFAULT_FONT, 14f, Color.Cyan, Color.White, Color.Black, new Vector2(200, 40), onClick);

            this.Manager.Add(screenSwitchButton);
            
            base.Initialize();
        }
    }
}
