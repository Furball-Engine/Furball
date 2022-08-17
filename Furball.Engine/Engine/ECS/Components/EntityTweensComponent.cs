using System.Collections.Generic;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;

namespace Furball.Engine.Engine.ECS.Components; 

public class EntityTweensComponent : IEntityComponent {
    public List<Tween> Tweens;

    public EntityTweensComponent() {
        this.Tweens = new List<Tween>();
    }
}