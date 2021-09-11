namespace Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes {
    //currently no use but its there!
    /// <summary>
    /// Tweens a Double between a Start Value and an End Value over a Certain amount of Time
    /// </summary>
    public class DoubleTween : Tween {
        /// <summary>
        /// Where does the Tween begin
        /// </summary>
        private readonly double _startDouble;
        /// <summary>
        /// Where does the Tween end
        /// </summary>
        private readonly double _endDouble;

        /// <summary>
        /// Constructor for a basic Double Tween
        /// </summary>
        /// <param name="type">What does this Tween affect on the Drawable</param>
        /// <param name="source">Start Value</param>
        /// <param name="dest">End Value</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="endTime">End Time</param>
        /// <param name="easing">Easing</param>
        public DoubleTween(TweenType type, double source, double dest, int startTime, int endTime, Easing easing = Easing.None) {
            this.TweenType = type;

            this._startDouble = source;
            this._endDouble   = dest;

            this.StartTime = startTime;
            this.EndTime   = endTime;
            this.Easing    = easing;
        }
        /// <summary>
        /// Gets the Current Interpolated Value
        /// </summary>
        /// <returns></returns>
        public double GetCurrent() {
            if (!this.Initiated)
                return this._startDouble;
            if (this.Terminated)
                return this._endDouble;

            return this.CalculateCurrent(this._startDouble, this._endDouble);
        }
    }
}
