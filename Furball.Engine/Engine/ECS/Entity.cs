using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Furball.Engine.Engine.ECS.Components;
using Furball.Vixie;

namespace Furball.Engine.Engine.ECS {
    public class Entity : GameComponent {
        public EntityTransform Transform;

        private List<EntitySystem>                     _systems;
        private ConcurrentDictionary<Type, IComponent> _components;

        public Entity(List<EntitySystem> systems) {
            this._systems = systems;
        }

        public Entity(params EntitySystem[] components) {
            this._systems = components.ToList();
        }

        public pComponentType GetComponent<pComponentType>() where pComponentType : class, IComponent {
            if (this._components.TryGetValue(typeof(pComponentType), out IComponent component)) {
                return (pComponentType) component;
            }

            return null;
        }

        public void AddComponent<pComponentType>(pComponentType component) where pComponentType : class, IComponent {
            if (this._components.ContainsKey(typeof(pComponentType)))
                throw new Exception("Component of this Type already added.");

            this._components.TryAdd(typeof(pComponentType), component);
        }

        /// <summary>
        /// Adds a GameComponent to the Component list and Initializes it
        /// </summary>
        /// <param name="system">Component to Add</param>
        public void AddSystem(EntitySystem system) {
            this._systems.Add(system);

            system.Initialize(this);
            //We need to make sure the Component list is sorted so we process it in the right order in Update an Draw
            this._systems = this._systems.OrderByDescending(c => c.ProcessOrder).ToList();
        }
        /// <summary>
        /// Removes a GameComponent from the list and unloads & disposes it
        /// </summary>
        /// <param name="system">Component to Remove</param>
        public void Remove(EntitySystem system) {
            this._systems.Remove(system);

            system.Unload();
            system.Dispose();
        }
        /// <summary>
        /// Draws all added GameComponents
        /// </summary>
        /// <param name="deltaTime">Time since last Draw</param>
        public override void Draw(double deltaTime) {
            for (int i = 0; i != this._systems.Count; i++) {
                EntitySystem current = this._systems[i];

                current.Draw(deltaTime);
            }
        }
        /// <summary>
        /// Updates all added GameComponents
        /// </summary>
        /// <param name="deltaTime">Time since last Update</param>
        public override void Update(double deltaTime) {
            for (int i = 0; i != this._systems.Count; i++) {
                EntitySystem current = this._systems[i];

                current.Update(deltaTime);
            }
        }
        /// <summary>
        /// Disposes all GameComponents
        /// </summary>
        public override void Dispose() {
            for (int i = 0; i != this._systems.Count; i++) {
                EntitySystem current = this._systems[i];

                current.Dispose();
            }
        }
    }
}
