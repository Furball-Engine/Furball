using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives; 

/// <summary>
/// Simple rectangle Drawable
/// </summary>
public class RectanglePrimitiveDrawable : Drawable {
    /// <summary>
    ///     Whether the rectangle is filled in
    /// </summary>
    public bool Filled;
    /// <summary>
    ///     The size of the rectangle
    /// </summary>
    public Vector2 RectSize;
    /// <summary>
    ///     The thickness of the outline
    /// </summary>
    public float Thickness;
        
    public override Vector2 Size => this.RectSize * this.Scale;
        
    /// <summary>
    /// Creates a Rectangle
    /// </summary>
    /// <param name="position">Where to Draw</param>
    /// <param name="size">How big</param>
    /// <param name="thickness">How thicc</param>
    /// <param name="filled">Fill or not</param>
    public RectanglePrimitiveDrawable(Vector2 position, Vector2 size, float thickness, bool filled) {
        this.Position  = position;
        this.RectSize  = size;
        this.Thickness = thickness;
        this.Filled    = filled;
    }

    public RectanglePrimitiveDrawable() { }
        
    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        if (this.Filled)
            batch.FillRectangle(
            args.Position,
            this.RealSize,
            args.Color
            );

        batch.DrawRectangle(
        args.Position,
        this.RealSize,
        this.Thickness * args.Scale.Y,
        args.Color
        );
    }
}