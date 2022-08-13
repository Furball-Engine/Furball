
using System.Drawing;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables; 

/// <summary>
/// A Basic Managed Drawable that just Draws a Texture to the Screen,
/// </summary>
public class TexturedDrawable : Drawable {
    /// <summary>
    /// The Texture Being drawn
    /// </summary>
    protected Texture _texture;
    /// <summary>
    ///     The texture being drawn
    /// </summary>
    public Texture Texture => this._texture;
    /// <summary>
    /// Crop Rectangle, this basically tells which part of the Texture to Render
    /// Leave null to draw the entire Texture
    /// </summary>
    private Rectangle? _cropping = null;
    /// <summary>
    /// Unprocessed Size of the Drawable in Pixels
    /// <remarks>This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be properly visible on all resolutions</remarks>
    /// </summary>
    public override Vector2 Size => this._cropping == null ? new Vector2(this._texture.Size.X, this._texture.Size.Y) * this.Scale : new Vector2(this._cropping.Value.Width, this._cropping.Value.Height) * this.Scale;

    /// <summary>
    /// TexturedDrawable Constructor
    /// </summary>
    /// <param name="texture">Texture to Draw</param>
    /// <param name="position">Where to Draw</param>
    public TexturedDrawable(Texture texture, Vector2 position) {
        this.Position = position;

        this._texture = texture;
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

        this._cropping = cropping;
        this._texture  = texture;
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        if(this._cropping != null)
            batch.Draw(
            this._texture,
            args.Position,
            args.Scale,
            args.Rotation,
            args.Color,
            this._cropping.Value,
            args.Effects,
            this.RotationOrigin 
            );
        else
            batch.Draw(this._texture, args.Position, args.Scale, args.Rotation, args.Color, args.Effects, this.RotationOrigin);
    }

    /// <summary>
    /// Changes the Cropping of the Texture
    /// </summary>
    /// <param name="crop">New Cropping</param>
    public void ChangeCropping(Rectangle? crop) => this._cropping = crop;
    public void SetTexture(Texture texture) => this._texture = texture;
}