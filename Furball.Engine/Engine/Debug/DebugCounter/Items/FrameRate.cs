using System;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items; 

/// <summary>
/// Basic Frame Rate counter, updates every second
/// </summary>
public class FrameRate : DebugCounterItem {
    private int    _lastUpdatedFramerate;
    private int    _frames;
    private double _deltaTime;

    public override void Draw(double time) {
        this._deltaTime += time;
        this._frames++;

        if (this._deltaTime >= 1000.0) {
            this._lastUpdatedFramerate = this._frames;
            this._deltaTime            = 0.0;
            this._frames               = 0;
        }

        base.Draw(time);
    }

    public override string GetAsString(double time) => $"{this._lastUpdatedFramerate:N0}fps ({Math.Round(1000.0 / this._lastUpdatedFramerate, 2):N2}ms)";
}