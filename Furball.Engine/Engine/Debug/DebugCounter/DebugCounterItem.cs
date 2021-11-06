

namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    public abstract class DebugCounterItem {
        public virtual bool ForceNewLine { get; }
        
        public virtual void Update(double time) {}
        public virtual void Draw(GameTime time) {}


        public abstract string GetAsString(GameTime time);
    }
}
