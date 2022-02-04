using System.Numerics;

namespace Furball.Engine.Engine.ECS.Components {
    public struct EntityTransform : IComponent {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }
}
