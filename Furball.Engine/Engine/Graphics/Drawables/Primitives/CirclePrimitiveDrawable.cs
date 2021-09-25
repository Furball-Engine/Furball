using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    public class CirclePrimitiveDrawable : ManagedDrawable {
        public float Thickness = 1f;
        public int   Sides     = 128;

        public CirclePrimitiveDrawable(Vector2 position, float radius, float thickness, Color color, int detail = 128) {
            this.CircleRadius  = radius;
            this.Circular      = true;
            this.Thickness     = thickness;
            this.ColorOverride = color;
            this.Position      = position;
            this.Sides         = detail;
        }

        public override void Draw(GameTime time, SpriteBatch batch, DrawableManagerArgs args) {
            batch.DrawCircle(args.Position, this.CircleRadius, this.Sides, args.Color, this.Thickness, args.LayerDepth);
        }
    }
}
