namespace Furball.Engine.Engine.Helpers {
    public static class MathHelper {
        public static double Lerp(double start, double end, double amount)
        {
            return start + ((end - start) * amount);
        }
    }
}
