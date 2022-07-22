using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Engine.Engine.Timing;

namespace Furball.Game.Screens; 

public class ScrollingTest : TestScreen {
    public override void Initialize() {
        base.Initialize();

        LoopingTimeSource source = new(FurballGame.GameTimeSource, 10000);
        TexturedDrawable drawable = new(FurballGame.WhitePixel, Vector2.Zero) {
            Scale      = new Vector2(50, 50),
            TimeSource = source
        };
        drawable.Tweens.Add(new VectorTween(TweenType.Movement, Vector2.Zero, new(1280, 720), 0, 10000) {
            KeepAlive = true
        });

        this.Manager.Add(drawable);
    }
}