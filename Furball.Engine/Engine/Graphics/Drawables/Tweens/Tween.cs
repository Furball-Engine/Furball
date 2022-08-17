using System;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie.Helpers.Helpers;

namespace Furball.Engine.Engine.Graphics.Drawables.Tweens; 

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
    public double StartTime;
    /// <summary>
    /// When does it End?
    /// </summary>
    public double EndTime;

    public double LastKnownStartTime = 0;
    public double LastKnownEndTime   = 0;

    /// <summary>
    /// How long is the Tween?
    /// </summary>
    public double Duration => this.EndTime - this.StartTime;
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
    /// If this is set to true the Tween won't be automatically removed when Terminated
    /// </summary>
    public bool KeepAlive;

    /// <summary>
    /// Time right now
    /// </summary>
    protected double TimeNow;

    /// <summary>
    /// Wrapper for <see cref="CalculateCurrent(double, double)"/> to not have to cast manually in code
    /// </summary>
    /// <param name="start">Start Value</param>
    /// <param name="end">End Value</param>
    /// <returns>Interpolated Value with Easing</returns>
    protected float CalculateCurrent(float start, float end) => (float)this.CalculateCurrent(start, (double)end);
    /// <summary>
    /// Interpoltes Start and End using the Easing and Progress of the Tween to return a Interpolated Value
    /// </summary>
    /// <param name="start">Start Value</param>
    /// <param name="end">End Value</param>
    /// <returns>Interpolated Value with Easing</returns>
    protected double CalculateCurrent(double start, double end) {
        double x = (this.TimeNow - this.StartTime) / this.Duration;

        x = x.Clamp(0d, 1d);
        
        //Most implementations taken from https://easings.net/
        return this.Easing switch {
            Easing.In             => MathHelper.Lerp(end, start, (1 - x) * (1 - x)),
            Easing.InSine         => 1 - Math.Cos(x * Math.PI / 2d),
            Easing.OutSine        => Math.Sin(x * Math.PI / 2d),
            Easing.InOutSine  => -(Math.Cos(Math.PI * x) - 1) / 2,
            Easing.InOutQuad  => x < 0.5 ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2,
            Easing.InQuad     => x * x,
            Easing.OutQuad    => 1 - (1 - x) * (1 - x),
            Easing.InCubic    => x * x * x,
            Easing.OutCubic   => 1 - Math.Pow(1 - x, 3),
            Easing.InOutCubic => x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2,
            Easing.InQuart    => x * x * x * x,
            Easing.OutQuart   => 1 - Math.Pow(1 - x, 4),
            Easing.InOutQuart => x < 0.5 ? 8 * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 4) / 2,
            Easing.Out            => MathHelper.Lerp(start, end,   x * x),
            Easing.InDouble       => MathHelper.Lerp(end,   start, Math.Pow(1 - x, 4)),
            Easing.OutDouble      => MathHelper.Lerp(start, end,   Math.Pow(x,     4)),
            Easing.InHalf         => MathHelper.Lerp(end,   start, Math.Pow(1 - x, 1.5)),
            Easing.OutHalf        => MathHelper.Lerp(start, end,   Math.Pow(x,     1.5)),
            Easing.InOut          => start + (-2 * Math.Pow(x, 3) + 3 * (x * x)) * (end - start),
            _                     => MathHelper.Lerp(start, end, x)
        };
    }
    /// <summary>
    /// Updates the Tween by giving it the Up to Date Time
    /// </summary>
    /// <param name="time">Current Time</param>
    public void Update(double time) {
        this.TimeNow = time;

        this.Initiated  = this.TimeNow >= this.StartTime;
        this.Terminated = this.Initiated && this.TimeNow > this.EndTime;
    }
}