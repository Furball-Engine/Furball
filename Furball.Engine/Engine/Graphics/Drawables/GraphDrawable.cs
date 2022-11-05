using System;
using System.Drawing;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables;

public struct GraphAxis {
    /// <summary>
    /// The label of the axis
    /// </summary>
    public string Label;

    /// <summary>
    /// The numerical start of the axis
    /// </summary>
    public double Start;
    /// <summary>
    /// The numerical end of the axis
    /// </summary>
    public double End;

    public double Length => this.End - this.Start;
}

public class GraphDrawable : Drawable {
    /// <summary>
    /// The render batch to use for drawing, this is used as a cache, so we dont have to recalculate everything every frame
    /// </summary>
    private readonly DrawableBatch _batch;

    private readonly Func<double, double> _graphFunction;

    public GraphAxis X;
    public GraphAxis Y;

    public override Vector2 Size => new Vector2((float)this.X.Length, (float)this.Y.Length) * this.Scale;

    /// <summary>
    /// Creates a new GraphDrawable
    /// </summary>
    /// <param name="graphFunction">The function of (x -> y) the graph uses</param>
    /// <param name="x">The X axis</param>
    /// <param name="y">The Y axis</param>
    public GraphDrawable(Func<double, double> graphFunction, GraphAxis x, GraphAxis y) {
        this._batch = new DrawableBatch();

        this._graphFunction = graphFunction;
        this.X              = x;
        this.Y              = y;

        this._batch.Begin();
        this._batch.End();
    }

    /// <summary>
    /// How many samples to calculate per X unit
    /// </summary>
    public double Resolution = 10;

    public void Recalculate() {
        // this._batch.Begin();



        // this._batch.SoftEnd();
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        batch.ScissorPush(this, new Rectangle(this.RealPosition.ToPoint(), this.RealSize.ToSize()));
        
        Vector2 position = this.RealPosition;

        float top    = position.Y;
        float bottom = (float)(position.Y + this.X.Length);
        float left   = position.X;
        float right  = (float)(position.X + this.Y.Length);

        //Iterate from the start of the X axis to the end of the X axis, with the resolution
        for (double x = this.X.Start; x < this.X.End; x += 1 / this.Resolution) {
            //Calculate the Y value for the current X value
            double y = this._graphFunction(x);

            //Calculate the position of the current point
            Vector2 point = new Vector2((float)(x - this.X.Start), (float)(y - this.Y.Start)) * this.Scale;

            //If the point is outside of the graph, skip it
            // if (point.X + position.X < left || point.X + position.X > right || point.Y + position.Y < top || point.Y + position.Y > bottom)
                // continue;

            //Calculate the position of the next point
            Vector2 nextPoint = new Vector2(
                                (float)((x + 1 / this.Resolution) - this.X.Start), 
                                (float)(this._graphFunction(x + 1 / this.Resolution) - this.Y.Start)) * this.Scale;

            //Draw the line
            batch.DrawLine(
            new Vector2(point.X + position.X,     bottom - (point.Y + position.Y)) + new Vector2(0,      this.Size.Y),
            new Vector2(nextPoint.X + position.X, bottom - (nextPoint.Y + position.Y))  + new Vector2(0, this.Size.Y),
            1,
            Color.White
            );
        }

        //Draw a background
        batch.DrawRectangle(position, this.RealSize, 2, new Color(0, 0, 0, 100));

        batch.ScissorPop(this);
        
        // this._batch.ManualDraw();
    }

    public override void Dispose() {
        this._batch.Dispose();

        base.Dispose();
    }
}
