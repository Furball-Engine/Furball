using System;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

/// <summary>
/// Basic Update Rate Counter, updates every second
/// </summary>
internal class UpdateRate : DebugCounterItem {
    private int    _lastUpdatedUpdateRate;
    private int    _updates;
    private double _deltaTime;

    public override void Update(double time) {
        this._deltaTime += time;
        this._updates++;

        if (this._deltaTime >= 1000d) {
            this._lastUpdatedUpdateRate =  this._updates;
            this._deltaTime             -= 1000d;
            this._updates               =  0;
        }

        base.Update(time);
    }

    public override string GetAsString(double time) => $"{this._lastUpdatedUpdateRate:N0}ups ({1000.0 / this._lastUpdatedUpdateRate:N2}ms)";
}
