
using System.Drawing;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie;

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
    public override Vector2 Size => this.Cropping == null ? new Vector2(this.Texture.Size.X, this.Texture.Size.Y) * this.Scale : new Vector2(this.Cropping.Value.Width, this.Cropping.Value.Height) * this.Scale;

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

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        if(this.Cropping != null)
            batch.Draw(
            this.Texture,
            args.Position,
            args.Scale,
            args.Rotation,
            args.Color,
            this.Cropping.Value,
            args.Effects,
            this.RotationOrigin 
            );
        else
            batch.Draw(this.Texture, args.Position, args.Scale, args.Rotation, args.Color, args.Effects, this.RotationOrigin);
    }
}