using System.Collections.Generic;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;

namespace Furball.Engine.Engine.ECS.Components {
    public class EntityTweens : IEntityComponent {
        public List<Tween> Tweens;

        public EntityTweens() {
            this.Tweens = new List<Tween>();
        }
    }
}
