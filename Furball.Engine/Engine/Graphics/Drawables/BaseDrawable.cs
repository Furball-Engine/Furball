using System.Collections.Generic;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Engine.Engine.Timing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics.Drawables {
    public enum OriginType {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }

    public abstract class BaseDrawable {
        /// <summary>
        /// Unprocessed Position where the Drawable is expected to be drawn
        /// <remarks>This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be properly visible on all resolutions</remarks>
        /// </summary>
        public Vector2 Position = Vector2.Zero;
        /// <summary>
        /// Unprocessed Size of the Drawable in Pixels
        /// <remarks>This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be properly visible on all resolutions</remarks>
        /// </summary>
        public Vector2 Size = Vector2.Zero;
        /// <summary>
        /// Unprocessed Color Override of the Drawable, if a White Texture gets drawn with a red override, voila its red
        /// <remarks>This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be properly visible on all resolutions</remarks>
        /// </summary>
        public Color ColorOverride = Color.White;
        /// <summary>
        /// Unprocessed Rotation of the Drawable in Radians
        /// <remarks>This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be properly visible on all resolutions</remarks>
        /// </summary>
        public float Rotation = 0f;
        /// <summary>
        /// Unprocessed Scale of the Drawable, new Vector(1, 1) draws the Drawable at full scale
        /// <remarks>This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be properly visible on all resolutions</remarks>
        /// </summary>
        public Vector2 Scale = Vector2.One;
        /// <summary>
        /// Basic SpriteEffect, was provided by SpriteBatch so might aswell put it here
        /// </summary>
        public SpriteEffects SpriteEffect = SpriteEffects.None;
        /// <summary>
        /// What time does the Drawable go by? Used for Tweens
        /// </summary>
        public ITimeSource TimeSource = FurballGame.GameTimeSource;
        /// <summary>
        /// The draw depth of the Drawable
        /// </summary>
        public float Depth;
        /// <summary>
        /// List of Tweens
        /// </summary>
        public List<Tween> Tweens = new();
        /// <summary>
        /// The position of the Origin to render at
        /// </summary>
        public OriginType OriginType = OriginType.TopLeft;
        /// <summary>
        /// Whether to scale and move the drawable when the resolution changes
        /// </summary>
        public bool ResolutionScale = true;

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
                            this.ColorOverride.A = (byte)(255 * fadeTween.GetCurrent());
                        break;
                }
            }
        }
    }
}
