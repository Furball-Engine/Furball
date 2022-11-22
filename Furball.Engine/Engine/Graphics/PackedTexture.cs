using System.Collections.Generic;
using Silk.NET.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Furball.Engine.Engine.Graphics; 

public class PackedTexture {
    public Image<Rgba32>                  Image;
    public List<TexturePacker.TakenSpace> Spaces;

    public Vector2D<int> Size;
}
