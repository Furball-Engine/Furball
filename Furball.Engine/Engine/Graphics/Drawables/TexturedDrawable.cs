using System;
using System.Drawing;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Vixie;
using JetBrains.Annotations;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables;

/// <summary>
/// A Basic Managed Drawable that just Draws a Texture to the Screen,
/// </summary>
public class TexturedDrawable : Drawable {
    /// <summary>
    /// The Texture Being drawn
    /// </summary>
    public Texture Texture;
    /// <summary>
    /// Crop Rectangle, this basically tells which part of the Texture to Render
    /// Leave null to draw the entire Texture
    /// </summary>
    public Rectangle? Cropping = null;
    /// <summary>
    /// Unprocessed Size of the Drawable in Pixels
    /// <remarks>This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be properly visible on all resolutions</remarks>
    /// </summary>
    public override Vector2 Size => this.Cropping == null
                                        ? new Vector2(this.Texture.Size.X,       this.Texture.Size.Y) * this.Scale
                                        : new Vector2(this.Cropping.Value.Width, this.Cropping.Value.Height) * this.Scale;

    private FloatTween _fadeTextureTween = null;
    private Texture    _fadeTextureTo;
    public void FadeTexture(Texture tex, double time, Easing easing = Easing.None) {
        if (tex.Size != this.Texture.Size)
            throw new Exception("Sizes of the fading textures must match!");

        this._fadeTextureTween = new FloatTween(TweenType.Fade, 0, 1, FurballGame.Time, FurballGame.Time + time, easing);
        this._fadeTextureTo    = tex;
    }

    /// <summary>
    /// TexturedDrawable Constructor
    /// </summary>
    /// <param name="texture">Texture to Draw</param>
    /// <param name="position">Where to Draw</param>
    public TexturedDrawable(Texture texture, Vector2 position) {
        this.Position = position;

        this.Texture = texture;
    }

    /// <summary>
    /// TexturedDrawable Constructor that allows for Cropping
    /// </summary>
    /// <param name="texture">Texture to Draw</param>
    /// <param name="position">Where to Draw</param>
    /// <param name="cropping">What Part to Draw</param>
    /// <param name="rotation">Rotation in Radians</param>
    public TexturedDrawable(Texture texture, Vector2 position, Rectangle cropping, float rotation = 0f) {
        this.Position = position;
        this.Rotation = rotation;

        this.Cropping = cropping;
        this.Texture  = texture;
    }

    public override void Update(double time) {
        base.Update(time);

        //Update the tween with the current time of the drawable
        this._fadeTextureTween?.Update(this.DrawableTime);

        //If the tween is terminated, then we are fully faded to the new texture
        if (this._fadeTextureTween is {
                Terminated: true
            }) {
            this.Texture           = this._fadeTextureTo;
            this._fadeTextureTween = null;
            this._fadeTextureTo    = null;
        }
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        if (this.Cropping != null) {
            if (this._fadeTextureTween != null) {
                float progress = this._fadeTextureTween.GetCurrent();

                Color orig = args.Color with {
                    Af = args.Color.Af * (1f - progress)
                };

                batch.Draw(this._fadeTextureTo, args.Position, args.Scale, args.Rotation, args.Color, this.Cropping.Value, args.Effects, this.RotationOrigin);
                batch.Draw(this.Texture,        args.Position, args.Scale, args.Rotation, orig,       this.Cropping.Value, args.Effects, this.RotationOrigin);
            } else
                batch.Draw(this.Texture, args.Position, args.Scale, args.Rotation, args.Color, this.Cropping.Value, args.Effects, this.RotationOrigin);
        } else {
            if (this._fadeTextureTween != null) {
                float progress = this._fadeTextureTween.GetCurrent();

                Color orig = args.Color with {
                    Af = args.Color.Af * (1f - progress)
                };

                batch.Draw(this._fadeTextureTo, args.Position, args.Scale, args.Rotation, args.Color, args.Effects, this.RotationOrigin);
                batch.Draw(this.Texture,        args.Position, args.Scale, args.Rotation, orig,       args.Effects, this.RotationOrigin);
            } else
                batch.Draw(this.Texture, args.Position, args.Scale, args.Rotation, args.Color, args.Effects, this.RotationOrigin);
        }
    }
}
