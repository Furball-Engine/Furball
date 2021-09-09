using System;

namespace Furball.Engine.Engine.Drawables.Tweens.TweenTypes {
    //currently no use but its there!
    public class DoubleTween : Tween {
        private readonly double _startDouble;
        private readonly double _endDouble;

        public DoubleTween(TweenType type, double source, double dest, int startTime, int endTime, Easing easing = Easing.None) {
            this.TweenType = type;

            this._startDouble = source;
            this._endDouble   = dest;

            this.StartTime = startTime;
            this.EndTime   = endTime;
            this.Easing    = easing;
        }

        public double GetCurrent() {
            if (!this.Initiated)
                return this._startDouble;
            if (this.Terminated)
                return this._endDouble;

            return this.CalculateCurrent(this._startDouble, this._endDouble);
        }
    }
}
