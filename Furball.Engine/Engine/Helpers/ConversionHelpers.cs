using System.Drawing;
using System.Numerics;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Helpers;

public static class ConversionHelpers {
    public static void ColorFromHexString(ref this Color color, string input) {
        System.Drawing.Color drawingColor = ColorTranslator.FromHtml(input);

        color.R = drawingColor.R;
        color.G = drawingColor.G;
        color.B = drawingColor.B;
        color.A = drawingColor.A;
    }

    public static Color ColorFromHexString(string input) {
        Color color = new();

        System.Drawing.Color drawingColor = ColorTranslator.FromHtml(input);

        color.R = drawingColor.R;
        color.G = drawingColor.G;
        color.B = drawingColor.B;
        color.A = drawingColor.A;

        return color;
    }

    public static string ToHexString(this Color input) {
        System.Drawing.Color color = System.Drawing.Color.FromArgb(input.A, input.R, input.G, input.B);

        return ColorTranslator.ToHtml(color);
    }

    public static Vector2 ToVector2(this Point  point) => new(point.X, point.Y);
    public static Vector2 ToVector2(this PointF point) => new(point.X, point.Y);
    public static Vector2 ToVector2(this Size   size)  => new(size.Width, size.Height);
    public static Vector2 ToVector2(this SizeF  size)  => new(size.Width, size.Height);

    public static Point  ToPoint(this  Vector2 vec2) => new((int)vec2.X, (int)vec2.Y);
    public static PointF ToPointF(this Vector2 vec2) => new(vec2.X, vec2.Y);
    public static Size   ToSize(this   Vector2 vec2) => new((int)vec2.X, (int)vec2.Y);
    public static SizeF  ToSizeF(this  Vector2 vec2) => new(vec2.X, vec2.Y);
}
