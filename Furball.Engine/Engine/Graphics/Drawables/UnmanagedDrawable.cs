using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics.Drawables {
    /// <summary>
    /// A Drawable that gets passed in a raw SpriteBatch, giving the User Maximum Control over how the Drawable gets Drawn
    /// This is Perfect for a Drawable that needs Precise SpriteBatch Manipulation, Shaders, and other Things
    /// As The SpriteBatch doesn't Get Started by the DrawableManager,
    /// <remarks>
    /// Unmanaged Drawables get drawn after ManagedDrawables.
    /// </remarks>
    /// </summary>
    public abstract class UnmanagedDrawable : BaseDrawable {
        /// <summary>
        /// Method for Drawing the Drawable,
        /// Gets called every Draw
        /// </summary>
        /// <param name="time">How much time has passed since last Draw</param>
        /// <param name="batch">Raw SpriteBatch</param>
        public abstract void Draw(GameTime time, SpriteBatch batch, DrawableManagerArgs args);
        /// <summary>
        /// If your Drawable needs updating, this is the Method to Override,
        /// Gets Called every Update
        /// </summary>
        /// <param name="time">How much time has passed since last Update</param>
        public virtual void Update(GameTime time) {}
    }
}
