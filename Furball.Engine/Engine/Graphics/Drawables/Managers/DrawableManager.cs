using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics.Drawables.Managers {
    public class DrawableManager : UnmanagedDrawable {
        private List<BaseDrawable> _drawables = new();

        public override void Draw(GameTime time, SpriteBatch batch) {
            // Split _drawables into 2 lists containing the ManagedDrawables and the UnmanagedDrawables
            List<ManagedDrawable>   managedDrawables   = new();
            List<UnmanagedDrawable> unmanagedDrawables = new();
            
            for (int i = 0; i != this._drawables.Count; i++) {
                BaseDrawable baseDrawable = this._drawables[i];
                
                if(baseDrawable is ManagedDrawable managedDrawable) managedDrawables.Add(managedDrawable);
                if(baseDrawable is UnmanagedDrawable unmanagedDrawable) unmanagedDrawables.Add(unmanagedDrawable);
            }

            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            for (int i = 0; i != managedDrawables.Count; i++) {
                ManagedDrawable currentDrawable = managedDrawables[i];

                currentDrawable.Draw(time, batch);
            }
            batch.End();

            for (int i = 0; i != unmanagedDrawables.Count; i++) {
                UnmanagedDrawable currentDrawable = unmanagedDrawables[i];

                currentDrawable.Draw(time, batch);
            }
        }

        public override void Update(GameTime time) {
            // Split _drawables into 2 lists containing the ManagedDrawables and the UnmanagedDrawables
            List<ManagedDrawable>   managedDrawables   = new();
            List<UnmanagedDrawable> unmanagedDrawables = new();
            
            for (int i = 0; i != this._drawables.Count; i++) {
                BaseDrawable baseDrawable = this._drawables[i];
                
                if(baseDrawable is ManagedDrawable managedDrawable) managedDrawables.Add(managedDrawable);
                if(baseDrawable is UnmanagedDrawable unmanagedDrawable) unmanagedDrawables.Add(unmanagedDrawable);
            }
            
            for (int i = 0; i != managedDrawables.Count; i++) {
                ManagedDrawable currentDrawable = managedDrawables[i];

                currentDrawable.UpdateTweens();
                currentDrawable.Update(time);
            }
        
            for (int i = 0; i != unmanagedDrawables.Count; i++) {
                UnmanagedDrawable currentDrawable = unmanagedDrawables[i];

                currentDrawable.UpdateTweens();
                currentDrawable.Update(time);
            }
        }

        public void Add(BaseDrawable drawable) => this._drawables.Add(drawable);
    }
}
