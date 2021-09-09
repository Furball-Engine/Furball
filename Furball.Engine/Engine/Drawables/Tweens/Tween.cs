using System;
using System.ComponentModel.Design;
using MathHelper=Furball.Engine.Engine.Helpers.MathHelper;

namespace Furball.Engine.Engine.Drawables.Tweens {
    public class Tween {
        public TweenType TweenType;

        public int    StartTime;
        public int    EndTime;
        public int    Duration => this.EndTime - this.StartTime;
        public Easing Easing;

        public bool Initiated;
        public bool Terminated;

        protected int TimeNow;

        protected float CalculateCurrent(float start, float end) => (float) this.CalculateCurrent((double) start, (double) end);

        protected double CalculateCurrent(double start, double end) {
            double progress = ((double)this.TimeNow - (double)this.StartTime) / (double)this.Duration;

            return this.Easing switch {
                Easing.In        => MathHelper.Lerp(end, start, (1 - progress) * (1 - progress)),
                Easing.Out       => MathHelper.Lerp(start, end, progress * progress),
                Easing.InDouble  => MathHelper.Lerp(end, start, Math.Pow(1 - progress, 4)),
                Easing.OutDouble => MathHelper.Lerp(start, end, Math.Pow(progress, 4)),
                Easing.InHalf    => MathHelper.Lerp(end, start, Math.Pow(1 - progress, 1.5)),
                Easing.OutHalf   => MathHelper.Lerp(start, end, Math.Pow(progress, 1.5)),
                Easing.InOut     => start + (-2 * Math.Pow(progress, 3) + 3 * (progress * progress)) * (end - start),
                _                => MathHelper.Lerp(start, end, progress)
            };
        }

        public void Update(int time) {
            this.TimeNow = time;

            this.Initiated  = this.TimeNow >= this.StartTime;
            this.Terminated = this.Initiated && this.TimeNow > this.EndTime;
        }
    }
}
