using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Silk.NET.Core;
using Color = Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables; 

public class CompositeDrawable : Drawable {
    protected bool SortDrawables;

    public CompositeDrawable() {
        this.Children = new List<Drawable>();
    }
    
    private readonly DrawableManagerArgs _drawableArgs = new();

    public bool InvisibleToInput = false;
    public bool ChildrenInvisibleToInput = false;
        
    public override Vector2 Size {
        get {
            Vector2 topLeft     = new(0, 0);
            Vector2 bottomRight = new(0, 0);

            for (int i = 0; i < this.Children!.Count; i++) {
                Drawable drawable = this.Children[i];

                RectangleF rect = drawable.Rectangle;

                rect.Location = new PointF(rect.Location.X - drawable.LastCalculatedOrigin.X, rect.Location.Y - drawable.LastCalculatedOrigin.Y);

                if (rect.X < topLeft.X) topLeft.X = rect.X;
                if (rect.Y < topLeft.Y) topLeft.Y = rect.Y;

                if (rect.Right  > bottomRight.X) bottomRight.X = rect.Right;
                if (rect.Bottom > bottomRight.Y) bottomRight.Y = rect.Bottom;
            }

            return (bottomRight - topLeft) * this.Scale;
        }
    }

    public ReaderWriterLockSlim ChildrenLock = new ReaderWriterLockSlim();
    
    public override void Update(double time) {
        bool taken = this.ChildrenLock.TryEnterUpgradeableReadLock(1);

        for (int i = 0; i < this.Children!.Count; i++) {
            Drawable drawable = this.Children![i];
            drawable.Update(time);
            drawable.UpdateTweens();
        }

        if (taken)
            this.ChildrenLock.ExitUpgradeableReadLock();
    }

    public override void Dispose() {
        if (this.Children != null)
            foreach (Drawable drawable in this.Children!) {
                drawable.Dispose();
            }

        this.Children?.Clear();

        base.Dispose();
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        if (this.SortDrawables) {
            this.ChildrenLock.EnterWriteLock();
            this.Children!.Sort(DrawableDrawComparer.Instance);

            this.SortDrawables = false;
            this.ChildrenLock.ExitWriteLock();
        }

        bool taken = this.ChildrenLock.TryEnterReadLock(1);
        
        for (int i = 0; i < this.Children!.Count; i++) {
            Drawable drawable = this.Children[i];

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (drawable == null)
                // ReSharper disable once HeuristicUnreachableCode
                continue;
            
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (drawable.Depth != drawable.DrawablesLastKnownDepth)
                this.SortDrawables = true;

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
        }

        if (taken)
            this.ChildrenLock.ExitReadLock();
    }
}