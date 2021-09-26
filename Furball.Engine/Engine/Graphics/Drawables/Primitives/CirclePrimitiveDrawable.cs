using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    public class CirclePrimitiveDrawable : ManagedDrawable {
        public float Thickness;

        public CirclePrimitiveDrawable(Vector2 position, float radius, float thickness, Color color) {
            this.CircleRadius  = radius;
            this.Circular      = true;
            this.Thickness     = thickness;
            this.ColorOverride = color;
            this.Position      = position;
        }

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.ShapeBatch.DrawCircle(args.Position, this.CircleRadius, Color.Transparent, args.Color, this.Thickness);
        }
    }
}
