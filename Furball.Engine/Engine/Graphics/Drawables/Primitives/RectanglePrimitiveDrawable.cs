using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    /// <summary>
    /// Simple rectangle Drawable
    /// </summary>
    public class RectanglePrimitiveDrawable : ManagedDrawable {
        public bool    Filled;
        public Vector2 RectSize;
        public float   Thickness;
        public override Vector2 Size => this.RectSize;
        /// <summary>
        /// Creates a Rectangle
        /// </summary>
        /// <param name="position">Where to Draw</param>
        /// <param name="size">How big</param>
        /// <param name="thickness">How thicc</param>
        /// <param name="filled">Fill or not</param>
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
