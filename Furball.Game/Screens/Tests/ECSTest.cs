using System.Numerics;
using Furball.Engine;
using Furball.Engine.Engine.ECS;
using Furball.Engine.Engine.ECS.Components;
using Furball.Engine.Engine.ECS.Systems;
using Furball.Engine.Engine.Graphics;

namespace Furball.Game.Screens.Tests; 

public class EcsTest : TestScreen {
    private Entity _entity;
    
    public override void Initialize() {
        base.Initialize();

        this._entity = new Entity();
        EntityTextureComponent   texture   = this._entity.AddComponent<EntityTextureComponent>();
        EntityTransformComponent transform = this._entity.AddComponent<EntityTransformComponent>();

        texture.Texture    = ContentManager.LoadTextureFromFile("test.png");
        transform.Position = new Vector2(100, 100);

        this._entity.AddSystem(new DrawSystem());
        
        FurballGame.Instance.RegisterEntity(this._entity);
    }
}
