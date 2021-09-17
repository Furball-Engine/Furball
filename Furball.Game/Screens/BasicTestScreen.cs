using System;
using System.IO;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Audio;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MathHelper=Furball.Engine.Engine.Helpers.MathHelper;

namespace Furball.Game.Screens {
    public class BasicTestScreen : Screen {
        public override void Initialize() {
            TexturedDrawable whiteTexture = new(FurballGame.Instance.Content.Load<Texture2D>("white"), new Vector2(240, 240));

            whiteTexture.Tweens.Add(new VectorTween(TweenType.Movement, new Vector2(240, 240), new Vector2(540,   540),   1000, 10000, Easing.In));
            whiteTexture.Tweens.Add(new VectorTween(TweenType.Scale,    new Vector2(1,   1),   new Vector2(0.25f, 0.25f), 1000, 10000, Easing.In));
            whiteTexture.Tweens.Add(new FloatTween(TweenType.Rotation, 0f, (float)MathHelper.DegreesToRadians(1440), 1000, 10000));
            whiteTexture.Tweens.Add(new FloatTween(TweenType.Fade,     1f, 0.5f,                                     1000, 10000));
            whiteTexture.Tweens.Add(new ColorTween(TweenType.Color, Color.White, Color.Red, 1000, 10000));

            this.Manager.Add(whiteTexture);

            //AudioStream stream = AudioEngine.LoadFile("testaudio.mp3");
            //stream.Play();
            //
            //whiteTexture.TimeSource = stream;


            base.Initialize();
        }
    }
}
