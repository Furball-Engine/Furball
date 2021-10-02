using System.Drawing;
using Color=Microsoft.Xna.Framework.Color;

namespace Furball.Engine.Engine.Helpers {
    public class ColorConverter {
        public static Color FromString(string input) {
            System.Drawing.Color color = ColorTranslator.FromHtml(input);

            return new(color.R, color.G, color.B, color.A);
        }

        public static string ToHexString(Color input) {
            System.Drawing.Color color = System.Drawing.Color.FromArgb(input.A, input.R, input.G, input.B);


            return ColorTranslator.ToHtml(color);
        }
    }
}
