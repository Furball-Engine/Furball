
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Transitions; 

/// <summary>
/// Nice smooth Fade In and Out Transition
/// </summary>
public class FadeTransition : Transition {
    private TexturedDrawable _fadeScreen;

    public FadeTransition() {
        this._fadeScreen = new TexturedDrawable(FurballGame.WhitePixel, new Vector2(0, 0)) {
            Scale         = new Vector2(1280, 720),
            ColorOverride = Color.Transparent,
            Clickable     = false,
            CoverClicks   = false,
            Hoverable     = false,
            CoverHovers   = false
        };

        this.Manager.Add(this._fadeScreen);
            
        FurballGame.Instance.OnRelayout += this.OnRelayout;
    }
        
    private void OnRelayout(object sender, Vector2 e) {
        this._fadeScreen.Scale = e;
    }

    public override void Dispose() {
        base.Dispose();
            
        FurballGame.Instance.OnRelayout += this.OnRelayout;
    }

    public override double TransitionBegin() {
        double currentTime = this._fadeScreen.DrawableTime;

        ColorTween fadeInTween  = new(TweenType.Color, Color.Transparent, Color.Black,       currentTime,       currentTime + 500);
        ColorTween fadeOutTween = new(TweenType.Color, Color.Black, Color.Transparent, currentTime + 600, currentTime + 1100);

        this._fadeScreen.Tweens.Add(fadeInTween);
        this._fadeScreen.Tweens.Add(fadeOutTween);

        return 500.0;
    }

    public override double TransitionEnd() => 500.0;
}