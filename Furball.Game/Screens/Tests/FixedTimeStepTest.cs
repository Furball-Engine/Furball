using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Vixie.Backends.Shared;

namespace Furball.Game.Screens.Tests; 

public class FixedTimeStepTest : TestScreen {
    private TexturedDrawable    _1PerSecond;
    private TexturedDrawable    _2PerSecond;
    private TexturedDrawable    _3PerSecond;
    private FixedTimeStepMethod _1PerSecondMethod;
    private FixedTimeStepMethod _2PerSecondMethod;
    private FixedTimeStepMethod _3PerSecondMethod;

    public override void Initialize() {
        base.Initialize();
            
        const float x    = 15; 
        float       y    = 15;
        const float size = 50;

        this.Manager.Add(
        this._1PerSecond = new TexturedDrawable(FurballGame.WhitePixel, new(x, y)) {
            Scale = new(size)
        });
        this.Manager.Add(new TextDrawable(new(x + this._1PerSecond.Size.X + 15, y + this._1PerSecond.Size.Y / 2f), FurballGame.DefaultFont, "Once per second", 24) {
            OriginType = OriginType.LeftCenter
        });

        y += this._1PerSecond.Size.Y + 15;

        this.Manager.Add(
        this._2PerSecond = new TexturedDrawable(FurballGame.WhitePixel, new(x, y)) {
            Scale = new(size)
        });
        this.Manager.Add(new TextDrawable(new(x + this._2PerSecond.Size.X + 15, y + this._2PerSecond.Size.Y / 2f), FurballGame.DefaultFont, "Twice per second", 24) {
            OriginType = OriginType.LeftCenter
        });
            
        y += this._2PerSecond.Size.Y + 15;

        this.Manager.Add(
        this._3PerSecond = new TexturedDrawable(FurballGame.WhitePixel, new(x, y)) {
            Scale = new(size)
        });
        this.Manager.Add(new TextDrawable(new(x + this._3PerSecond.Size.X + 15, y + this._3PerSecond.Size.Y / 2f), FurballGame.DefaultFont, "Thrice per second", 24) {
            OriginType = OriginType.LeftCenter
        });
            
        const float tweenTime = 300;
            
        FurballGame.TimeStepMethods.Add(
        this._1PerSecondMethod = new FixedTimeStepMethod(
        1000,
        () => {
            this._1PerSecond.Tweens.Add(new ColorTween(TweenType.Color, Color.Blue, Color.White, FurballGame.Time, FurballGame.Time + tweenTime));
        }
        )
        );
            
        FurballGame.TimeStepMethods.Add(
        this._2PerSecondMethod = new FixedTimeStepMethod(
        500,
        () => {
            this._2PerSecond.Tweens.Add(new ColorTween(TweenType.Color, Color.Blue, Color.White, FurballGame.Time, FurballGame.Time + tweenTime));
        }
        )
        );
            
        FurballGame.TimeStepMethods.Add(
        this._3PerSecondMethod = new FixedTimeStepMethod(
        1000d / 3d,
        () => {
            this._3PerSecond.Tweens.Add(new ColorTween(TweenType.Color, Color.Blue, Color.White, FurballGame.Time, FurballGame.Time + tweenTime));
        }
        )
        );
    }

    public override void Dispose() {
        base.Dispose();

        FurballGame.TimeStepMethods.Remove(this._1PerSecondMethod);
        FurballGame.TimeStepMethods.Remove(this._2PerSecondMethod);
        FurballGame.TimeStepMethods.Remove(this._3PerSecondMethod);
    }
}