using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Drawables {
    public abstract class UnmanagedDrawable : BaseDrawable {
        public abstract void Draw(GameTime time, SpriteBatch batch);
        public virtual void Update(GameTime time) {}
    }
}
