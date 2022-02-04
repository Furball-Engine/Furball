using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Furball.Engine.Engine.ECS.Components;
using Furball.Vixie;

namespace Furball.Engine.Engine.ECS {
    public class Entity : GameComponent {
        /// <summary>
        /// Transform of the Entity
        /// </summary>
        public EntityTransform Transform;

        /// <summary>
        /// All the Entities Systems
        /// </summary>
        private List<EntitySystem>                     _systems;
        /// <summary>
        /// All the Entities Components
        /// </summary>
        private ConcurrentDictionary<Type, IEntityComponent> _components;

        /// <summary>
        /// Creates a Empty Entity
        /// </summary>
        public Entity() {
            this.DefaultInitialize();
        }
        /// <summary>
        /// Creates a Entity with EntitySystems and potentially EntityComponents
        /// </summary>
        /// <param name="systems"></param>
        /// <param name="components"></param>
        public Entity(IList<EntitySystem> systems = null, IList<IEntityComponent> components = null) {
            this.DefaultInitialize();

            this._systems = systems?.ToList();

            if (components == null)
                return;

            for (int i = 0; i != components.Count; i++) {
                IEntityComponent current = components[i];

                if (this._components.ContainsKey(current.GetType()))
                    throw new Exception("Component of this Type already added.");

                this._components.TryAdd(current.GetType(), current);
            }
        }

        /// <summary>
        /// Gets a Component
        /// </summary>
        /// <typeparam name="pComponentType">Type of Component to Get</typeparam>
        /// <returns>Assuming it exists, the Component you're looking for</returns>
        public pComponentType GetComponent<pComponentType>() where pComponentType : class, IEntityComponent, new()  {
            if (this._components.TryGetValue(typeof(pComponentType), out IEntityComponent component)) {
                return (pComponentType) component;
            }

            return null;
        }

        /// <summary>
        /// Adds a Component
        /// </summary>
        /// <param name="component">Instance of Component to Add</param>
        /// <typeparam name="pComponentType">Type of Component</typeparam>
        /// <exception cref="Exception">Throws if the Type of Component already exists</exception>
        public void AddComponent<pComponentType>(pComponentType component) where pComponentType : class, IEntityComponent, new() {
            if (this._components.ContainsKey(typeof(pComponentType)))
                throw new Exception("Component of this Type already added.");

            this._components.TryAdd(typeof(pComponentType), component);
        }

        /// <summary>
        /// Adds and Instanciates a new Component of the specified type
        /// </summary>
        /// <typeparam name="pComponentType">Type of Component</typeparam>
        /// <exception cref="Exception">Throws if the Type of Component already exists</exception>
        public void AddComponent<pComponentType>() where pComponentType : class, IEntityComponent, new() {
            if (this._components.ContainsKey(typeof(pComponentType)))
                throw new Exception("Component of this Type already added.");

            this._components.TryAdd(typeof(pComponentType), new pComponentType());
        }

        /// <summary>
        /// Default Initialization, Initializes Systems and Components and adds the Transform Component to the entity
        /// </summary>
        private void DefaultInitialize() {
            this._systems    = new List<EntitySystem>();
            this._components = new ConcurrentDictionary<Type, IEntityComponent>();

            this.AddComponent(this.Transform);
        }

        /// <summary>
        /// Adds a GameComponent to the Component list and Initializes it
        /// </summary>
        /// <param name="system">Component to Add</param>
        public Entity AddSystem(EntitySystem system) {
            this._systems.Add(system);

            system.Initialize(this);
            //We need to make sure the Component list is sorted so we process it in the right order in Update an Draw
            this._systems = this._systems.OrderByDescending(c => c.ProcessOrder).ToList();

            return this;
        }
        /// <summary>
        /// Removes a GameComponent from the list and unloads & disposes it
        /// </summary>
        /// <param name="system">Component to Remove</param>
        public Entity Remove(EntitySystem system) {
            this._systems.Remove(system);

            system.Unload();
            system.Dispose();

            return this;
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
