using System.Collections.Generic;
using Furball.Engine.Engine.ECS.Components;

namespace Furball.Engine.Engine.ECS.Systems; 

public class TweenSystem : EntitySystem {
    private EntityTweens     _entityTweens;
    private EntityTimeSource _entityTimeSource;

    public override void Initialize(Entity entity) {
        base.Initialize(entity);

        this._entityTweens     = this.AssignedEntity.GetComponent<EntityTweens>();
        this._entityTimeSource = this.AssignedEntity.GetComponent<EntityTimeSource>();

        if (this._entityTweens == null || this._entityTimeSource == null)
            throw new KeyNotFoundException("To assign a TweenSystem to an Entity, the Entity must have a EntityTweens and a EntityTimeSource Component.");
    }
}