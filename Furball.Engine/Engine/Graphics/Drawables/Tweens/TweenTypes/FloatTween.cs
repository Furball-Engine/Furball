using System;

namespace Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes {
    /// <summary>
    /// Tweens a Float between a Start Value and an End Value over a Certain amount of Time
    /// </summary>
    public class FloatTween : Tween {
        /// <summary>
        /// Where does the Tween Begin
        /// </summary>
        private readonly float _startFloat;
        /// <summary>
        /// Where does the Tween end
        /// </summary>
        private readonly float _endFloat;

        /// <summary>
        /// Constructor for a basic Float Tween
        /// </summary>
        /// <param name="type">What does this Tween affect on the Drawable</param>
        /// <param name="source">Start Value</param>
        /// <param name="dest">End Value</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="endTime">End Time</param>
        /// <param name="easing">Easing</param>
        /// <exception cref="InvalidOperationException">You cannot use a Float Tween to Tween Movement, Color, etc. </exception>
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
        /// <summary>
        /// Gets the Current Interpolated Value
        /// </summary>
        /// <returns></returns>
        public float GetCurrent() {
            if (!this.Initiated)
                return this._startFloat;
            if (this.Terminated)
                return this._endFloat;

            return this.CalculateCurrent(this._startFloat, this._endFloat);
        }
    }
}
