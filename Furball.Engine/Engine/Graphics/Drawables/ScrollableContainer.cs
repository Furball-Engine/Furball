using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Input.Events;
using Furball.Vixie.Helpers.Helpers;

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
    
    private void OnMouseMove(object sender, MouseMoveEventArgs e) {
        this._realHovered = this.RealContains(e.Position);
    }

    public void Add(Drawable drawable) {
        //Save the original Y position
        drawable.Tags.Add(drawable.Position.Y.ToString(CultureInfo.InvariantCulture));

        this.Children!.Add(drawable);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (drawable.Position.Y + drawable.Size.Y > this._lastMax || drawable.Size.Y == this.Size.Y)
            this._lastMax = Math.Max(0, drawable.Position.Y + drawable.Size.Y);
    }

    private bool _realHovered;

    private void OnMouseScroll(object sender, MouseScrollEventArgs e) {
        if (this._realHovered) {
            this._targetScroll -= e.ScrollAmount.Y * this.ScrollSpeed;
            if (!this.InfiniteScrolling)
                this._targetScroll = this._targetScroll.Clamp(0, this._lastMax);
        }
    }

    public float ScrollSpeed = 32.0f;

    public  bool  InfiniteScrolling = false;
    private float _lastMax          = 0;

    public override void Update(double time) {
        foreach (Drawable drawable in this.Children!) {
            float origY = float.Parse(drawable.Tags[0]);

            float target = origY  - this._targetScroll;
            float diff   = target - drawable.Position.Y;

            drawable.Position.Y += (float)(diff * 0.985d * (time / 1000d) * 8);
        }

        base.Update(time);
    }

    public override void Dispose() {
        FurballGame.InputManager.OnMouseScroll -= this.OnMouseScroll;
        FurballGame.InputManager.OnMouseMove   -= this.OnMouseMove;

        base.Dispose();
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        batch.ScissorPush(this, new Rectangle((int)this.RealPosition.X, (int)this.RealPosition.Y, (int)this.RealSize.X, (int)this.RealSize.Y)); 
        base.Draw(time, batch, args);
        batch.ScissorPop(this);
    }

    public void RecalculateMax() {
        this._lastMax = 0;

        foreach (Drawable drawable in this.Children!)
            if (drawable.Position.Y + drawable.Size.Y - this.Size.Y > this._lastMax)
                this._lastMax = Math.Max(0, drawable.Position.Y + drawable.Size.Y - this.Size.Y);
    }

    public void SetTargetScroll(float f) {
        this._targetScroll = f.Clamp(9, this._lastMax);
    }
}
