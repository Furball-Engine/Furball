

using System.Numerics;

namespace Furball.Engine.Engine.Helpers; 

public enum BezierCurveType {
    Quadratic,
    Cubic
}

public class BezierHelper {
    public static Vector2 QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t) {
        Vector2 a = MathHelper.Lerp(p0, p1, t);
        Vector2 b = MathHelper.Lerp(p1, p2, t);
        return MathHelper.Lerp(a, b, t);
    }

    public static Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
        Vector2 a = MathHelper.Lerp(p0, p1, t);
        Vector2 b = MathHelper.Lerp(p1, p2, t);
        Vector2 c = MathHelper.Lerp(p2, p3, t);
        Vector2 d = MathHelper.Lerp(a,  b,  t);
        Vector2 e = MathHelper.Lerp(b,  c,  t);
        return MathHelper.Lerp(d, e, t);
    }
}