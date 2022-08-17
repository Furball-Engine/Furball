using System;

namespace Furball.Engine.Engine.ECS.Systems.Helpers; 

public static class ComponentGuard {
    public static pComponentType EnsureNonNull<pComponentType>(EntitySystem system, Entity entity) where pComponentType : class, IEntityComponent, new() {
        pComponentType component = entity.GetComponent<pComponentType>();

        if (component == null)
            throw new Exception($"Entity of type {entity.GetType()} must have a Component of type {typeof(pComponentType)} to use with EntitySystem {system.GetType()}");
        
        return component;
    }
}
