using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics.Drawables.Managers {
    public class DrawableManager : UnmanagedDrawable {
        private List<BaseDrawable> _drawables = new();

        private List<ManagedDrawable> tempDrawManaged = new();
        private List<ManagedDrawable> tempUpdateManaged = new();
        private List<UnmanagedDrawable> tempDrawUnmanaged = new();
        private List<UnmanagedDrawable> tempUpdateUnmanaged = new();
        public override void Draw(GameTime time, SpriteBatch batch) {
            // Split _drawables into 2 lists containing the ManagedDrawables and the UnmanagedDrawables
            this.tempDrawManaged.Clear();
            this.tempDrawUnmanaged.Clear();
            
            for (int i = 0; i != this._drawables.Count; i++) {
                BaseDrawable baseDrawable = this._drawables[i];

                if (baseDrawable is ManagedDrawable managedDrawable) tempDrawManaged.Add(managedDrawable);
                if (baseDrawable is UnmanagedDrawable unmanagedDrawable) tempDrawUnmanaged.Add(unmanagedDrawable);
            }

            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            for (int i = 0; i < tempDrawManaged.Count; i++) {
                ManagedDrawable currentDrawable = tempDrawManaged[i];

                currentDrawable.Draw(time, batch);
            }
            batch.End();

            for (int i = 0; i < tempDrawUnmanaged.Count; i++) {
                UnmanagedDrawable currentDrawable = tempDrawUnmanaged[i];

                currentDrawable.Draw(time, batch);
            }
        }

        public override void Update(GameTime time) {
            // Split _drawables into 2 lists containing the ManagedDrawables and the UnmanagedDrawables
            this.tempUpdateManaged.Clear();
            this.tempUpdateUnmanaged.Clear();
            
            for (int i = 0; i != this._drawables.Count; i++) {
                BaseDrawable baseDrawable = this._drawables[i];

                if (baseDrawable is ManagedDrawable managedDrawable) tempUpdateManaged.Add(managedDrawable);
                if (baseDrawable is UnmanagedDrawable unmanagedDrawable) tempUpdateUnmanaged.Add(unmanagedDrawable);
            }

            for (int i = 0; i < tempUpdateManaged.Count; i++) {
                ManagedDrawable currentDrawable = tempUpdateManaged[i];

                currentDrawable.UpdateTweens();
                currentDrawable.Update(time);
            }

            for (int i = 0; i < tempUpdateUnmanaged.Count; i++) {
                UnmanagedDrawable currentDrawable = tempUpdateUnmanaged[i];

                currentDrawable.UpdateTweens();
                currentDrawable.Update(time);
            }
        }

        public void Add(BaseDrawable drawable) => this._drawables.Add(drawable);
    }
}
