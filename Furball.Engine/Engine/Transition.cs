using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine {
    public abstract class Transition : UnmanagedDrawable {
        protected DrawableManager Manager = new();

        public abstract void TransitionBegin();
        public virtual void TransitionEnd() {}

        public override void Draw(GameTime time, DrawableBatch drawableBatch, DrawableManagerArgs args = null) {
            this.Manager.Draw(time, drawableBatch, args);
        }

        public override void Update(GameTime time) {
            this.Manager.Update(time);
        }
    }
}
