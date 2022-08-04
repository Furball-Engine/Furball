

using System;

namespace Furball.Engine.Engine.Debug.DebugCounter;

public abstract class DebugCounterItem : IDisposable {
    public virtual bool ForceNewLine { get; }
        
    public virtual void Update(double time) {}
    public virtual void Draw(double   time) {}


    public abstract string GetAsString(double time);
    public virtual  void   Dispose() {}
}