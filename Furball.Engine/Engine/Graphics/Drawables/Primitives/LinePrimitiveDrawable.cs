using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    public class LinePrimitiveDrawable : ManagedDrawable {
        public float   Thickness;
        public Vector2 EndPoint;

        public LinePrimitiveDrawable(Vector2 position, Vector2 endPoint, float thickness) {
            this.Position  = position;
            this.Thickness = thickness;
            this.EndPoint  = endPoint;
        }
        
        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.ShapeBatch.DrawLine(args.Position - args.Origin, this.EndPoint, 1f, args.Color, Color.White, this.Thickness);
        }
    }
}
