using System;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie;

namespace Furball.Engine.Engine.Graphics.Drawables;

public class DebugWeakReferencedTextureDrawable : Drawable {
    private readonly WeakReference<Texture> _texture;

    public event EventHandler TextureReferenceCleared;

    public override Vector2 Size {
        get {
            if (!this._texture.TryGetTarget(out Texture tex)) {
                this.TextureReferenceCleared?.Invoke(this, EventArgs.Empty);
                return Vector2.Zero;
            }

            return new Vector2(tex.Size.X, tex.Size.Y) * this.Scale;
        }
    }

    public DebugWeakReferencedTextureDrawable(WeakReference<Texture> texture) => this._texture = texture;

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        if (!this._texture.TryGetTarget(out Texture tex)) {
            this.TextureReferenceCleared?.Invoke(this, EventArgs.Empty);
            return;
        }

        batch.Draw(tex, args.Position, args.Scale, args.Rotation, args.Color, args.Effects, this.RotationOrigin);
    }
}
