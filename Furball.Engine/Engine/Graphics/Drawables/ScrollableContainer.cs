using System;
using System.Globalization;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie;
using Furball.Vixie.Helpers.Helpers;
using SixLabors.ImageSharp;

namespace Furball.Engine.Engine.Graphics.Drawables;

public class ScrollableContainer : CompositeDrawable {
    private float _targetScroll;

    private readonly Vector2 _size;
    public override  Vector2 Size => this._size * this.Scale;

    public ScrollableContainer(Vector2 size) {
        FurballGame.InputManager.OnMouseScroll += this.OnMouseScroll;
        FurballGame.InputManager.OnMouseMove   += this.OnMouseMove;

        this._size = size;
    }
    private void OnMouseMove(object sender, (Vector2 position, string cursorName) e) {
        this._realHovered = this.RealContains(e.position.ToPoint());
    }

    public void Add(Drawable drawable) {
        //Save the original Y position
        drawable.Tags.Add(drawable.Position.Y.ToString(CultureInfo.InvariantCulture));

        this.Drawables.Add(drawable);

        if (drawable.Position.Y + drawable.Size.Y - this.Size.Y > this._lastMax)
            this._lastMax = Math.Max(0, drawable.Position.Y + drawable.Size.Y - this.Size.Y);
    }

    private bool _realHovered;

    private void OnMouseScroll(object sender, ((int scrollWheelId, float scrollAmount) scroll, string cursorName) e) {
        if (this._realHovered) {
            this._targetScroll -= e.scroll.scrollAmount * this.ScrollSpeed;
            if (!this.InfiniteScrolling)
                this._targetScroll = this._targetScroll.Clamp(0, this._lastMax);
        }
    }

    public float ScrollSpeed = 4.0f;

    public  bool  InfiniteScrolling = false;
    private float _lastMax          = 0;

    public override void Update(double time) {
        foreach (Drawable drawable in this.Drawables) {
            float origY = float.Parse(drawable.Tags[0]);

            float target = origY  - this._targetScroll;
            float diff   = target - drawable.Position.Y;

            drawable.Position.Y += (float)(diff * 0.985d * time * 8);
        }

        base.Update(time);
    }

    public override void Dispose() {
        FurballGame.InputManager.OnMouseScroll -= this.OnMouseScroll;
        FurballGame.InputManager.OnMouseMove   -= this.OnMouseMove;

        base.Dispose();
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        batch.End();

        Rectangle rect = GraphicsBackend.Current.ScissorRect;

        Rectangle newRect = new(
        new((int)(this.RealPosition.X * FurballGame.VerticalRatio), (int)(this.RealPosition.Y * FurballGame.VerticalRatio)),
        new((int)(this._size.X        * FurballGame.VerticalRatio), (int)(this._size.Y        * FurballGame.VerticalRatio))
        );

        GraphicsBackend.Current.ScissorRect = newRect;

        batch.Begin();
        base.Draw(time, batch, args);
        batch.End();

        GraphicsBackend.Current.ScissorRect = rect;
        batch.Begin();
    }

    public void RecalculateMax() {
        this._lastMax = 0;

        foreach (Drawable drawable in this.Drawables)
            if (drawable.Position.Y + drawable.Size.Y - this.Size.Y > this._lastMax)
                this._lastMax = Math.Max(0, drawable.Position.Y + drawable.Size.Y - this.Size.Y);
    }
}
