using Furball.Engine.Engine.Timing;

namespace Furball.Engine.Engine.ECS.Components {
    public class EntityTimeSource : IEntityComponent {
        public ITimeSource TimeSource;
    }
}
