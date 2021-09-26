using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    public class RectanglePrimitiveDrawable : ManagedDrawable {
        public bool    Filled;
        public Vector2 RectSize;
        public float   Thickness;
        public override Vector2 Size => this.RectSize;

        public RectanglePrimitiveDrawable(Vector2 position, Vector2 size, float thickness, bool filled) {
            this.Position  = position;
            this.RectSize  = size;
            this.Thickness = thickness;
            this.Filled    = filled;
        }

        public RectanglePrimitiveDrawable() { }
        
        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            if(this.Filled)
                batch.ShapeBatch.FillRectangle(args.Position - args.Origin, this.RectSize, args.Color);
            else
                batch.ShapeBatch.DrawRectangle(args.Position - args.Origin, this.RectSize, Color.Transparent, args.Color, this.Thickness);
        }
    }
}
