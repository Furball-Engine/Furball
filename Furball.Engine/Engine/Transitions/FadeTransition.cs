using System.Drawing;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;


namespace Furball.Engine.Engine.Transitions {
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
                CoverClicks   = false
            };

            this.Manager.Add(this._fadeScreen);
        }

        public override int TransitionBegin() {
            int currentTime = this._fadeScreen.DrawableTime;

            ColorTween fadeInTween = new ColorTween(TweenType.Color,  Color.Transparent, Color.Black,       currentTime,       currentTime + 500);
            ColorTween fadeOutTween = new ColorTween(TweenType.Color, Color.Black,       Color.Transparent, currentTime + 600, currentTime + 1100);

            this._fadeScreen.Tweens.Add(fadeInTween);
            this._fadeScreen.Tweens.Add(fadeOutTween);

            return 500;
        }

        public override int TransitionEnd() => 500;
    }
}
