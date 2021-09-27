using Apos.Shapes;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics {
    /// <summary>
    /// A Basic Abstraction for Sprite and Shape batch
    /// </summary>
    public class DrawableBatch {
        public SpriteBatch SpriteBatch;
        public ShapeBatch  ShapeBatch;

        public DrawableBatch(SpriteBatch spriteBatch, ShapeBatch shapeBatch) {
            this.SpriteBatch = spriteBatch;
            this.ShapeBatch  = shapeBatch;
        }
    }
}
