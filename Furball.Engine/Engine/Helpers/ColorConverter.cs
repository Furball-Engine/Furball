using System.Drawing;
using Color=Microsoft.Xna.Framework.Color;

namespace Furball.Engine.Engine.Helpers {
    public static class ColorConverter {
        public static void FromHexString(this Color color, string input) {
            System.Drawing.Color drawingColor = ColorTranslator.FromHtml(input);

            color.R = drawingColor.R;
            color.G = drawingColor.G;
            color.B = drawingColor.B;
            color.A = drawingColor.A;
        }

        public static Color FromHexString(string input) {
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
    }
}
