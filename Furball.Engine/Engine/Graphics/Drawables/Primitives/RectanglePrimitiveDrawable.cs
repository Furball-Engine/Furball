using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Xssp.MonoGame.Primitives2D;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    /// <summary>
    /// Simple rectangle Drawable
    /// </summary>
    public class RectanglePrimitiveDrawable : ManagedDrawable {
        public bool    Filled;
        public Vector2 RectSize;
        public float   Thickness;
        public override Vector2 Size => this.RectSize * this.Scale;
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
            if(this.Filled) {
                batch.SpriteBatch.DrawRectangle(
                    args.Position * FurballGame.VerticalRatio,
                    this.Size * FurballGame.VerticalRatio,
                    args.Color,
                    this.Thickness * FurballGame.VerticalRatio,
                    args.LayerDepth
                );
                batch.SpriteBatch.FillRectangle(
                    args.Position * FurballGame.VerticalRatio, 
                    this.Size * FurballGame.VerticalRatio, 
                    args.Color, 
                    args.LayerDepth
                );
            }
            else
                batch.SpriteBatch.DrawRectangle(
                    args.Position * FurballGame.VerticalRatio,
                    this.Size * FurballGame.VerticalRatio,
                    args.Color,
                    this.Thickness * FurballGame.VerticalRatio,
                    args.LayerDepth
                );
        }
    }
}
