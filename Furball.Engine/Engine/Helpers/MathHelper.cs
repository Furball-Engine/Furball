using System;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Helpers {
    public static class MathHelper {
        //I wish i knew how lerp worked to explain this
        //i know tho >:3 -beyley
        /// <summary>
        /// Takes a linear interpolation of 2 values and an amount, the algorithm works as follows<br></br>
        ///
        /// The goal of this algorithm is to have
        /// <code>amount=0.0 == start</code>
        /// and
        /// <code>amount=1.0 == end</code>
        /// To be able to achive this we need to be able to change start by at least the difference of end and start,
        /// as start plus the difference of end and start is equal to end<br></br><br></br>
        /// Due to <code>start + (difference) == end</code>
        /// Adding half of difference to start should be half of the way to end from start, 1/4th of difference is 1/4th the way to end from start,
        /// which is the lerped value we want 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns>The interpolated value</returns>
        public static double Lerp(double a, double b, double t) => a + (b - a) * t;

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => (1f - t) * a + t * b;

        public static Vector2 QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t) {
            Vector2 a = Lerp(p0, p1, t);
            Vector2 b = Lerp(p1, p2, t);
            return Lerp(a, b, t);
        }

        public static Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
            Vector2 a = Lerp(p0, p1, t);
            Vector2 b = Lerp(p1, p2, t);
            Vector2 c = Lerp(p2, p3, t);
            Vector2 d = Lerp(a,  b,  t);
            Vector2 e = Lerp(b,  c,  t);
            return Lerp(d, e, t);
        }
        
        /// <summary>
        /// Convert a rotation amount in degrees to radians 
        /// </summary>
        /// <param name="deg">Amount in degrees</param>
        /// <returns>Amount in radians</returns>
        public static double DegreesToRadians(double deg) => deg * Math.PI / 180.0;
    }
}
