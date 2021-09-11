using System;

namespace Furball.Engine.Engine.Helpers {
    public static class MathHelper {
        //I wish i knew how lerp worked to explain this
        public static double Lerp(double start, double end, double amount) => start + (end - start) * amount;

        public static double DegreesToRadians(double deg) => deg * Math.PI / 180.0;
    }
}
