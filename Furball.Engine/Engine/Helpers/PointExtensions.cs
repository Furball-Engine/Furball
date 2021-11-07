
using System.Drawing;
using System.Numerics;

namespace Furball.Engine.Engine.Helpers {
    public static class PointExtensions {
        public static Vector2 ToVector2(this Point point) => new Vector2(point.X, point.Y);
    }
}
