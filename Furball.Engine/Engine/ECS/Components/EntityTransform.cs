using System.Numerics;

namespace Furball.Engine.Engine.ECS.Components {
    public class EntityTransform : IEntityComponent {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }
}
