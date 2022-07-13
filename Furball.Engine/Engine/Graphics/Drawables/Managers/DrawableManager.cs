using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie;
using Furball.Vixie.Backends.Shared;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables.Managers {
    public class DrawableManager : Drawable {
        private List<Drawable> _drawables = new();

        public IReadOnlyList<Drawable> Drawables => this._drawables.AsReadOnly();

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

        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs _ = null) {
            if (this._sortDrawables) {
                this._drawables = this._drawables.OrderByDescending(o => o.Depth).ToList();

                this._sortDrawables = false;
            }

            bool unmanaged = !batch.Begun;

            if (unmanaged)
                batch.Begin();

            int tempCount = this._drawables.Count;
            this.CountManaged = tempCount;

            for (int i = 0; i < tempCount; i++) {
                Drawable drawable = this._drawables[i];

                drawable.LastCalculatedOrigin = drawable.CalculateOrigin();
                drawable.RealPosition         = drawable.Position - drawable.LastCalculatedOrigin;

                if (!drawable.Visible) continue;

                if (Math.Abs(drawable.DrawablesLastKnownDepth - drawable.Depth) > 0.01d) {
                    this._sortDrawables = true;

                    drawable.DrawablesLastKnownDepth = drawable.Depth;
                }
                Vector2 origin = drawable.CalculateOrigin();
                drawable.LastCalculatedOrigin = origin;
                
                //TODO: Potentially give ScaledPosition and ScaledScale
                //Which would just be:
                /*
                    args.Position *= FurballGame.VerticalRatio;
                    args.Scale    *= FurballGame.VerticalRatio;
                 */
                //Since literally every single drawable does this, might aswell make it easier,
                //also given that drawablemanagers will have to scale on their own width and height instead of screen width and height,
                //itll def make developing easier


                this._args.Color    = drawable.ColorOverride;
                this._args.Effects  = drawable.SpriteEffect;
                this._args.Position = drawable.RealPosition;
                this._args.Rotation = drawable.Rotation;
                this._args.Scale    = drawable.RealScale = drawable.Scale;

                Rectangle rect = new(
                (this._args.Position - origin).ToPoint(),
                new Size((int)Math.Ceiling(drawable.Size.X * this._args.Scale.X), (int)Math.Ceiling(drawable.Size.Y * this._args.Scale.Y))
                );

                if (rect.IntersectsWith(FurballGame.DisplayRect)) {
                    drawable.Draw(time, batch, this._args);
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

            if (unmanaged)
                batch.End();
        }

        private          TextureRenderTarget _target2D;
        private readonly DrawableManagerArgs _args = new();
        public TextureRenderTarget DrawRenderTarget2D(double time, DrawableBatch batch, DrawableManagerArgs _ = null) {
            if(this._target2D?.Size.X != FurballGame.RealWindowWidth || this._target2D?.Size.Y != FurballGame.RealWindowHeight)
                this._target2D = Resources.CreateTextureRenderTarget((uint) FurballGame.RealWindowWidth, (uint) FurballGame.RealWindowHeight);
            
            this._target2D.Bind();

            GraphicsBackend.Current.Clear();

            this.Draw(time, batch, _);

            this._target2D.Unbind();

            return this._target2D;
        }

        public override void Update(double time) {
            int tempCount = this._drawables.Count;
            for (int i = 0; i < tempCount; i++) {
                Drawable currentDrawable = this._drawables[i];

                currentDrawable.UpdateTweens();
                currentDrawable.Update(time);
            }
        }

        public void Add(Drawable drawable) {
            this._drawables.Add(drawable);
            this._drawables = this._drawables.OrderByDescending(o => o.Depth).ToList();
        }

        public void Add(List<Drawable> drawables) {
            this._drawables.AddRange(drawables);
            this._drawables = this._drawables.OrderByDescending(o => o.Depth).ToList();
        }

        public void Add(params Drawable[] drawables) {
            this._drawables.AddRange(drawables);
            this._drawables = this._drawables.OrderByDescending(o => o.Depth).ToList();
        }
        public void Remove(Drawable drawable) {
            this._drawables.Remove(drawable);
        }

        public override void Dispose() {
            foreach (Drawable drawable in this._drawables)
                drawable.Dispose();

            lock (StatLock) {
                Instances--;
                DrawableManagers.Remove(this);
            }

            base.Dispose();
        }
    }
}
