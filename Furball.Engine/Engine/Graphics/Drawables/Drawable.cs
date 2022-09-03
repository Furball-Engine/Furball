using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes.BezierPathTween;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Input.Events;
using Furball.Engine.Engine.Timing;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Helpers;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables; 

public enum OriginType {
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center,
    TopCenter,
    BottomCenter,
    LeftCenter,
    RightCenter
}
    
/// <summary>
/// A Basic Drawable
/// </summary>
public abstract class Drawable : IDisposable {
    /// <summary>
    ///     This is the real position of a drawable, ignoring whether it is inside of a CompositeDrawable or not
    /// </summary>
    public Vector2 RealPosition;
    /// <summary>
    ///     This is the real scale of a drawable, ignoring whether it is inside of a CompositeDrawable or not
    /// </summary>
    public Vector2 RealScale;
        
    // private Vector2 _position = Vector2.Zero;
    /// <summary>
    ///     Radius of the Circle (Used for Click detection and other hitboxes)
    /// </summary>
    public Bindable<float> CircleRadius = new(0f);
    /// <summary>
    ///     Is the Drawable Circular? (Used for Click detection and other hitboxes)
    /// </summary>
    public bool Circular = false;
    /// <summary>
    ///     Whether the drawable is able to be clicked
    /// </summary>
    public bool Clickable = true;
    /// <summary>
    ///     Unprocessed Color Override of the Drawable, if a White Texture gets drawn with a red override, voila its red
    /// </summary>
    public Color ColorOverride = Color.White;
    /// <summary>
    ///     Whether the drawable covers other drawables from being clicked
    /// </summary>
    public bool CoverClicks = true;
    /// <summary>
    ///     Whether the drawable covers other drawables from being hovered over
    /// </summary>
    public bool CoverHovers = true;
    /// <summary>
    ///     The draw depth of the Drawable, more number = further backwards
    /// </summary>
    public double Depth = 0f;
    public double DrawablesLastKnownDepth = 0f;
    /// <summary>
    ///     Whether the drawable is able to be hovered on
    /// </summary>
    public bool Hoverable = true;

    public Vector2 LastCalculatedOrigin = Vector2.Zero;
    /// <summary>
    ///     The render origin of the drawable
    /// </summary>
    public OriginType OriginType = OriginType.TopLeft;
    /// <summary>
    ///     The origin of the screen where 0,0 is
    /// </summary>
    public OriginType ScreenOriginType = OriginType.TopLeft;
    /// <summary>
    ///     Unprocessed Rotation of the Drawable in Radians
    ///     <remarks>
    ///         This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be
    ///         properly visible on all resolutions
    ///     </remarks>
    /// </summary>
    public float Rotation = 0f;
    /// <summary>
    /// The origin which around the drawable will rotation
    /// </summary>
    public Vector2 RotationOrigin;
    /// <summary>
    ///     Unprocessed Scale of the Drawable, new Vector(1, 1) draws the Drawable at full scale
    ///     <remarks>
    ///         This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be
    ///         properly visible on all resolutions
    ///     </remarks>
    /// </summary>
    public Vector2 Scale = Vector2.One;
    /// <summary>
    ///     Basic SpriteEffect, was provided by SpriteBatch so might aswell put it here
    /// </summary>
    public TextureFlip SpriteEffect = TextureFlip.None;

    public List<string> Tags = new();
    /// <summary>
    ///     What time does the Drawable go by? Used for Tweens
    /// </summary>
    public ITimeSource TimeSource = FurballGame.GameTimeSource;

    /// <summary>
    ///     The tooltip to display when hovering over the drawable
    /// </summary>
    public string ToolTip = string.Empty;
    /// <summary>
    ///     List of Tweens
    /// </summary>
    public List<Tween> Tweens = new();
    /// <summary>
    ///     Whether the drawable is visible or not
    /// </summary>
    public bool Visible = true;

    /// <summary>
    ///     Unprocessed Position where the Drawable is expected to be drawn
    ///     <remarks>
    ///         This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be
    ///         properly visible on all resolutions
    ///     </remarks>
    /// </summary>
    public Vector2 Position = Vector2.Zero;

    public RectangleF Rectangle     => new((this.Position     - this.RotationOrigin).ToPointF(), this.Size.ToSizeF());
    public RectangleF RealRectangle => new((this.RealPosition - this.RotationOrigin).ToPointF(), this.RealSize.ToSizeF());
    /// <summary>
    ///     Unprocessed Size of the Drawable in Pixels
    ///     <remarks>
    ///         This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be
    ///         properly visible on all resolutions
    ///     </remarks>
    /// </summary>
    public virtual Vector2 Size { get; } = new();
    /// <summary>
    ///     The real size of the drawable in pixels
    /// </summary>
    public Vector2 RealSize => this.Size / this.Scale * this.RealScale;
    /// <summary>
    ///     Shorter way of getting TimeSource.GetCurrentTime()
    /// </summary>
    public double DrawableTime => this.TimeSource.GetCurrentTime();
    /// <summary>
    ///     Whether a cursor is hovering over the drawable
    /// </summary>
    public bool IsHovered {
        get;
        private set;
    } = false;
    /// <summary>
    ///     Whether the drawable is being clicked
    /// </summary>
    public bool IsClicked {
        get;
        private set;
    } = false;
    /// <summary>
    ///     Is the Drawable being dragged?
    /// </summary>
    public bool IsDragging {
        get;
        private set;
    } = false;

    /// <summary>
    ///     Checks whether a point is inside of the drawable
    /// </summary>
    /// <param name="point">The point to check</param>
    /// <returns>Whether the point is inside the drawable</returns>
    public bool Contains(Vector2 point) {
        if (this.Circular)
            return Vector2.Distance(point, this.Position - this.RotationOrigin) < this.CircleRadius;

        return this.Rectangle.Contains(point.ToPointF());
    }
    public bool RealContains(Vector2 point) {
        if (this.Circular)
            return Vector2.Distance(point, this.RealPosition - this.RotationOrigin) < this.CircleRadius;

        return this.RealRectangle.Contains(point.ToPointF());
    }
    public void Hover(bool value) {
        if (value == this.IsHovered) return;

        if (value)
            this.OnHover?.Invoke(this, EventArgs.Empty);
        else
            this.OnHoverLost?.Invoke(this, EventArgs.Empty);

        this.IsHovered = value;
    }
    public void Click(bool value, MouseButtonEventArgs e) {
        if (value == this.IsClicked) return;

        if (value)
            this.OnClick?.Invoke(this, e);
        else
            this.OnClickUp?.Invoke(this, e);

        this.IsClicked = value;
    }
    public void DragState(bool value, MouseDragEventArgs point) {
        if (value == this.IsDragging) return;

        if (value)
            this.OnDragBegin?.Invoke(this, point);
        else
            this.OnDragEnd?.Invoke(this, point);

        this.IsDragging = value;
    }
    public void Drag(MouseDragEventArgs point) {
        if (!this.IsDragging) return;

        this.OnDrag?.Invoke(this, point);
    }
    /// <summary>
    ///     Called whenever a cursor hovers over the drawable
    /// </summary>
    public event EventHandler OnHover;
    /// <summary>
    ///     Called whenever a cursor moves off of the drawable
    /// </summary>
    public event EventHandler OnHoverLost;
    /// <summary>
    ///     Called when the drawable is clicked
    /// </summary>
    public event EventHandler<MouseButtonEventArgs> OnClick;
    /// <summary>
    ///     Called when the drawable is no longer being clicked
    /// </summary>
    public event EventHandler<MouseButtonEventArgs> OnClickUp;
    /// <summary>
    ///     Gets fired when the Drawable is first getting started to Drag
    /// </summary>
    public event EventHandler<MouseDragEventArgs> OnDragBegin;
    /// <summary>
    ///     Gets fired every Input Frame for the duration of the drag
    /// </summary>
    public event EventHandler<MouseDragEventArgs> OnDrag;
    /// <summary>
    ///     Gets Fired when the Dragging stops
    /// </summary>
    public event EventHandler<MouseDragEventArgs> OnDragEnd;

    public virtual void ClearEvents() {
        this.OnClick     = null;
        this.OnClickUp   = null;
        this.OnDrag      = null;
        this.OnDragEnd   = null;
        this.OnDragBegin = null;
        this.OnHover     = null;
        // this.OnMove      = null;
        this.OnHoverLost = null;
    }

    public virtual Vector2 CalculateOrigin() {
        return this.OriginType switch {
            OriginType.TopLeft      => Vector2.Zero,
            OriginType.TopRight     => new Vector2(this.Size.X,     0),
            OriginType.BottomLeft   => new Vector2(0,               this.Size.Y),
            OriginType.BottomRight  => new Vector2(this.Size.X,     this.Size.Y),
            OriginType.Center       => new Vector2(this.Size.X / 2, this.Size.Y / 2),
            OriginType.TopCenter    => new Vector2(this.Size.X / 2, 0),
            OriginType.BottomCenter => new Vector2(this.Size.X / 2, this.Size.Y),
            OriginType.LeftCenter   => new Vector2(0,               this.Size.Y / 2),
            OriginType.RightCenter  => new Vector2(this.Size.X,     this.Size.Y / 2),
            _                       => throw new ArgumentOutOfRangeException(nameof (this.OriginType), "That OriginType is unsupported.")
        };
    }

    public virtual void Dispose() {
        this.Tweens.Clear();
    }

    private bool _sortTweenScheduled = false;

    /// <summary>
    ///     Updates the pDrawables Tweens
    /// </summary>
    public void UpdateTweens() {
        this.Tweens.RemoveAll(tween => tween == null || tween.Terminated && !tween.KeepAlive);

        if (this._sortTweenScheduled) {
            this.Tweens.Sort((x, y) => (int)(x.StartTime - y.StartTime));
            this._sortTweenScheduled = false;
        }

        double currentTime = this.TimeSource.GetCurrentTime();

        for (int i = 0; i != this.Tweens.Count; i++) {
            Tween currentTween = this.Tweens[i];

            // ReSharper disable twice CompareOfFloatsByEqualityOperator
            if (currentTween.LastKnownStartTime != currentTween.StartTime || currentTween.LastKnownEndTime != currentTween.EndTime)
                this._sortTweenScheduled = true;

            currentTween.LastKnownStartTime = currentTween.StartTime;
            currentTween.LastKnownEndTime   = currentTween.EndTime;

            currentTween.Update(currentTime);

            if (!currentTween.Initiated)
                continue;

            switch (currentTween.TweenType) {
                case TweenType.Color:
                    if (currentTween is ColorTween colorTween)
                        this.ColorOverride = colorTween.GetCurrent();
                    break;
                case TweenType.Movement: {

                    if (currentTween is VectorTween vectorTween)
                        this.Position = vectorTween.GetCurrent();
                    break;
                }
                case TweenType.RotationOrigin: {

                    if (currentTween is VectorTween vectorTween)
                        this.RotationOrigin = vectorTween.GetCurrent();
                    break;
                }
                case TweenType.Path:

                    if (currentTween is PathTween pathTween)
                        this.Position = pathTween.GetCurrent();
                    break;
                case TweenType.Scale:

                    if (currentTween is VectorTween scaleTween)
                        this.Scale = scaleTween.GetCurrent();
                    break;
                case TweenType.Rotation:

                    if (currentTween is FloatTween rotationTween)
                        this.Rotation = rotationTween.GetCurrent();
                    break;
                case TweenType.Fade:

                    if (currentTween is FloatTween fadeTween)
                        this.ColorOverride.Af = fadeTween.GetCurrent();
                    break;
            }
        }
    }

    #region Tween Helpers

    public void FadeColor(Color color, int duration, Easing easing = Easing.None) {
        lock (this.Tweens) {
            this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Color);
        }

        this.Tweens.Add(
        new ColorTween(TweenType.Color, this.ColorOverride, color, this.TimeSource.GetCurrentTime(), this.TimeSource.GetCurrentTime() + duration, easing)
        );
    }

    public void FlashColor(Color color, int duration, Easing easing = Easing.None) {
        if (this.ColorOverride == color)
            return;

        lock (this.Tweens) {
            this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Color);
        }

        this.Tweens.Add(
        new ColorTween(TweenType.Color, color, this.ColorOverride, this.TimeSource.GetCurrentTime(), this.TimeSource.GetCurrentTime() + duration, easing)
        );
    }

    public void FadeIn(int duration, Easing easing = Easing.None) {
        if (this.ColorOverride.A == 255)
            return;

        lock (this.Tweens) {
            this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Color);
        }

        Color endColor = this.ColorOverride;
        endColor.A = 255;

        this.Tweens.Add(
        new ColorTween(TweenType.Color, this.ColorOverride, endColor, this.TimeSource.GetCurrentTime(), this.TimeSource.GetCurrentTime() + duration, easing)
        );
    }

    public void FadeInFromZero(int duration, Easing easing = Easing.None) {
        if (this.ColorOverride.A == 255)
            return;

        lock (this.Tweens) {
            this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Color);
        }

        Color startColor = this.ColorOverride;
        startColor.A = 0;

        Color endColor = this.ColorOverride;
        endColor.A = 255;

        this.Tweens.Add(
        new ColorTween(TweenType.Color, startColor, endColor, this.TimeSource.GetCurrentTime(), this.TimeSource.GetCurrentTime() + duration, easing)
        );
    }

    public void FadeOut(int duration, Easing easing = Easing.None) {
        if (this.ColorOverride.A == 0)
            return;

        lock (this.Tweens) {
            this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Color);
        }

        Color endColor = this.ColorOverride;
        endColor.A = 0;

        this.Tweens.Add(
        new ColorTween(TweenType.Color, this.ColorOverride, endColor, this.TimeSource.GetCurrentTime(), this.TimeSource.GetCurrentTime() + duration, easing)
        );
    }

    public void FadeOutFromOne(int duration, Easing easing = Easing.None) {
        if (this.ColorOverride.A == 0)
            return;

        lock (this.Tweens) {
            this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Color);
        }

        Color startColor = this.ColorOverride;
        startColor.A = 255;

        Color endColor = this.ColorOverride;
        endColor.A = 0;

        this.Tweens.Add(
        new ColorTween(TweenType.Color, this.ColorOverride, endColor, this.TimeSource.GetCurrentTime(), this.TimeSource.GetCurrentTime() + duration, easing)
        );
    }

    public void MoveToRelative(Vector2 move, int duration = 0, Easing easing = Easing.None) {
        this.MoveTo(this.Position + move, duration, easing);
    }

    public void MoveTo(Vector2 dest, int duration = 0, Easing easing = Easing.None) {
        if (this.Position == dest)
            return;

        lock (this.Tweens) {
            this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Movement);
        }

        this.Tweens.Add(
        new VectorTween(TweenType.Movement, this.Position, dest, this.TimeSource.GetCurrentTime(), this.TimeSource.GetCurrentTime() + duration, easing)
        );
    }

    public void RotateRelative(float radians, int duration, Easing easing = Easing.None) {
        lock (this.Tweens) {
            this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Rotation);
        }

        this.Tweens.Add(
        new FloatTween(
        TweenType.Rotation,
        this.Rotation,
        this.Rotation + radians,
        this.TimeSource.GetCurrentTime(),
        this.TimeSource.GetCurrentTime() + duration,
        easing
        )
        );
    }

    public void Rotate(float radians, int duration, Easing easing = Easing.None) {
        lock (this.Tweens) {
            this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Rotation);
        }

        this.Tweens.Add(
        new FloatTween(TweenType.Rotation, this.Rotation, radians, this.TimeSource.GetCurrentTime(), this.TimeSource.GetCurrentTime() + duration, easing)
        );
    }
    //dumb name because Scale already taken
    public void DirectScale(Vector2 newScale, int duration, Easing easing = Easing.None) {
        if (this.Scale == newScale)
            return;

        lock (this.Tweens) {
            this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Scale);
        }

        this.Tweens.Add(
        new VectorTween(TweenType.Scale, this.Scale, newScale, this.TimeSource.GetCurrentTime(), this.TimeSource.GetCurrentTime() + duration, easing)
        );
    }

    public void ScaleRelative(Vector2 increase, int duration, Easing easing = Easing.None) {
        lock (this.Tweens) {
            this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Scale);
        }

        this.Tweens.Add(
        new VectorTween(TweenType.Scale, this.Scale, this.Scale + increase, this.TimeSource.GetCurrentTime(), this.TimeSource.GetCurrentTime() + duration, easing)
        );
    }

    #endregion

    /// <summary>
    ///     Method for Drawing the Drawable,
    ///     Gets called every Draw
    /// </summary>
    /// <param name="time">How much time has passed since last Draw</param>
    /// <param name="batch">Already Started SpriteBatch</param>
    /// <param name="args">The DrawableManagerArgs variable, which contains the arguments to pass into your batch.Draw call</param>
    public virtual void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {}
    /// <summary>
    ///     If your Drawable needs updating, this is the Method to Override,
    ///     Gets Called every Update
    /// </summary>
    /// <param name="time">How much time has passed since last Update</param>
    public virtual void Update(double time) {}

    ~Drawable() {
        DisposeQueue.Enqueue(this);
    }
}