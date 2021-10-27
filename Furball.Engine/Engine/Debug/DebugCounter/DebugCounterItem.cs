using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    public abstract class DebugCounterItem {
        public virtual void Update(GameTime time) {}
        public virtual void Draw(GameTime time) {}

        public abstract string GetAsString(GameTime time);
    }
}
