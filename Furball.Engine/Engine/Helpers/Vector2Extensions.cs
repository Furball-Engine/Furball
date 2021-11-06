using System.Drawing;
using System.Numerics;

namespace Furball.Engine.Engine.Helpers {
    public static class Vector2Extensions {
        public static Point ToPoint(this Vector2 vec2) => new Point((int) vec2.X, (int) vec2.Y);
        public static Size ToSize(this Vector2 vec2) => new Size((int) vec2.X, (int) vec2.Y);
    }
}
