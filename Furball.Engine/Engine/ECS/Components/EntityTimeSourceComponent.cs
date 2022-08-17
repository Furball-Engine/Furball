using Furball.Engine.Engine.Timing;

namespace Furball.Engine.Engine.ECS.Components; 

public class EntityTimeSourceComponent : IEntityComponent {
    public ITimeSource TimeSource;
}