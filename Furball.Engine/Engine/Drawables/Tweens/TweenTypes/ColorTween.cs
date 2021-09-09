using System;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Drawables.Tweens.TweenTypes {
    public class ColorTween : Tween {
        private readonly Color _startColor;
        private readonly Color _endColor;

        public ColorTween(TweenType type, Color source, Color dest, int startTime, int endTime, Easing easing = Easing.None) {
            if (type != TweenType.Color)
                throw new InvalidOperationException("Color Tweens on Drawables can only be used for Color Tweens!");

            this.TweenType = type;

            this._startColor = source;
            this._endColor   = dest;

            this.StartTime = startTime;
            this.EndTime   = endTime;
            this.Easing    = easing;
        }

        public Color GetCurrent() {
            if (!this.Initiated)
                return this._startColor;
            if (this.Terminated)
                return this._endColor;

            return new Color(
                (int) this.CalculateCurrent(this._startColor.R, this._endColor.R),
                (int) this.CalculateCurrent(this._startColor.G, this._endColor.G),
                (int) this.CalculateCurrent(this._startColor.B, this._endColor.B),
                (int) this.CalculateCurrent(this._startColor.A, this._endColor.A)
            );
        }
    }
}
