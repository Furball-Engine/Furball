using System.Drawing;

namespace Furball.Engine.Engine.Graphics.TexturePacker; 

public class TextureToPack {
    public string Name;
    public Size   Size;

    public int Width => this.Size.Width;
    public int Height => this.Size.Height;
}
