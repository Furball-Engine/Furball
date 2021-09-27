using System.Collections.Generic;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables {
    public class CompositeDrawable : ManagedDrawable {
        private List<ManagedDrawable> _drawableList = new();

        public bool InheritValues;

        public CompositeDrawable(Vector2 position, bool inheritValues = true) {
            this.InheritValues = inheritValues;
            this.Position      = position;
            this.Clickable     = false;
        }

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            if (this.InheritValues) {
                args.Position += this.Position;
                args.Rotation += this.Rotation;
                args.Scale    *= this.Scale;
            }

            for (int i = 0; i != this._drawableList.Count; i++) {
                ManagedDrawable currentDrawable = this._drawableList[i];

                currentDrawable.Draw(time, batch, args);
            }
        }

        public override void Update(GameTime time) {
            for (int i = 0; i != this._drawableList.Count; i++) {
                ManagedDrawable currentDrawable = this._drawableList[i];

                currentDrawable.Update(time);
                currentDrawable.UpdateTweens();
            }
        }

        public void Add(ManagedDrawable drawable) => this._drawableList.Add(drawable);
    }
}
