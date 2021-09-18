using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics.Drawables.Managers {
    public class DrawableManager : UnmanagedDrawable {
        private List<BaseDrawable> _drawables = new();

        private List<ManagedDrawable>   tempDrawManaged     = new();
        private List<ManagedDrawable>   tempUpdateManaged   = new();
        private List<UnmanagedDrawable> tempDrawUnmanaged   = new();
        private List<UnmanagedDrawable> tempUpdateUnmanaged = new();
        public override void Draw(GameTime time, SpriteBatch batch, DrawableManagerArgs _ = null) {
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

                DrawableManagerArgs args = new() {
                    Color      = currentDrawable.ColorOverride,
                    Effects    = currentDrawable.SpriteEffect,
                    LayerDepth = currentDrawable.Depth,
                    Origin     = CalculateNewOriginPosition(currentDrawable),
                    Position   = currentDrawable.Position,
                    Rotation   = currentDrawable.Rotation,
                    Scale      = currentDrawable.Scale
                };

                currentDrawable.Draw(time, batch, args);
            }
            batch.End();

            for (int i = 0; i < tempDrawUnmanaged.Count; i++) {
                UnmanagedDrawable currentDrawable = tempDrawUnmanaged[i];

                DrawableManagerArgs args = new() {
                    Color      = currentDrawable.ColorOverride,
                    Effects    = currentDrawable.SpriteEffect,
                    LayerDepth = currentDrawable.Depth,
                    Origin     = CalculateNewOriginPosition(currentDrawable),
                    Position   = currentDrawable.Position,
                    Rotation   = currentDrawable.Rotation,
                    Scale      = currentDrawable.Scale
                };

                currentDrawable.Draw(time, batch, args);
            }
        }

        private static Vector2 CalculateNewOriginPosition(BaseDrawable drawable) {
            return drawable.OriginType switch {
                OriginType.TopLeft     => Vector2.Zero,
                OriginType.TopRight    => new Vector2(drawable.Size.X, 0),
                OriginType.BottomLeft  => new Vector2(0,               drawable.Size.Y),
                OriginType.BottomRight => new Vector2(drawable.Size.X, drawable.Size.Y),
                _                      => throw new ArgumentOutOfRangeException(nameof (drawable.OriginType), "That OriginType is unsupported.")
            };
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
