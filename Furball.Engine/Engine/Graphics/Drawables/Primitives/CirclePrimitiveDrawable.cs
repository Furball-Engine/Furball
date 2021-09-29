using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Xssp.MonoGame.Primitives2D;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    /// <summary>
    /// Simple Circle Drawable
    /// </summary>
    public class CirclePrimitiveDrawable : ManagedDrawable {
        public int Detail;
        /// <summary>
        /// Creates a Circle
        /// </summary>
        /// <param name="position">Where to Draw</param>
        /// <param name="radius">How big should the Circle be</param>
        /// <param name="outlineColor">What Color should the Border be</param>
        /// <param name="detail">How many sides are on the circle</param>
        public CirclePrimitiveDrawable(Vector2 position, float radius, Color outlineColor, int detail = 25) {
            this.CircleRadius  = radius;
            this.Circular      = true;
            this.ColorOverride = outlineColor;
            this.Position      = position;
            this.Detail        = detail;
        }

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.SpriteBatch.DrawCircle(args.Position * FurballGame.VerticalRatio, this.CircleRadius * FurballGame.VerticalRatio, this.Detail, args.Color, args.LayerDepth);
        }
    }
}
