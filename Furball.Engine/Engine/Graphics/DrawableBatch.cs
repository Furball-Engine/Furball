using Apos.Shapes;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics {
    public class DrawableBatch {
        public SpriteBatch SpriteBatch;
        public ShapeBatch  ShapeBatch;

        public DrawableBatch(SpriteBatch spriteBatch, ShapeBatch shapeBatch) {
            this.SpriteBatch = spriteBatch;
            this.ShapeBatch  = shapeBatch;
        }
    }
}
