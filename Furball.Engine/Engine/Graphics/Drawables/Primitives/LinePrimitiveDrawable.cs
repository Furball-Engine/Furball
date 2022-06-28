using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie.Backends.Shared;

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
            if (this.RelativePosition) {
                batch.DrawLine(
                args.Position,
                args.Position + this.EndPosition * args.Scale,
                this.Thickness * args.Scale.X,
                this.ColorOverride
                );
            } 
            else {
                Vector2 diff = this.EndPosition - this.Position;

                batch.DrawLine(
                args.Position,
                args.Position + diff * args.Scale,
                this.Thickness * args.Scale.X,
                this.ColorOverride
                );
            }
        }
    }
}
