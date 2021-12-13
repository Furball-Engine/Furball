using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Collections.ObjectModel;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;


namespace Furball.Engine.Engine.Graphics.Drawables {
    public class CompositeDrawable : ManagedDrawable {
        /// <summary>
        ///     The list of drawables contained in the CompositeDrawable
        /// </summary>
        protected List<ManagedDrawable> _drawables = new();
        public ReadOnlyCollection<ManagedDrawable> Drawables => this._drawables.AsReadOnly();

        public override Vector2 Size {
            get {
                Vector2 topLeft     = new(0, 0);
                Vector2 bottomRight = new(0, 0);

                for (int i = 0; i < this._drawables.Count; i++) {
                    ManagedDrawable managedDrawable = this._drawables[i];
                    
                    if (managedDrawable.Rectangle.X < topLeft.X) topLeft.X = managedDrawable.Rectangle.X;
                    if (managedDrawable.Rectangle.Y < topLeft.Y) topLeft.Y = managedDrawable.Rectangle.Y;

                    if (managedDrawable.Rectangle.Right  > bottomRight.X) bottomRight.X = managedDrawable.Rectangle.Right;
                    if (managedDrawable.Rectangle.Bottom > bottomRight.Y) bottomRight.Y = managedDrawable.Rectangle.Bottom;
                }

                return (bottomRight - topLeft) * this.Scale;
            }
        }

        // public CompositeDrawable() => this.OnClick += this.OnDrawableClick;

        /// <summary>
        ///     When the drawable is clicked, this is called to allow for things inside of the drawable to be clicked
        /// </summary>
        // private void OnDrawableClick(object sender, Point e) {
        //     Point adjustedPoint = ((e.ToVector2() - this.Position + this.LastCalculatedOrigin) * this.Scale).ToPoint();
        //
        //     for (int i = 0; i < this._drawables.Count; i++) {
        //         ManagedDrawable drawable = this._drawables[i];
        //
        //         if (drawable.Contains(adjustedPoint)) {
        //             drawable.Click(true,  adjustedPoint);
        //             drawable.Click(false, adjustedPoint);
        //         }
        //     }
        // }

        public override void Update(double time) {
            foreach (ManagedDrawable drawable in this.Drawables) {
                drawable.Update(time);
                drawable.UpdateTweens();
            }
        }

        public override void Dispose() {
            //this.OnClick -= this.OnDrawableClick;

            base.Dispose();
        }

        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
            foreach (ManagedDrawable drawable in this.Drawables) {
                if (!drawable.Visible) continue;
                
                drawable.LastCalculatedOrigin = drawable.CalculateOrigin();
                drawable.RealPosition = args.Position + (drawable.Position - drawable.LastCalculatedOrigin) * args.Scale;

                if (!drawable.Visible) continue;

                DrawableManagerArgs drawableArgs = new() {
                    Color    = drawable.ColorOverride,
                    Effects  = args.Effects,
                    Position = drawable.RealPosition,
                    Rotation = args.Rotation,
                    Scale    = drawable.RealScale = args.Scale * drawable.Scale
                };

                drawable.Draw(time, batch, drawableArgs);
            }
        }
    }
}
