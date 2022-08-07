using System.Collections.Generic;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables; 

public class CompositeDrawable : Drawable {
    /// <summary>
    ///     The list of drawables contained in the CompositeDrawable
    /// </summary>
    public List<Drawable> Drawables = new();

    protected bool _sortDrawables;

    private readonly DrawableManagerArgs _drawableArgs = new();

    public bool InvisibleToInput = false;
    public bool ChildrenInvisibleToInput = false;
        
    public override Vector2 Size {
        get {
            Vector2 topLeft     = new(0, 0);
            Vector2 bottomRight = new(0, 0);

            for (int i = 0; i < this.Drawables.Count; i++) {
                Drawable drawable = this.Drawables[i];

                if (drawable.Rectangle.X < topLeft.X) topLeft.X = drawable.Rectangle.X;
                if (drawable.Rectangle.Y < topLeft.Y) topLeft.Y = drawable.Rectangle.Y;

                if (drawable.Rectangle.Right  > bottomRight.X) bottomRight.X = drawable.Rectangle.Right;
                if (drawable.Rectangle.Bottom > bottomRight.Y) bottomRight.Y = drawable.Rectangle.Bottom;
            }

            return (bottomRight - topLeft) * this.Scale;
        }
    }

    public override void Update(double time) {
        foreach (Drawable drawable in this.Drawables) {
            drawable.Update(time);
            drawable.UpdateTweens();
        }
    }

    public override void Dispose() {
        //this.OnClick -= this.OnDrawableClick;

        base.Dispose();
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        if (this._sortDrawables) {
            this.Drawables.Sort((x, y) => x.Depth.CompareTo(y.Depth));

            this._sortDrawables = false;
        }

        for (int i = 0; i < this.Drawables.Count; i++) {
            Drawable drawable = this.Drawables[i];

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (drawable.Depth != drawable.DrawablesLastKnownDepth)
                this._sortDrawables = true;

            drawable.DrawablesLastKnownDepth = drawable.Depth;
                
            drawable.LastCalculatedOrigin = drawable.CalculateOrigin();
            drawable.RealPosition         = args.Position + (drawable.Position - drawable.LastCalculatedOrigin) * args.Scale;

            if (!drawable.Visible) continue;


            this._drawableArgs.Color = new Color(
            (byte)(args.Color.Rf * this.ColorOverride.Rf * drawable.ColorOverride.R),
            (byte)(args.Color.Gf * this.ColorOverride.Gf * drawable.ColorOverride.G),
            (byte)(args.Color.Bf * this.ColorOverride.Bf * drawable.ColorOverride.B),
            (byte)(args.Color.Af * this.ColorOverride.Af * drawable.ColorOverride.A)
            );
            this._drawableArgs.Effects  = args.Effects;
            this._drawableArgs.Position = drawable.RealPosition;
            this._drawableArgs.Rotation = args.Rotation + drawable.Rotation;
            this._drawableArgs.Scale    = drawable.RealScale = args.Scale * drawable.Scale;
            
            drawable.Draw(time, batch, this._drawableArgs);
            if (FurballGame.DrawInputOverlay)
                switch (drawable.Clickable) {
                    case false when drawable.CoverClicks:
                        batch.DrawRectangle(
                        new(drawable.RealRectangle.X, drawable.RealRectangle.Y),
                        new(drawable.RealRectangle.Width, drawable.RealRectangle.Height),
                        1,
                        Color.Red
                        );
                        break;
                    case true when drawable.CoverClicks:
                        batch.DrawRectangle(
                        new(drawable.RealRectangle.X, drawable.RealRectangle.Y),
                        new(drawable.RealRectangle.Width, drawable.RealRectangle.Height),
                        1,
                        Color.Green
                        );
                        break;
                }
        }
    }
}