using System;
using System.Numerics;


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
        /// <summary>
        /// Convert a rotation amount in degrees to radians 
        /// </summary>
        /// <param name="deg">Amount in degrees</param>
        /// <returns>Amount in radians</returns>
        public static double DegreesToRadians(double deg) => deg * Math.PI / 180.0;
        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param>
        /// <param name="value2">The second position in the interpolation.</param>
        /// <param name="value3">The third position in the interpolation.</param>
        /// <param name="value4">The fourth position in the interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>A position that is the result of the Catmull-Rom interpolation.</returns>
        public static float CatmullRom(float value1, float value2, float value3, float value4, float amount)
        {
            // Using formula from http://www.mvps.org/directx/articles/catmull/
            // Internally using doubles not to lose precission
            double amountSquared = amount * amount;
            double amountCubed = amountSquared * amount;
            return (float)(0.5 * (2.0 * value2 +
                                  (value3 - value1) * amount +
                                  (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
                                  (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed));
        }
        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains CatmullRom interpolation of the specified vectors.
        /// </summary>
        /// <param name="value1">The first vector in interpolation.</param>
        /// <param name="value2">The second vector in interpolation.</param>
        /// <param name="value3">The third vector in interpolation.</param>
        /// <param name="value4">The fourth vector in interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of CatmullRom interpolation.</returns>
        public static Vector2 CatmullRom(Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, float amount)
        {
            return new Vector2(
                CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
                CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount)
            );
        }
    }
}
