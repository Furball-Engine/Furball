using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;

namespace Furball.Engine.Engine.Graphics.Drawables.Primitives {
    /// <summary>
    /// Simple Line Drawable
    /// </summary>
    public class LinePrimitiveDrawable : ManagedDrawable {
        /// <summary>
        ///     The length of the line in pixels
        /// </summary>
        public float Length;
        /// <summary>
        ///     The thickness of the line
        /// </summary>
        public float Thickness = 1f;
        /// <summary>
        ///     The angle of the line in radians
        /// </summary>
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
        
        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.Renderer.DrawLine(args.Position * FurballGame.VerticalRatio, this.Length * FurballGame.VerticalRatio, this.Angle, args.Color, this.Thickness, 0f);
        }
    }
}
