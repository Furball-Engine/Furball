using System.Collections.Generic;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables {
    /// <summary>
    /// CompositeDrawable allows for Multiple Drawables to be drawn in 1 drawable
    /// It can also inherit values, so for example if CompositeDrawable is scaled to 2x, and the drawables are all 0.5x, theyll end up being 1x
    /// It does the same to Position and Rotation (though only adding them up)
    /// </summary>
    public class CompositeDrawable : ManagedDrawable {
        private List<ManagedDrawable> _drawableList = new();

        public bool InheritValues;
        /// <summary>
        /// Creates a CompositeDrawable
        /// </summary>
        /// <param name="position">Where to Draw</param>
        /// <param name="inheritValues">Whether to Inherit Values or not</param>
        public CompositeDrawable(Vector2 position, bool inheritValues = true) {
            this.InheritValues = inheritValues;
            this.Position      = position;
            this.Clickable     = false;
        }

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            int count = this._drawableList.Count;

            if (count == 0)
                return;

            if (this.InheritValues) {
                args.Position += this.Position;
                args.Rotation += this.Rotation;
                args.Scale    *= this.Scale;
            }

            for (int i = 0; i != count; i++) {
                ManagedDrawable currentDrawable = this._drawableList[i];

                currentDrawable.Draw(time, batch, args);
            }
        }

        public override void Update(GameTime time) {
            int count = this._drawableList.Count;

            if (count == 0)
                return;

            for (int i = 0; i != count; i++) {
                ManagedDrawable currentDrawable = this._drawableList[i];

                currentDrawable.Update(time);
                currentDrawable.UpdateTweens();
            }
        }

        public void Add(ManagedDrawable drawable) => this._drawableList.Add(drawable);
    }
}
