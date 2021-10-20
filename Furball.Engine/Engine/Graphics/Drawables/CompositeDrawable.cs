using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Furball.Engine.Engine.Graphics.Drawables.Managers;

namespace Furball.Engine.Engine.Graphics.Drawables {
    public class CompositeDrawable : ManagedDrawable {
        public List<ManagedDrawable> Drawables = new();

        public override Vector2 Size => new(0);

        public override void Update(GameTime time) {
            foreach (ManagedDrawable drawable in this.Drawables) {
                drawable.Update(time);
                drawable.UpdateTweens();
            }
        }

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            foreach (ManagedDrawable drawable in this.Drawables) {
                DrawableManagerArgs drawableArgs = new() {
                    Color    = drawable.ColorOverride,
                    Effects  = args.Effects,
                    Position = args.Position + (drawable.Position * args.Scale),
                    Rotation = args.Rotation,
                    Scale    = args.Scale * drawable.Scale
                };
                
                drawable.Draw(time, batch, drawableArgs);
            }
        }
    }
}
