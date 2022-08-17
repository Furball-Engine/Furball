using Furball.Vixie;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.ECS.Components; 

public class EntityTextureComponent : IEntityComponent {
    public Texture     Texture;
    public TextureFlip TextureFlip = TextureFlip.None;
}
