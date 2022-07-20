namespace Furball.Engine.Engine.ECS; 

public class EntitySystem {
    /// <summary>
    /// The Entity this System is assigned to
    /// </summary>
    protected Entity AssignedEntity;
    /// <summary>
    /// Processing Order, higher order means drawn and updated first
    /// </summary>
    public double ProcessOrder = 0;
    /// <summary>
    /// Gets fired when the GameComponent first gets added to the Component List
    /// </summary>
    public virtual void Initialize(Entity entity) {
        this.AssignedEntity = entity;
    }
    /// <summary>
    /// Gets fired on Every Update
    /// </summary>
    /// <param name="deltaTime">How much time has passed since the Last Update</param>
    public virtual void Update(double deltaTime) {}
    /// <summary>
    /// Gets fired on Every Draw
    /// </summary>
    /// <param name="deltaTime">How much time has passed since the Last Draw</param>
    public virtual void Draw(double deltaTime) {}
    /// <summary>
    /// Used to Dispose any Disposable variables
    /// </summary>
    public virtual void Dispose() {}
    /// <summary>
    /// Used to Unload any data if necessary, also gets fired when the GameComponent gets removed from the Component List
    /// </summary>
    public virtual void Unload() {}
}