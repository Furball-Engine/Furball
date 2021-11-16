using System;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie.Graphics;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    /// <summary>
    /// Simple Line Drawable
    /// </summary>
    public class LinePrimitiveDrawable : ManagedDrawable {
        public Vector2 EndPosition;
        public float   Thickness;
        public bool    RelativePosition;

        public LinePrimitiveDrawable(Vector2 start, Vector2 end, Color color, float thickness = 1f, bool relative = true) {
            this.Position         = start;
            this.EndPosition      = end;
            this.ColorOverride    = color;
            this.Thickness        = thickness;
            this.RelativePosition = relative;
        }
        
        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.DrawLine(this.Position, this.RelativePosition ? this.Position + this.EndPosition: this.EndPosition, this.Thickness, this.ColorOverride);
        }
    }
}
