using System.Collections.Generic;
using Silk.NET.Maths;
using Rectangle=System.Drawing.Rectangle;

namespace Furball.Engine.Engine.Graphics.TexturePacker; 

public class PackedTexture {
    public List<TakenSpace> Spaces;
    public List<Rectangle>  EmptySpaces;

    public Vector2D<int> Size;
}
