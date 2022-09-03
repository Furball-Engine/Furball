using System;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives; 

/// <summary>
/// Simple Circle Drawable
/// </summary>
public class CirclePrimitiveDrawable : Drawable {
    /// <summary>
    ///     The detail of the circle
    /// </summary>
    public Bindable<int> Detail;

    public Vector2[] CalculatedPoints;

    public float Thickness;

    public void Recalculate() {
        this.CalculatedPoints = new Vector2[this.Detail];
            
        for (int i = 0; i < this.Detail; i++) {    
            double angle = 2 * Math.PI *i / this.Detail; 
            this.CalculatedPoints[i] = new Vector2((float)(Math.Cos(angle) * this.CircleRadius), (float)(Math.Sin(angle) * this.CircleRadius)); 
        } 
    }

    /// <summary>
    /// Creates a Circle
    /// </summary>
    /// <param name="position">Where to Draw</param>
    /// <param name="radius">How big should the Circle be</param>
    /// <param name="outlineColor">What Color should the Border be</param>
    /// <param name="thickness">The thickness of the outline</param>
    /// <param name="detail">How many sides are on the circle</param>
    public CirclePrimitiveDrawable(Vector2 position, float radius, Color outlineColor, float thickness, int detail = 25) {
        this.CircleRadius  = new Bindable<float>(radius);
        this.Circular      = true;
        this.ColorOverride = outlineColor;
        this.Position      = position;
        this.Detail        = new Bindable<int>(detail);
        this.Thickness     = thickness;
            
        this.Recalculate();

        this.Detail.OnChange        += (_, _) => this.Recalculate();
        this.CircleRadius!.OnChange += (_, _) => this.Recalculate();
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        for (int i = 0; i < this.CalculatedPoints.Length; i++) {
            batch.DrawLine(
            this.CalculatedPoints[i] + this.Position,
            (i == this.CalculatedPoints.Length - 1 ? this.CalculatedPoints[0] : this.CalculatedPoints[i + 1]) + this.Position,
            this.Thickness,
            this.ColorOverride
            );
        }
    }
}