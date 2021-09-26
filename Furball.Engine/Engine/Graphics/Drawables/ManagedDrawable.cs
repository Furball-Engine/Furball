using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables {
    /// <summary>
    /// A Basic Drawable that gets passed in an Already Started SpriteBatch,
    /// Do not use this for Drawables that require Effects, or other Special SpriteBatch behaviour,
    /// as This Drawable is getting passed an Already Begun SpriteBatch
    /// use <see cref="UnmanagedDrawable"/> for this task
    /// <remarks>
    /// Basic Drawables get drawn before UnmanagedDrawables.
    /// </remarks>
    /// </summary>
    public abstract class ManagedDrawable : BaseDrawable {
        /// <summary>
        /// Method for Drawing the Drawable,
        /// Gets called every Draw
        /// </summary>
        /// <param name="time">How much time has passed since last Draw</param>
        /// <param name="batch">Already Started SpriteBatch</param>
        /// <param name="args">The DrawableManagerArgs variable, which contains the arguments to pass into your batch.Draw call</param>
        public virtual void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {}
        /// <summary>
        /// If your Drawable needs updating, this is the Method to Override,
        /// Gets Called every Update
        /// </summary>
        /// <param name="time">How much time has passed since last Update</param>
        public virtual void Update(GameTime time) {}
    }
}
