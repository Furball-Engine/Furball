using System;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Drawables.Tweens.TweenTypes {
    public class VectorTween : Tween {
        private readonly Vector2 _startVector;
        private readonly Vector2 _endVector;

        public VectorTween(TweenType type, Vector2 source, Vector2 dest, int startTime, int endTime, Easing easing = Easing.None) {
            if (type != TweenType.Movement && type != TweenType.Scale)
                throw new InvalidOperationException("Vector Tweens on Drawables can only be used for Movement and Scaling tweens!");

            this.TweenType = type;

            this._startVector = source;
            this._endVector   = dest;

            this.StartTime = startTime;
            this.EndTime   = endTime;
            this.Easing    = easing;
        }

        public Vector2 GetCurrent() {
            if (!this.Initiated)
                return this._startVector;
            if (this.Terminated)
                return this._endVector;

            return new Vector2(
                this.CalculateCurrent(this._startVector.X, this._endVector.X),
                this.CalculateCurrent(this._startVector.Y, this._endVector.Y)
            );
        }
    }
}
