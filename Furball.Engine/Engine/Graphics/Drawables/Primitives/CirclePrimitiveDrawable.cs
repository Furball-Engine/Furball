using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Furball.Engine.Engine.Graphics.Drawables.Primatives {
    public class CirclePrimitiveDrawable : ManagedDrawable {
        public float Radius;
        public float Thickness = 1f;
        public int   Sides     = 10;

        public CirclePrimitiveDrawable(float radius, float thickness, Color color) {
            this.Radius        = radius;
            this.Thickness     = thickness;
            this.ColorOverride = color;
        }

        public override void Draw(GameTime time, SpriteBatch batch, DrawableManagerArgs args) {
            batch.DrawCircle(args.Position, this.Radius, this.Sides, args.Color, this.Thickness, args.LayerDepth);
        }
    }
}
