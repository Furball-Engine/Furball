using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Foxfire.Engine.Engine.Drawables {
    public class DrawableManager {
        private List<UnmanagedDrawable> _unmanagedDrawables = new();
        private List<ManagedDrawable>   _managedDrawables   = new();

        public void Draw(GameTime time, SpriteBatch batch) {
            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            lock (this._managedDrawables) {
                for (int i = 0; i != this._managedDrawables.Count; i++) {
                    ManagedDrawable currentDrawable = this._managedDrawables[i];

                    currentDrawable.Draw(time, batch);
                }
            }

            batch.End();

            lock (this._unmanagedDrawables) {
                for (int i = 0; i != this._unmanagedDrawables.Count; i++) {
                    UnmanagedDrawable currentDrawable = this._unmanagedDrawables[i];

                    currentDrawable.Draw(time, batch);
                }
            }
        }

        public void Update(GameTime time) {
            lock (this._managedDrawables) {
                for (int i = 0; i != this._managedDrawables.Count; i++) {
                    ManagedDrawable currentDrawable = this._managedDrawables[i];

                    currentDrawable.Update(time);
                }
            }

            lock (this._unmanagedDrawables) {
                for (int i = 0; i != this._unmanagedDrawables.Count; i++) {
                    UnmanagedDrawable currentDrawable = this._unmanagedDrawables[i];

                    currentDrawable.Update(time);
                }
            }
        }
    }
}
