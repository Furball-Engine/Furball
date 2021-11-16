using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes.BezierPathTween;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Timing;
using Furball.Vixie.Graphics;
using Color=Furball.Vixie.Graphics.Color;

namespace Furball.Engine.Engine.Graphics.Drawables {
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
    /// Base Class for Managed and Unmanaged Drawable
    /// </summary>
    public abstract class BaseDrawable {
        private Vector2 _position = Vector2.Zero;

        public List<string> Tags = new();

        /// <summary>
        ///     The tooltip to display when hovering over the drawable
        /// </summary>
        public string ToolTip = string.Empty;
        
        /// <summary>
        /// Unprocessed Position where the Drawable is expected to be drawn
        /// <remarks>This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be properly visible on all resolutions</remarks>
        /// </summary>
        public Vector2 Position {
            get => this._position;
            set {
                if (value == this._position)
                    return;
                this.OnMove?.Invoke(this, value);
                this._position = value;
            }
        }

        public Rectangle Rectangle => new((this.Position - this.LastCalculatedOrigin).ToPoint(), this.Size.ToSize());

        /// <summary>
        ///     Checks whether a point is inside of the drawable
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>Whether the point is inside the drawable</returns>
        public bool Contains(Point point) {
            if (this.Circular)
                return Vector2.Distance(point.ToVector2(), this._position - this.LastCalculatedOrigin) < this.CircleRadius;

            return this.Rectangle.Contains(point);
        }

        public Vector2 LastCalculatedOrigin = Vector2.Zero;
        public event EventHandler<Vector2> OnMove;
        /// <summary>
        /// Unprocessed Size of the Drawable in Pixels
        /// <remarks>This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be properly visible on all resolutions</remarks>
        /// </summary>
        public virtual Vector2 Size { get; } = new();
        /// <summary>
        /// Is the Drawable Circular? (Used for Click detection and other hitboxes)
        /// </summary>
        public bool Circular = false;
        /// <summary>
        /// Radius of the Circle (Used for Click detection and other hitboxes)
        /// </summary>
        public float CircleRadius = 0f;
        /// <summary>
        /// Unprocessed Color Override of the Drawable, if a White Texture gets drawn with a red override, voila its red
        /// <remarks>This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be properly visible on all resolutions</remarks>
        /// </summary>
        public Color ColorOverride = Color.White;
        /// <summary>
        /// Unprocessed Rotation of the Drawable in Radians
        /// <remarks>This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be properly visible on all resolutions</remarks>
        /// </summary>
        public float Rotation = 0f;
        /// <summary>
        /// Unprocessed Scale of the Drawable, new Vector(1, 1) draws the Drawable at full scale
        /// <remarks>This variable does not get changed as the DrawableManager translates the Drawable to be Scaled to be properly visible on all resolutions</remarks>
        /// </summary>
        public Vector2 Scale = Vector2.One;
        /// <summary>
        /// Basic SpriteEffect, was provided by SpriteBatch so might aswell put it here
        /// </summary>
        public TextureFlip SpriteEffect = TextureFlip.None;
        /// <summary>
        /// What time does the Drawable go by? Used for Tweens
        /// </summary>
        public ITimeSource TimeSource = FurballGame.GameTimeSource;
        /// <summary>
        /// Shorter way of getting TimeSource.GetCurrentTime()
        /// </summary>
        public int DrawableTime => this.TimeSource.GetCurrentTime();
        /// <summary>
        /// The draw depth of the Drawable
        /// </summary>
        public double Depth = 0f;
        public double DrawablesLastKnownDepth = 0f;
        /// <summary>
        /// List of Tweens
        /// </summary>
        public List<Tween> Tweens = new();
        /// <summary>
        /// The position of the Origin to render at
        /// </summary>
        public OriginType OriginType = OriginType.TopLeft;
        /// <summary>
        /// Whether a cursor is hovering over the drawable
        /// </summary>
        public bool IsHovered {
            get;
            private set;
        } = false;
        public void Hover(bool value) {
            if (value == this.IsHovered) return;

            if (value)
                this.OnHover?.Invoke(this, EventArgs.Empty);
            else
                this.OnHoverLost?.Invoke(this, EventArgs.Empty);

            this.IsHovered = value;
        }
        /// <summary>
        /// Whether the drawable is being clicked
        /// </summary>
        public bool IsClicked {
            get;
            private set;
        } = false;
        public void Click(bool value, Point point) {
            if (value == this.IsClicked) return;

            if (value)
                this.OnClick?.Invoke(this, point);
            else
                this.OnClickUp?.Invoke(this, point);

            this.IsClicked = value;
        }
        /// <summary>
        /// Is the Drawable being dragged?
        /// </summary>
        public bool IsDragging {
            get;
            private set;
        } = false;
        public void DragState(bool value, Point point) {
            if (value == this.IsDragging) return;

            if (value)
                this.OnDragBegin?.Invoke(this, point);
            else
                this.OnDragEnd?.Invoke(this, point);

            this.IsDragging = value;
        }
        public void Drag(Point point) {
            if (!this.IsDragging) return;

            this.OnDrag?.Invoke(this, point);
        }
        /// <summary>
        /// Whether the drawable is able to be clicked
        /// </summary>
        public bool Clickable = true;
        /// <summary>
        /// Whether the drawable covers other drawables from being clicked
        /// </summary>
        public bool CoverClicks = true;
        /// <summary>
        /// Whether the drawable is able to be hovered on
        /// </summary>
        public bool Hoverable = true;
        /// <summary>
        /// Whether the drawable covers other drawables from being hovered over
        /// </summary>
        public bool CoverHovers = true;
        /// <summary>
        /// Whether the drawable is visible or not
        /// </summary>
        public bool Visible = true;
        /// <summary>
        /// Called whenever a cursor hovers over the drawable
        /// </summary>
        public event EventHandler OnHover;
        /// <summary>
        /// Called whenever a cursor moves off of the drawable
        /// </summary>
        public event EventHandler OnHoverLost;
        /// <summary>
        /// Called when the drawable is clicked
        /// </summary>
        public event EventHandler<Point> OnClick;
        /// <summary>
        /// Called when the drawable is no longer being clicked
        /// </summary>
        public event EventHandler<Point> OnClickUp;
        /// <summary>
        /// Gets fired when the Drawable is first getting started to Drag
        /// </summary>
        public event EventHandler<Point> OnDragBegin;
        /// <summary>
        /// Gets fired every Input Frame for the duration of the drag
        /// </summary>
        public event EventHandler<Point> OnDrag;
        /// <summary>
        /// Gets Fired when the Dragging stops
        /// </summary>
        public event EventHandler<Point> OnDragEnd;

        public virtual void ClearEvents() {
            this.OnClick     = null;
            this.OnClickUp   = null;
            this.OnDrag      = null;
            this.OnDragEnd   = null;
            this.OnDragBegin = null;
            this.OnHover     = null;
            this.OnMove      = null;
            this.OnHoverLost = null;
        }

        public virtual void Dispose() {
            this.Tweens.Clear();
        }
        
        /// <summary>
        /// Updates the pDrawables Tweens
        /// </summary>
        public void UpdateTweens() {
            this.Tweens.RemoveAll(tween => tween.Terminated && !tween.KeepAlive);

            var sortedTweens = this.Tweens.OrderBy(tween => tween.StartTime).ToList();

            for (int i = 0; i != sortedTweens.Count; i++) {
                Tween currentTween = sortedTweens[i];

                currentTween.Update(this.TimeSource.GetCurrentTime());

                if(!currentTween.Initiated)
                    continue;

                switch (currentTween.TweenType) {
                    case TweenType.Color:
                        ColorTween colorTween = currentTween as ColorTween;

                        if (colorTween != null)
                            this.ColorOverride = colorTween.GetCurrent();
                        break;
                    case TweenType.Movement:
                        VectorTween vectorTween = currentTween as VectorTween;

                        if (vectorTween != null)
                            this.Position = vectorTween.GetCurrent();
                        break;
                    case TweenType.Path:
                        PathTween pathTween = currentTween as PathTween;

                        if (pathTween != null)
                            this.Position = pathTween.GetCurrent();
                        break;
                    case TweenType.Scale:
                        VectorTween scaleTween = currentTween as VectorTween;

                        if (scaleTween != null)
                            this.Scale = scaleTween.GetCurrent();
                        break;
                    case TweenType.Rotation:
                        FloatTween rotationTween = currentTween as FloatTween;

                        if (rotationTween != null)
                            this.Rotation = rotationTween.GetCurrent();
                        break;
                    case TweenType.Fade:
                        FloatTween fadeTween = currentTween as FloatTween;

                        if (fadeTween != null)
                            this.ColorOverride.A = (byte)(255 * fadeTween.GetCurrent());
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
            new ColorTween(
                    TweenType.Color,
                    this.ColorOverride,
                    color,
                    this.TimeSource.GetCurrentTime(),
                    this.TimeSource.GetCurrentTime() + duration,
                    easing
                )
            );
        }

        public void FlashColor(Color color, int duration, Easing easing = Easing.None) {
            if (this.ColorOverride == color)
                return;

            lock (this.Tweens) {
                this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Color);
            }

            this.Tweens.Add(
            new ColorTween(
                    TweenType.Color,
                    color,
                    this.ColorOverride,
                    this.TimeSource.GetCurrentTime(),
                    this.TimeSource.GetCurrentTime() + duration,
                    easing
                )
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
            new ColorTween(
                    TweenType.Color,
                    this.ColorOverride,
                    endColor,
                    this.TimeSource.GetCurrentTime(),
                    this.TimeSource.GetCurrentTime() + duration,
                    easing
                )
            );
        }

        public void FadeInFromZero(int duration, Easing easing = Easing.None) {
            if (this.ColorOverride.A == 0)
                return;

            lock (this.Tweens) {
                this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Color);
            }

            Color startColor = this.ColorOverride;
            startColor.A = 0;

            Color endColor = this.ColorOverride;
            endColor.A = 255;

            this.Tweens.Add(
                    new ColorTween(
                    TweenType.Color,
                    startColor,
                    endColor,
                    this.TimeSource.GetCurrentTime(),
                    this.TimeSource.GetCurrentTime() + duration,
                    easing
                )
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
                    new ColorTween(
                    TweenType.Color,
                    this.ColorOverride,
                    endColor,
                    this.TimeSource.GetCurrentTime(),
                    this.TimeSource.GetCurrentTime() + duration,
                    easing
                )
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
                    new ColorTween(
                    TweenType.Color,
                    this.ColorOverride,
                    endColor,
                    this.TimeSource.GetCurrentTime(),
                    this.TimeSource.GetCurrentTime() + duration,
                    easing
                )
            );
        }

        public void MoveTo(Vector2 dest) {
            this.MoveTo(dest, 0);
        }

        public void MoveToRelative(Vector2 move) {
            this.MoveTo(this.Position + move, 0);
        }

        public void MoveToRelative(Vector2 move, int duration, Easing easing = Easing.None) {
            this.MoveTo(this.Position + move, duration, easing);
        }

        public void MoveTo(Vector2 dest, int duration, Easing easing = Easing.None) {
            if (this.Position == dest)
                return;

            lock (this.Tweens) {
                this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Movement);
            }

            this.Tweens.Add(
            new VectorTween(
                    TweenType.Movement,
                    this.Position,
                    dest,
                    this.TimeSource.GetCurrentTime(),
                    this.TimeSource.GetCurrentTime() + duration,
                    easing
                )
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
                new FloatTween(
                    TweenType.Rotation,
                    this.Rotation,
                    radians,
                    this.TimeSource.GetCurrentTime(),
                    this.TimeSource.GetCurrentTime() + duration,
                    easing
                )
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
            new VectorTween(
                    TweenType.Scale,
                    this.Scale,
                    newScale,
                    this.TimeSource.GetCurrentTime(),
                    this.TimeSource.GetCurrentTime() + duration,
                    easing
                )
            );
        }

        public void ScaleRelative(Vector2 increase, int duration, Easing easing = Easing.None) {
            lock (this.Tweens) {
                this.Tweens.RemoveAll(tween => tween.TweenType == TweenType.Scale);
            }

            this.Tweens.Add(
            new VectorTween(
                    TweenType.Scale,
                    this.Scale,
                    this.Scale + increase,
                    this.TimeSource.GetCurrentTime(),
                    this.TimeSource.GetCurrentTime() + duration,
                    easing
                )
            );
        }

        #endregion
    }
}
