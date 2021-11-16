using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie.Graphics;
using Color=Furball.Vixie.Graphics.Color;

namespace Furball.Engine.Engine.Graphics.Drawables.Managers {
    public class DrawableManager : UnmanagedDrawable {
        private List<BaseDrawable> _totalDrawables = new();

        private List<ManagedDrawable>   _managedDrawables   = new();
        private List<UnmanagedDrawable> _unmanagedDrawables = new();

        public IReadOnlyList<BaseDrawable> Drawables => this._totalDrawables.AsReadOnly();

        public int CountManaged { get; private set; }
        public int CountUnmanaged { get; private set; }

        public static object                StatLock         = new ();
        public static List<DrawableManager> DrawableManagers = new();
        public static int                   Instances        = 0;

        public DrawableManager() {
            lock (StatLock) {
                Instances++;
                DrawableManagers.Add(this);
            }
        }

        private bool _sortDrawables = false;
        
        public override void Draw(double time, DrawableBatch drawableBatch, DrawableManagerArgs _ = null) {
            if (this._sortDrawables) {
                this._totalDrawables     = this._totalDrawables.OrderByDescending(o => o.Depth).ToList();
                this._managedDrawables   = this._managedDrawables.OrderByDescending(o => o.Depth).ToList();
                this._unmanagedDrawables = this._unmanagedDrawables.OrderByDescending(o => o.Depth).ToList();

                this._sortDrawables = false;
            }
            
            drawableBatch.Begin();

            int tempCount = this._managedDrawables.Count;
            this.CountManaged = tempCount;

            for (int i = 0; i < tempCount; i++) {
                ManagedDrawable currentDrawable = this._managedDrawables[i];
                if (!currentDrawable.Visible) continue;

                if (Math.Abs(currentDrawable.DrawablesLastKnownDepth - currentDrawable.Depth) > 0.01d) {
                    this._sortDrawables = true;

                    currentDrawable.DrawablesLastKnownDepth = currentDrawable.Depth;
                }
                
                Vector2 origin = CalculateNewOriginPosition(currentDrawable);
                currentDrawable.LastCalculatedOrigin = origin;
                
                //TODO:
                //Potentially give ScaledPosition and ScaledScale
                //Which would just be:
                /*
                    args.Position *= FurballGame.VerticalRatio;
                    args.Scale    *= FurballGame.VerticalRatio;
                 */
                //Since literally every single drawable does this, might aswell make it easier,
                //also given that drawablemanagers will have to scale on their own width and height instead of screen width and height,
                //itll def make developing easier
                
                DrawableManagerArgs args = new() {
                    Color      = currentDrawable.ColorOverride,
                    Effects    = currentDrawable.SpriteEffect,
                    Position   = currentDrawable.Position - origin,
                    Rotation   = currentDrawable.Rotation,
                    Scale      = currentDrawable.Scale
                };

                Rectangle rect = new(
                    (args.Position - origin).ToPoint(),
                    new Size((int)Math.Ceiling(currentDrawable.Size.X * args.Scale.X), (int)Math.Ceiling(currentDrawable.Size.Y * args.Scale.Y))
                );

                if(rect.IntersectsWith(FurballGame.DisplayRect))
                    currentDrawable.Draw(time, drawableBatch, args);
            }

            drawableBatch.End();

            tempCount           = this._unmanagedDrawables.Count;
            this.CountUnmanaged = tempCount;

            for (int i = 0; i < tempCount; i++) {
                UnmanagedDrawable currentDrawable = this._unmanagedDrawables[i];
                if (!currentDrawable.Visible) continue;

                if (Math.Abs(currentDrawable.DrawablesLastKnownDepth - currentDrawable.Depth) > 0.01d) {
                    this._sortDrawables = true;

                    currentDrawable.DrawablesLastKnownDepth = currentDrawable.Depth;
                }
                
                Vector2 origin = CalculateNewOriginPosition(currentDrawable);
                currentDrawable.LastCalculatedOrigin = origin;

                DrawableManagerArgs args = new() {
                    Color      = currentDrawable.ColorOverride,
                    Effects    = currentDrawable.SpriteEffect,
                    Position   = currentDrawable.Position - origin,
                    Rotation   = currentDrawable.Rotation,
                    Scale      = currentDrawable.Scale
                };

                currentDrawable.Draw(time, drawableBatch, args);
            }
        }

        private TextureRenderTarget _target2D;
        public TextureRenderTarget DrawRenderTarget2D(double time, DrawableBatch batch, DrawableManagerArgs _ = null) {
            if(this._target2D?.TargetWidth != FurballGame.WindowWidth || this._target2D?.TargetHeight != FurballGame.WindowHeight)
                this._target2D = new TextureRenderTarget((uint) FurballGame.WindowWidth, (uint) FurballGame.WindowHeight);
            
            this._target2D.Bind();

            FurballGame.Instance.GraphicsDevice.GlClearColor(Color.Transparent);
            FurballGame.Instance.GraphicsDevice.GlClear();

            this.Draw(time, batch, _);

            this._target2D.Unbind();

            return this._target2D;
        }

        private static Vector2 CalculateNewOriginPosition(BaseDrawable drawable) {
            return drawable.OriginType switch {
                OriginType.TopLeft      => Vector2.Zero,
                OriginType.TopRight     => new Vector2(drawable.Size.X,     0),
                OriginType.BottomLeft   => new Vector2(0,                   drawable.Size.Y),
                OriginType.BottomRight  => new Vector2(drawable.Size.X,     drawable.Size.Y),
                OriginType.Center       => new Vector2(drawable.Size.X / 2, drawable.Size.Y / 2),
                OriginType.TopCenter    => new Vector2(drawable.Size.X / 2, 0),
                OriginType.BottomCenter => new Vector2(drawable.Size.X / 2, drawable.Size.Y),
                OriginType.LeftCenter   => new Vector2(0, drawable.Size.Y / 2),
                OriginType.RightCenter  => new Vector2(drawable.Size.X, drawable.Size.Y / 2),
                _                       => throw new ArgumentOutOfRangeException(nameof (drawable.OriginType), "That OriginType is unsupported.")
            };
        }

        public override void Update(double time) {
            int tempCount = this._managedDrawables.Count;
            for (int i = 0; i < tempCount; i++) {
                ManagedDrawable currentDrawable = this._managedDrawables[i];

                currentDrawable.UpdateTweens();
                currentDrawable.Update(time);
            }

            tempCount = this._unmanagedDrawables.Count;
            for (int i = 0; i < tempCount; i++) {
                UnmanagedDrawable currentDrawable = this._unmanagedDrawables[i];

                currentDrawable.UpdateTweens();
                currentDrawable.Update(time);
            }
        }

        public void Add(BaseDrawable drawable) {
            this._totalDrawables.Add(drawable);

            switch (drawable) {
                case ManagedDrawable managedDrawable:
                    this._managedDrawables.Add(managedDrawable);
                    break;
                case UnmanagedDrawable unmanagedDrawable:
                    this._unmanagedDrawables.Add(unmanagedDrawable);
                    break;
            }

            this._totalDrawables     = this._totalDrawables.OrderByDescending(o => o.Depth).ToList();
            this._managedDrawables   = this._managedDrawables.OrderByDescending(o => o.Depth).ToList();
            this._unmanagedDrawables = this._unmanagedDrawables.OrderByDescending(o => o.Depth).ToList();
        }

        public void Add(List<BaseDrawable> drawables) {
            this._totalDrawables.AddRange(drawables);

            foreach (BaseDrawable drawable in drawables)
                switch (drawable) {
                    case ManagedDrawable managedDrawable:
                        this._managedDrawables.Add(managedDrawable);
                        break;
                    case UnmanagedDrawable unmanagedDrawable:
                        this._unmanagedDrawables.Add(unmanagedDrawable);
                        break;
                }

            this._totalDrawables     = this._totalDrawables.OrderByDescending(o => o.Depth).ToList();
            this._managedDrawables   = this._managedDrawables.OrderByDescending(o => o.Depth).ToList();
            this._unmanagedDrawables = this._unmanagedDrawables.OrderByDescending(o => o.Depth).ToList();
        }

        public void Add(params BaseDrawable[] drawables) {
            this._totalDrawables.AddRange(drawables);

            foreach (BaseDrawable drawable in drawables)
                switch (drawable) {
                    case ManagedDrawable managedDrawable:
                        this._managedDrawables.Add(managedDrawable);
                        break;
                    case UnmanagedDrawable unmanagedDrawable:
                        this._unmanagedDrawables.Add(unmanagedDrawable);
                        break;
                }

            this._totalDrawables     = this._totalDrawables.OrderByDescending(o => o.Depth).ToList();
            this._managedDrawables   = this._managedDrawables.OrderByDescending(o => o.Depth).ToList();
            this._unmanagedDrawables = this._unmanagedDrawables.OrderByDescending(o => o.Depth).ToList();
        }
        public void Remove(BaseDrawable drawable) {
            this._totalDrawables.Remove(drawable);

            switch (drawable) {
                case ManagedDrawable managedDrawable:
                    managedDrawable.Dispose();
                    this._managedDrawables.Remove(managedDrawable);
                    break;
                case UnmanagedDrawable unmanagedDrawable:
                    unmanagedDrawable.Dispose();
                    this._unmanagedDrawables.Remove(unmanagedDrawable);
                    break;
            }
        }

        public override void Dispose() {
            foreach (BaseDrawable drawable in this._totalDrawables)
                drawable.Dispose();

            lock (StatLock) {
                Instances--;
                DrawableManagers.Remove(this);
            }

            base.Dispose();
        }
    }
}
