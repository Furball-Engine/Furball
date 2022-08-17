using System.Collections.Generic;
using Furball.Engine.Engine.ECS.Components;
using Furball.Engine.Engine.ECS.Systems.Helpers;

namespace Furball.Engine.Engine.ECS.Systems; 

public class TweenSystem : EntitySystem {
    private EntityTweensComponent     _entityTweensComponent;
    private EntityTimeSourceComponent _entityTimeSourceComponent;

    public override void Initialize(Entity entity) {
        base.Initialize(entity);

        this._entityTweensComponent     = ComponentGuard.EnsureNonNull<EntityTweensComponent>(this, entity);
        this._entityTimeSourceComponent = ComponentGuard.EnsureNonNull<EntityTimeSourceComponent>(this, entity);
    }
}