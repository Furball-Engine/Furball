using System;

namespace Furball.Engine.Engine.Drawables.Tweens.TweenTypes {
    public class FloatTween : Tween {
        private readonly float _startFloat;
        private readonly float _endFloat;

        public FloatTween(TweenType type, float source, float dest, int startTime, int endTime, Easing easing = Easing.None) {
            if (type != TweenType.Fade && type != TweenType.Rotation)
                throw new InvalidOperationException("Color Tweens on Drawables can only be used for Color Tweens!");

            this.TweenType = type;

            this._startFloat = source;
            this._endFloat   = dest;

            this.StartTime = startTime;
            this.EndTime   = endTime;
            this.Easing    = easing;
        }

        public float GetCurrent() {
            if (!this.Initiated)
                return this._startFloat;
            if (this.Terminated)
                return this._endFloat;

            return this.CalculateCurrent(this._startFloat, this._endFloat);
        }
    }
}
