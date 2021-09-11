using System;
using MathHelper=Furball.Engine.Engine.Helpers.MathHelper;

namespace Furball.Engine.Engine.Graphics.Drawables.Tweens {
    /// <summary>
    /// Base Tween Class
    /// </summary>
    public class Tween {
        /// <summary>
        /// What does the Tween affect on the Drawable
        /// </summary>
        public TweenType TweenType;

        /// <summary>
        /// When does it Start?
        /// </summary>
        public int StartTime;
        /// <summary>
        /// When does it End?
        /// </summary>
        public int EndTime;

        /// <summary>
        /// How long is the Tween?
        /// </summary>
        public int Duration => this.EndTime - this.StartTime;
        /// <summary>
        /// What easing to use?
        /// </summary>
        public Easing Easing;

        /// <summary>
        /// Has the Tween been Initiated
        /// </summary>
        public bool Initiated;
        /// <summary>
        /// Has the Tween ended and been Terminated
        /// </summary>
        public bool Terminated;

        /// <summary>
        /// Time right now
        /// </summary>
        protected int TimeNow;

        /// <summary>
        /// Wrapper for <see cref="CalculateCurrent(double, double)"/> to not have to cast manually in code
        /// </summary>
        /// <param name="start">Start Value</param>
        /// <param name="end">End Value</param>
        /// <returns>Interpolated Value with Easing</returns>
        protected float CalculateCurrent(float start, float end) => (float)this.CalculateCurrent((double)start, (double)end);
        /// <summary>
        /// Interpoltes Start and End using the Easing and Progress of the Tween to return a Interpolated Value
        /// </summary>
        /// <param name="start">Start Value</param>
        /// <param name="end">End Value</param>
        /// <returns>Interpolated Value with Easing</returns>
        protected double CalculateCurrent(double start, double end) {
            double progress = ((double)this.TimeNow - (double)this.StartTime) / (double)this.Duration;

            return this.Easing switch {
                Easing.In        => MathHelper.Lerp(end,   start, (1 - progress) * (1 - progress)),
                Easing.Out       => MathHelper.Lerp(start, end,   progress * progress),
                Easing.InDouble  => MathHelper.Lerp(end,   start, Math.Pow(1 - progress, 4)),
                Easing.OutDouble => MathHelper.Lerp(start, end,   Math.Pow(progress,     4)),
                Easing.InHalf    => MathHelper.Lerp(end,   start, Math.Pow(1 - progress, 1.5)),
                Easing.OutHalf   => MathHelper.Lerp(start, end,   Math.Pow(progress,     1.5)),
                Easing.InOut     => start + (-2 * Math.Pow(progress, 3) + 3 * (progress * progress)) * (end - start),
                _                => MathHelper.Lerp(start, end, progress)
            };
        }
        /// <summary>
        /// Updates the Tween by giving it the Up to Date Time
        /// </summary>
        /// <param name="time">Current Time</param>
        public void Update(int time) {
            this.TimeNow = time;

            this.Initiated  = this.TimeNow >= this.StartTime;
            this.Terminated = this.Initiated && this.TimeNow > this.EndTime;
        }
    }
}
