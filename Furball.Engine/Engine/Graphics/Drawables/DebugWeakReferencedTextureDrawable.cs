using System;
using System.Numerics;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie.Backends.Shared;

namespace pTyping.Engine.Debug;

public class DebugWeakReferencedTextureDrawable : Drawable {
    private readonly WeakReference<Texture> Texture;

    public event EventHandler TextureReferenceCleared;

    public override Vector2 Size {
        get {
            if (!this.Texture.TryGetTarget(out Texture tex)) {
                this.TextureReferenceCleared?.Invoke(this, EventArgs.Empty);
                return Vector2.Zero;
            }

            return tex.Size * this.Scale;
        }
    }

    public DebugWeakReferencedTextureDrawable(WeakReference<Texture> texture) => this.Texture = texture;

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        if (!this.Texture.TryGetTarget(out Texture tex)) {
            this.TextureReferenceCleared?.Invoke(this, EventArgs.Empty);
            return;
        }

        batch.Draw(tex, args.Position, args.Scale, args.Rotation, args.Color, args.Effects, this.RotationOrigin);
    }
}
