#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie;
using Furball.Vixie.Helpers;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables.Managers; 

public class DrawableManager : IDisposable {
    private List<Drawable> _drawables = new();

    public IReadOnlyList<Drawable> Drawables => this._drawables.AsReadOnly();

    public int Count { get; private set; }

    public static object                StatLock         = new ();
    public static List<DrawableManager> DrawableManagers = new();
    public static int                   Instances        = 0;

    public event EventHandler<Vector2>? OnScalingRelayoutNeeded; 

    public DrawableManager() {
        lock (StatLock) {
            Instances++;
            DrawableManagers.Add(this);
        }
    }

    public bool Visible = true;

    private bool _sortDrawables = true;

    private bool _effectedByScaling = false;
    public bool EffectedByScaling {
        get => this._effectedByScaling;
        set {
            this._effectedByScaling = value;
            if (this.EffectedByScaling)
                this.OnScalingRelayoutNeeded?.Invoke(this, this.Size);
        }
    }
    
    public Vector2 Position          = Vector2.Zero;

    private Vector2 _size = new(FurballGame.WindowWidth, FurballGame.DEFAULT_WINDOW_HEIGHT);
    public Vector2 Size {
        get => this._size;
        set {
            this._size = value;

            if (this.EffectedByScaling)
                this.OnScalingRelayoutNeeded?.Invoke(this, value);
        }
    }
    
    public void Draw(double time, DrawableBatch batch) {
        if (this._sortDrawables) {
            this._drawables.Sort(DrawableDrawComparer.Instance);

            this._sortDrawables = false;
        }

        int tempCount = this._drawables.Count;
        this.Count = tempCount;

        if (this.EffectedByScaling)
            batch.ScissorPush(this, new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)this.Size.X, (int)this.Size.Y));
        
        for (int i = 0; i < tempCount; i++) {
            Drawable drawable = this._drawables[i];

            drawable.LastCalculatedOrigin = drawable.CalculateOrigin();
            drawable.RealPosition         = drawable.Position;
            drawable.RealScale            = drawable.Scale;

            drawable.RealPosition -= drawable.ScreenOriginType switch {
                OriginType.TopLeft     => drawable.LastCalculatedOrigin,
                OriginType.TopRight    => new Vector2(-drawable.LastCalculatedOrigin.X, drawable.LastCalculatedOrigin.Y),
                OriginType.BottomLeft  => new Vector2(drawable.LastCalculatedOrigin.X,  -drawable.LastCalculatedOrigin.Y),
                OriginType.BottomRight => new Vector2(-drawable.LastCalculatedOrigin.X, -drawable.LastCalculatedOrigin.Y),
                _                      => throw new ArgumentOutOfRangeException()
            };

            Vector2 scaledSize = this.Size / (this.Size.Y / FurballGame.WindowHeight);
            
            if (this.EffectedByScaling)
                drawable.RealPosition = drawable.ScreenOriginType switch {
                    OriginType.TopLeft     => drawable.RealPosition,
                    OriginType.TopRight    => new Vector2(scaledSize.X - drawable.RealPosition.X, drawable.RealPosition.Y),
                    OriginType.BottomLeft  => new Vector2(drawable.RealPosition.X,                scaledSize.Y - drawable.RealPosition.Y),
                    OriginType.BottomRight => new Vector2(scaledSize.X - drawable.RealPosition.X, scaledSize.Y - drawable.RealPosition.Y),
                    _                      => throw new ArgumentOutOfRangeException()
                };
            else
                drawable.RealPosition = drawable.ScreenOriginType switch {
                    OriginType.TopLeft     => drawable.RealPosition,
                    OriginType.TopRight    => new Vector2(FurballGame.WindowWidth - drawable.RealPosition.X, drawable.RealPosition.Y),
                    OriginType.BottomLeft  => new Vector2(drawable.RealPosition.X,                           FurballGame.WindowHeight - drawable.RealPosition.Y),
                    OriginType.BottomRight => new Vector2(FurballGame.WindowWidth - drawable.RealPosition.X, FurballGame.WindowHeight - drawable.RealPosition.Y),
                    _                      => throw new ArgumentOutOfRangeException()
                };

            if (!drawable.Visible) continue;

            if (Math.Abs(drawable.DrawablesLastKnownDepth - drawable.Depth) > 0.01d) {
                this._sortDrawables = true;

                drawable.DrawablesLastKnownDepth = drawable.Depth;
            }

            
            if (this.EffectedByScaling) {
                float scaling = this.Size.Y / FurballGame.WindowHeight;
                
                drawable.RealPosition *= scaling;
                drawable.RealPosition += this.Position;
                drawable.RealScale    *= scaling;
            }

            this._args.Color    = drawable.ColorOverride;
            this._args.Effects  = drawable.SpriteEffect;
            this._args.Position = drawable.RealPosition;
            this._args.Rotation = drawable.Rotation;
            this._args.Scale    = drawable.RealScale;

            RectangleF rect = new(drawable.RealPosition.ToPointF(), new SizeF(drawable.RealSize.X, drawable.RealSize.Y));

            if ((!this.EffectedByScaling && rect.IntersectsWith(FurballGame.DisplayRect)) || (this.EffectedByScaling && rect.IntersectsWith(new RectangleF(this.Position.ToPointF(), this.Size.ToSizeF())))) {
                int scissorStackItems = batch.ScissorStackItemCount;
                
                drawable.Draw(time, batch, this._args);
 
                System.Diagnostics.Debug.Assert(scissorStackItems == batch.ScissorStackItemCount, $"Scissor stack is unbalanced after {drawable}.Draw was called!");
                if (FurballGame.DrawInputOverlay)
                    switch (drawable.Clickable) {
                        case false when drawable.CoverClicks:
                            batch.DrawRectangle(
                            new Vector2(drawable.RealRectangle.X,     drawable.RealRectangle.Y),
                            new Vector2(drawable.RealRectangle.Width, drawable.RealRectangle.Height),
                            1,
                            Color.Red
                            );
                            break;
                        case true when drawable.CoverClicks:
                            batch.DrawRectangle(
                            new Vector2(drawable.RealRectangle.X,     drawable.RealRectangle.Y),
                            new Vector2(drawable.RealRectangle.Width, drawable.RealRectangle.Height),
                            1,
                            Color.Green
                            );
                            break;
                    }
            }
        }

        if(this.EffectedByScaling)
            batch.ScissorPop(this);
    }

    private          RenderTarget? _target2D;
    private readonly DrawableManagerArgs  _args = new();
    public RenderTarget DrawRenderTarget2D(double time, DrawableBatch batch) {
        if((int)(this._target2D?.Size.X ?? 0) != FurballGame.RealWindowWidth || (int)(this._target2D?.Size.Y ?? 0) != FurballGame.RealWindowHeight) {
            this._target2D?.Dispose();
            this._target2D = new RenderTarget((uint)FurballGame.RealWindowWidth, (uint)FurballGame.RealWindowHeight);
        }
            
        this._target2D!.Bind();

        GraphicsBackend.Current.Clear();

        this.Draw(time, batch);

        this._target2D.Unbind();

        return this._target2D;
    }

    public virtual void Update(double time) {
        int tempCount = this._drawables.Count;
        for (int i = 0; i < tempCount; i++) {
            Drawable currentDrawable = this._drawables[i];

            currentDrawable.UpdateTweens();
            currentDrawable.Update(time);
        }
    }

    public void Add(Drawable drawable) {
        this._drawables.Add(drawable);
        this._sortDrawables = true;
    }

    public void Add(List<Drawable> drawables) {
        this._drawables.AddRange(drawables);
        this._sortDrawables = true;
    }

    public void Add(params Drawable[] drawables) {
        this._drawables.AddRange(drawables);
        this._sortDrawables = true;
    }
    public void Remove(Drawable drawable) {
        this._drawables.Remove(drawable);
    }

    private bool _isDisposed = false;
    public void Dispose() {
        if (this._isDisposed)
            return;

        this._isDisposed = true;
        
        foreach (Drawable drawable in this._drawables)
            drawable.Dispose();

        this._drawables.Clear();
        
        lock (StatLock) {
            Instances--;
            DrawableManagers.Remove(this);
        }
    }

    ~DrawableManager() {
        DisposeQueue.Enqueue(this);
    }
}