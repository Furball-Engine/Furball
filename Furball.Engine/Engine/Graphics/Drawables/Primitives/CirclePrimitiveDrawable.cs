using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    /// <summary>
    /// Simple Circle Drawable
    /// </summary>
    public class CirclePrimitiveDrawable : ManagedDrawable {
        public float Thickness;
        /// <summary>
        /// Creates a Circle
        /// </summary>
        /// <param name="position">Where to Draw</param>
        /// <param name="radius">How big should the Circle be</param>
        /// <param name="thickness">How thicc should the Border be</param>
        /// <param name="color">What Color should the Border be</param>
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
