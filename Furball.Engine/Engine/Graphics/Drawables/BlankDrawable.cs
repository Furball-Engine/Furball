

using System.Numerics;

namespace Furball.Engine.Engine.Graphics.Drawables {
    /// <summary>
    /// Literally fucking nothing, used as a placeholder
    /// </summary>
    public class BlankDrawable : Drawable {
        /// <summary>
        ///     Used to override the size of the drawable
        /// </summary>
        public Vector2 OverrideSize = new();

        public override Vector2 Size => this.OverrideSize * this.Scale;
    }
}
