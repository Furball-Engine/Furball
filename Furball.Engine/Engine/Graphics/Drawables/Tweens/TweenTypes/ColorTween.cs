using System;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie.Graphics;

namespace Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes {
    /// <summary>
    /// Tweens a Color between a Start Value and an End Value over a Certain amount of Time
    /// </summary>
    public class ColorTween : Tween {
        /// <summary>
        /// Where does the Tween begin
        /// </summary>
        private readonly Color _startColor;
        /// <summary>
        /// Where does the Tween end
        /// </summary>
        private readonly Color _endColor;

        private ValueContainer<Color> _colorRefrence;

        /// <summary>
        /// Constructor for a basic Color Tween
        /// </summary>
        /// <param name="toModify">Value to Modify</param>
        /// <param name="source">Start Value</param>
        /// <param name="dest">End Value</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="endTime">End Time</param>
        /// <param name="easing">Easing</param>
        /// <exception cref="InvalidOperationException">You cannot use a Color Tween to Tween Movement, Fade or Rotation etc. </exception>
        public ColorTween(ValueContainer<Color> toModify, Color source, Color dest, double startTime, double endTime, Easing easing = Easing.None) {
            this._startColor = source;
            this._endColor   = dest;

            this.StartTime = startTime;
            this.EndTime   = endTime;
            this.Easing    = easing;

            this._colorRefrence = toModify;
        }
        /// <summary>
        /// Gets the Current Interpolated Value
        /// </summary>
        /// <returns></returns>
        public Color GetCurrent() {
            if (!this.Initiated)
                return this._startColor;
            if (this.Terminated)
                return this._endColor;

            return new Color(
                (int)this.CalculateCurrent(this._startColor.R, this._endColor.R),
                (int)this.CalculateCurrent(this._startColor.G, this._endColor.G),
                (int)this.CalculateCurrent(this._startColor.B, this._endColor.B),
                (int)this.CalculateCurrent(this._startColor.A, this._endColor.A)
            );
        }

        protected override void UpdateValue() {
            this._colorRefrence.Value = this.GetCurrent();
        }
    }
}
