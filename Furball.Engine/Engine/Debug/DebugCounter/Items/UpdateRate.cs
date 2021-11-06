using System;


namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    /// <summary>
    /// Basic Update Rate Counter, updates every second
    /// </summary>
    public class UpdateRate : DebugCounterItem {
        private int    _lastUpdatedUpdateRate;
        private int    _updates;
        private double _deltaTime;

        public override void Update(double time) {
            this._deltaTime += time;
            this._updates++;

            if (this._deltaTime >= 1.0) {
                this._lastUpdatedUpdateRate = this._updates;
                this._deltaTime             = 0.0;
                this._updates               = 0;
            }

            base.Draw(time);
        }

        public override string GetAsString(double time) => $"{this._lastUpdatedUpdateRate}ups ({Math.Round(1000.0 / this._lastUpdatedUpdateRate, 2)}ms)";
    }
}
