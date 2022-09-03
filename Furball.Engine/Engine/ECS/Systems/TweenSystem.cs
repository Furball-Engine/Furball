using Furball.Engine.Engine.ECS.Components;
using Furball.Engine.Engine.ECS.Systems.Helpers;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;

namespace Furball.Engine.Engine.ECS.Systems; 

public class TweenSystem : EntitySystem {
    private EntityTweensComponent     _entityTweensComponent;
    private EntityTimeSourceComponent _entityTimeSourceComponent;

    public override void Initialize(Entity entity) {
        base.Initialize(entity);

        this._entityTweensComponent     = ComponentGuard.EnsureNonNull<EntityTweensComponent>(this, entity);
        this._entityTimeSourceComponent = ComponentGuard.EnsureNonNull<EntityTimeSourceComponent>(this, entity);
    }

    public override void Update(double deltaTime) {
        base.Update(deltaTime);

        double time = this._entityTimeSourceComponent.TimeSource.GetCurrentTime();
        foreach (Tween tween in this._entityTweensComponent.Tweens) {
            tween.Update(time);
        }
    }
}