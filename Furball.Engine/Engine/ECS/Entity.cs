using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Furball.Vixie;

namespace Furball.Engine.Engine.ECS {
    public class Entity : GameComponent {
        public Vector3               Position;
        public Vector3               Rotation;

        private  List<EntityComponent> _components;

        public Entity(List<EntityComponent> components) {
            this._components = components;
        }

        public Entity(params EntityComponent[] components) {
            this._components = components.ToList();
        }

        /// <summary>
        /// Adds a GameComponent to the Component list and Initializes it
        /// </summary>
        /// <param name="component">Component to Add</param>
        public void Add(EntityComponent component) {
            this._components.Add(component);

            component.Initialize();
            //We need to make sure the Component list is sorted so we process it in the right order in Update an Draw
            this._components = this._components.OrderByDescending(c => c.ProcessOrder).ToList();
        }
        /// <summary>
        /// Removes a GameComponent from the list and unloads & disposes it
        /// </summary>
        /// <param name="component">Component to Remove</param>
        public void Remove(EntityComponent component) {
            this._components.Remove(component);

            component.Unload();
            component.Dispose();
        }
        /// <summary>
        /// Draws all added GameComponents
        /// </summary>
        /// <param name="deltaTime">Time since last Draw</param>
        public override void Draw(double deltaTime) {
            for (int i = 0; i != this._components.Count; i++) {
                EntityComponent current = this._components[i];

                current.Draw(deltaTime);
            }
        }
        /// <summary>
        /// Updates all added GameComponents
        /// </summary>
        /// <param name="deltaTime">Time since last Update</param>
        public override void Update(double deltaTime) {
            for (int i = 0; i != this._components.Count; i++) {
                EntityComponent current = this._components[i];

                current.Update(deltaTime);
            }
        }
        /// <summary>
        /// Disposes all GameComponents
        /// </summary>
        public override void Dispose() {
            for (int i = 0; i != this._components.Count; i++) {
                EntityComponent current = this._components[i];

                current.Dispose();
            }
        }
    }
}
