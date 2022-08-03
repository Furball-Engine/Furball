using System;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Vixie.Backends.Shared;

namespace pTyping.Engine.Debug;

public class DebugTextureDisplayDrawable : CompositeDrawable {
    public DebugTextureDisplayDrawable() {
        this.ResetLayout();
    }

    private double _updateDelta = 0;

    public event EventHandler LayoutReset;

    public override void Update(double time) {
        base.Update(time);

        this._updateDelta += time;
        if (this._updateDelta > 1) {
            this.ResetLayout();
            this._updateDelta = 0;
        }
    }

    private void ResetLayout() {
        this.Drawables.Clear();

        float x      = 0, y = 0;
        float higher = 0;
        for (int i = 0; i < Global.TRACKED_TEXTURES.Count; i++) {
            WeakReference<Texture> reference = Global.TRACKED_TEXTURES[i];

            if (!reference.TryGetTarget(out _))
                continue;//if the reference is invalid, then skip it

            DebugWeakReferencedTextureDrawable drawable = new(reference) {
                Position = new Vector2(x, y)
            };

            drawable.Scale = new Vector2(200f / drawable.Size.X);

            drawable.TextureReferenceCleared += delegate {
                this.ResetLayout();
            };

            if (drawable.Size.Y > higher)
                higher = drawable.Size.Y;

            if (i % 2 == 1) {
                y += higher + 10;
                x =  0;

                higher = 0;
            } else {
                x = 200;
            }

            this.Drawables.Add(drawable);
        }

        this.LayoutReset?.Invoke(null, EventArgs.Empty);
    }
}
