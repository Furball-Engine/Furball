using System.Numerics;

namespace Furball.Engine.Engine.ECS.Components; 

public class EntityTransformComponent : IEntityComponent {
    public Vector2 Position;
    public float   Rotation;
    public Vector2 RotationOrigin;
    public Vector2 Scale = Vector2.One;
}