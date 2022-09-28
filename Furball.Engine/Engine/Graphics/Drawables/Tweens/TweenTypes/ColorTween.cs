using System;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes; 

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

    /// <summary>
    /// Constructor for a basic Color Tween
    /// </summary>
    /// <param name="type">What does this Tween affect on the Drawable</param>
    /// <param name="source">Start Value</param>
    /// <param name="dest">End Value</param>
    /// <param name="startTime">Start Time</param>
    /// <param name="endTime">End Time</param>
    /// <param name="easing">Easing</param>
    /// <exception cref="InvalidOperationException">You cannot use a Color Tween to Tween Movement, Fade or Rotation etc. </exception>
    public ColorTween(TweenType type, Color source, Color dest, double startTime, double endTime, Easing easing = Easing.None) {
        if (type != TweenType.Color)
            throw new InvalidOperationException("Color Tweens on Drawables can only be used for Color Tweens!");

        this.TweenType = type;

        this._startColor = source;
        this._endColor   = dest;

        this.StartTime = startTime;
        this.EndTime   = endTime;
        this.Easing    = easing;
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
        this.CalculateCurrent(this._startColor.Rf, this._endColor.Rf),
        this.CalculateCurrent(this._startColor.Gf, this._endColor.Gf),
        this.CalculateCurrent(this._startColor.Bf, this._endColor.Bf),
        this.CalculateCurrent(this._startColor.Af, this._endColor.Af)
        );
    }
}