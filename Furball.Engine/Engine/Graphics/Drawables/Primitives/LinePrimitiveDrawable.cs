using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Xssp.MonoGame.Primitives2D;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    /// <summary>
    /// Simple Line Drawable
    /// </summary>
    public class LinePrimitiveDrawable : ManagedDrawable {
        public float Length;
        public float Thickness = 1f;
        public float Angle;
        /// <summary>
        /// Creates a Line
        /// </summary>
        /// <param name="position">Where to Draw</param>
        /// <param name="length">The length of the line</param>
        /// <param name="angle">The angle of the line in radians</param>
        public LinePrimitiveDrawable(Vector2 position, float length, float angle) {
            this.Position = position;
            this.Length   = length;
            this.Angle    = angle;
        }
        
        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.SpriteBatch.DrawLine(args.Position * FurballGame.VerticalRatio, this.Length * FurballGame.VerticalRatio, this.Angle, args.Color, this.Thickness, 0f);
        }
    }
}
