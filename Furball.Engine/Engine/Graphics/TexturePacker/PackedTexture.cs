using System.Collections.Generic;
using Silk.NET.Maths;

namespace Furball.Engine.Engine.Graphics.TexturePacker; 

public class PackedTexture {
    public List<TakenSpace> Spaces;

    public Vector2D<int> Size;
}
