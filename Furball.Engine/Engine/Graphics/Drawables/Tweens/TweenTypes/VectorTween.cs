using System;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes {
    /// <summary>
    /// Tweens a Vector2 between a Start Value and an End Value over a Certain amount of Time
    /// </summary>
    public class VectorTween : Tween {
        /// <summary>
        /// Where does the Tween start
        /// </summary>
        private readonly Vector2 _startVector;
        /// <summary>
        /// Where does the Tween end
        /// </summary>
        private readonly Vector2 _endVector;

        /// <summary>
        /// Constructor for a basic Vector Tween
        /// </summary>
        /// <param name="type">What does this Tween affect on the Drawable</param>
        /// <param name="source">Start Value</param>
        /// <param name="dest">End Value</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="endTime">End Time</param>
        /// <param name="easing">Easing</param>
        /// <exception cref="InvalidOperationException">You cannot use a Vector Tween to Tween Color, Fade or Rotation etc. </exception>
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
        /// <summary>
        /// Gets the Current Interpolated Value
        /// </summary>
        /// <returns></returns>
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
