using System.Drawing;
using Furball.Engine.Engine.ECS.Components;
using Furball.Engine.Engine.ECS.Systems.Helpers;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.ECS.Systems; 

public class DrawSystem : EntitySystem {
    private EntityTextureComponent     _texture;
    private EntityTransformComponent   _transform;
    private EntityColorComponent       _color;
    private EntityTextureCropComponent _crop;
    
    public override void Initialize(Entity entity) {
        base.Initialize(entity);

        this._transform = ComponentGuard.EnsureNonNull<EntityTransformComponent>(this, entity);
        this._texture   = ComponentGuard.EnsureNonNull<EntityTextureComponent>(this, entity);

        this._color = entity.GetComponent<EntityColorComponent>();
        this._crop  = entity.GetComponent<EntityTextureCropComponent>();
    }

    public override void Draw(double deltaTime) {
        base.Draw(deltaTime);

        Color     color = this._color?.Color ?? Color.White;
        Rectangle crop  = this._crop?.Crop ?? new Rectangle(0, 0, this._texture.Texture.Width, this._texture.Texture.Height);
        
        FurballGame.DrawableBatch.Draw(this._texture.Texture, this._transform.Position, this._transform.Scale, this._transform.Rotation, color, crop, this._texture.TextureFlip, this._transform.RotationOrigin);
    }
}
