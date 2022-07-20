

namespace Furball.Engine.Engine.Debug.DebugCounter.Items; 

public abstract class DebugCounterItem {
    public virtual bool ForceNewLine { get; }
        
    public virtual void Update(double time) {}
    public virtual void Draw(double   time) {}


    public abstract string GetAsString(double time);
}