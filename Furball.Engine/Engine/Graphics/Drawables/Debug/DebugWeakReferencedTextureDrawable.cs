using System;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie;
using JetBrains.Annotations;

namespace Furball.Engine.Engine.Graphics.Drawables.Debug;

public class DebugWeakReferencedTextureDrawable : Drawable {
    [CanBeNull]
    private readonly WeakReference<Texture> _texture;
    [CanBeNull]
    private readonly WeakReference<RenderTarget> _renderTarget;

    public event EventHandler TextureReferenceCleared;

    public override Vector2 Size {
        get {
            if (this._texture != null) {
                if (!this._texture.TryGetTarget(out Texture tex)) {
                    this.TextureReferenceCleared?.Invoke(this, EventArgs.Empty);
                    return Vector2.Zero;
                }

                return new Vector2(tex.Size.X, tex.Size.Y) * this.Scale;
            }
            if (this._renderTarget != null) {
                if (!this._renderTarget.TryGetTarget(out RenderTarget target)) {
                    this.TextureReferenceCleared?.Invoke(this, EventArgs.Empty);
                    return Vector2.Zero;
                }

                return new Vector2(target.Size.X, target.Size.Y) * this.Scale;
            }

            return Vector2.Zero;
        }
    }

    public DebugWeakReferencedTextureDrawable(WeakReference<Texture>      texture) => this._texture = texture;
    public DebugWeakReferencedTextureDrawable(WeakReference<RenderTarget> reference) => this._renderTarget = reference;

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        if (this._texture != null) {
            if (!this._texture.TryGetTarget(out Texture tex)) {
                this.TextureReferenceCleared?.Invoke(this, EventArgs.Empty);
                return;
            }

            batch.Draw(tex, args.Position, args.Scale, args.Rotation, args.Color, args.Effects, this.RotationOrigin);
        }
        if (this._renderTarget != null) {
            if (!this._renderTarget.TryGetTarget(out RenderTarget tex)) {
                this.TextureReferenceCleared?.Invoke(this, EventArgs.Empty);
                return;
            }

            batch.Draw(tex, args.Position, args.Scale, args.Rotation, args.Color, args.Effects, this.RotationOrigin);
        }
    }
}
