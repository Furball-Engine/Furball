using System;
using System.Numerics;

namespace Furball.Engine.Engine.Helpers; 

public static class MathHelper {
    //I wish i knew how lerp worked to explain this
    //i know tho >:3 -beyley
    /// <summary>
    ///     Takes a linear interpolation of 2 values and an amount, the algorithm works as follows<br></br>
    ///     The goal of this algorithm is to have
    ///     <code>amount=0.0 == start</code>
    ///     and
    ///     <code>amount=1.0 == end</code>
    ///     To be able to achive this we need to be able to change start by at least the difference of end and start,
    ///     as start plus the difference of end and start is equal to end<br></br><br></br>
    ///     Due to <code>start + (difference) == end</code>
    ///     Adding half of difference to start should be half of the way to end from start, 1/4th of difference is 1/4th the
    ///     way to end from start,
    ///     which is the lerped value we want
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <returns>The interpolated value</returns>
    public static double Lerp(double a, double b, double t) => a + (b - a) * t;

    public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => (1f - t) * a + t * b;
    /// <summary>
    ///     Convert a rotation amount in degrees to radians
    /// </summary>
    /// <param name="deg">Amount in degrees</param>
    /// <returns>Amount in radians</returns>
    public static double DegreesToRadians(double deg) => deg * Math.PI / 180.0;

    /// <summary>
    /// Calculate the catmullrom of 4 points
    /// <br></br>
    /// Formula used: http://www.mvps.org/directx/articles/catmull/
    /// </summary>
    /// <param name="p0">The first point</param>
    /// <param name="p1">The second point</param>
    /// <param name="p2">The third point</param>
    /// <param name="p3">The fourth point</param>
    /// <param name="t">The amount value</param>
    /// <returns></returns>
    public static double CatmullRom(double p0, double p1, double p2, double p3, double t) {
        double tSquared = t * t;
        double tCubed   = tSquared * t;

        return 0.5 * (2 * p1 + (-p0 + p2) * t + (2 * p0 - 5 * p1 + 4 * p2 - p3) * tSquared + (-p0 + 3 * p1 - 3 * p2 + p3) * tCubed);
    }

    public static Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, double t) => new(
    (float)CatmullRom(p0.X, p1.X, p2.X, p3.X, t),
    (float)CatmullRom(p0.Y, p1.Y, p2.Y, p3.Y, t)
    );
}