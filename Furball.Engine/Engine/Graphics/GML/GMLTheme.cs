using FontStashSharp;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.GML; 

public class GMLTheme {
    public Color BackgroundFillColor = new Color(0, 0, 0, 175);

    public FontSystem Font            = FurballGame.DefaultFont;
    public float      DefaultFontSize = 14f;
}
