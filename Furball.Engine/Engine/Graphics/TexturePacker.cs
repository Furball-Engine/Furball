using System.Collections.Generic;
using Furball.Vixie;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Furball.Engine.Engine.Graphics; 

public class TexturePacker {
    private readonly List<Image<Rgba32>> _images;
    
    public TexturePacker(List<Image<Rgba32>> images) {
        this._images = images;
    }

    public Texture Pack() {
        //TODO
        return Game.ResourceFactory.CreateEmptyTexture(1, 1);
    }
}
