using System.Collections.Generic;
using Furball.Engine.Engine.Drawables.Tweens;
using Furball.Engine.Engine.Drawables.Tweens.TweenTypes;
using Furball.Engine.Engine.Timing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Drawables {
    public abstract class BaseDrawable {
        public Vector2       Position;
        public Vector2       Size;
        public Color         ColorOverride;
        public float         Rotation;
        public Vector2       Scale;
        public SpriteEffects SpriteEffect;
        public ITimeSource   TimeSource = FurballGame.GameTimeSource;

        public List<Tween> Tweens = new();

        /// <summary>
        /// Updates the pDrawables Tweens
        /// </summary>
        public void UpdateTweens() {
            for (int i = 0; i != this.Tweens.Count; i++) {
                Tween currentTween = this.Tweens[i];

                currentTween.Update(this.TimeSource.GetCurrentTime());

                switch (currentTween.TweenType) {
                    case TweenType.Color:
                        ColorTween colorTween = currentTween as ColorTween;

                        if (colorTween != null)
                            this.ColorOverride = colorTween.GetCurrent();
                        break;
                    case TweenType.Movement:
                        VectorTween vectorTween = currentTween as VectorTween;

                        if (vectorTween != null)
                            this.Position = vectorTween.GetCurrent();
                        break;
                    case TweenType.Scale:
                        VectorTween scaleTween = currentTween as VectorTween;

                        if (scaleTween != null)
                            this.Scale = scaleTween.GetCurrent();
                        break;
                    case TweenType.Rotation:
                        FloatTween rotationTween = currentTween as FloatTween;

                        if (rotationTween != null)
                            this.Rotation = rotationTween.GetCurrent();
                        break;
                    case TweenType.Fade:
                        FloatTween fadeTween = currentTween as FloatTween;

                        if (fadeTween != null)
                            this.ColorOverride.A = (byte) (255 * fadeTween.GetCurrent());
                        break;
                }
            }
        }
    }
}
