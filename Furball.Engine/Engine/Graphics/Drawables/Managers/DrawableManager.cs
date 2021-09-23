using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics.Drawables.Managers {
    public class DrawableManager : UnmanagedDrawable {
        private List<BaseDrawable> _drawables = new();

        private List<ManagedDrawable>   _tempDrawManaged     = new();
        private List<ManagedDrawable>   _tempUpdateManaged   = new();
        private List<UnmanagedDrawable> _tempDrawUnmanaged   = new();
        private List<UnmanagedDrawable> _tempUpdateUnmanaged = new();

        public override void Draw(GameTime time, SpriteBatch batch, DrawableManagerArgs _ = null) {
            // Split _drawables into 2 lists containing the ManagedDrawables and the UnmanagedDrawables
            this._tempDrawManaged.Clear();
            this._tempDrawUnmanaged.Clear();

            int tempCount = this._drawables.Count;
            for (int i = 0; i < tempCount; i++) {
                BaseDrawable baseDrawable = this._drawables[i];

                switch (baseDrawable) {
                    case ManagedDrawable managedDrawable:
                        this._tempDrawManaged.Add(managedDrawable);
                        break;
                    case UnmanagedDrawable unmanagedDrawable:
                        this._tempDrawUnmanaged.Add(unmanagedDrawable);
                        break;
                }
            }

            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            tempCount = this._tempDrawManaged.Count;
            for (int i = 0; i < tempCount; i++) {
                ManagedDrawable currentDrawable = this._tempDrawManaged[i];

                DrawableManagerArgs args = new() {
                    Color      = currentDrawable.ColorOverride,
                    Effects    = currentDrawable.SpriteEffect,
                    LayerDepth = currentDrawable.Depth,
                    Origin     = CalculateNewOriginPosition(currentDrawable),
                    Position   = currentDrawable.Position * (currentDrawable.ResolutionScale ? FurballGame.VerticalRatio : 1f),
                    Rotation   = currentDrawable.Rotation,
                    Scale      = currentDrawable.Scale * (currentDrawable.ResolutionScale ? FurballGame.VerticalRatio : 1f)
                };
                
                Rectangle rect = new(args.Position.ToPoint(), new Point((int)Math.Ceiling(currentDrawable.Size.X * args.Scale.X), (int)Math.Ceiling(currentDrawable.Size.Y * args.Scale.Y)));

                if(rect.Intersects(FurballGame.DisplayRect))
                    currentDrawable.Draw(time, batch, args);
            }

            batch.End();

            tempCount = this._tempDrawUnmanaged.Count;
            for (int i = 0; i < tempCount; i++) {
                UnmanagedDrawable currentDrawable = this._tempDrawUnmanaged[i];

                DrawableManagerArgs args = new() {
                    Color      = currentDrawable.ColorOverride,
                    Effects    = currentDrawable.SpriteEffect,
                    LayerDepth = currentDrawable.Depth,
                    Origin     = CalculateNewOriginPosition(currentDrawable),
                    Position   = currentDrawable.Position * (currentDrawable.ResolutionScale ? FurballGame.VerticalRatio : 1f),
                    Rotation   = currentDrawable.Rotation,
                    Scale      = currentDrawable.Scale * (currentDrawable.ResolutionScale ? FurballGame.VerticalRatio : 1f)
                };

                currentDrawable.Draw(time, batch, args);
            }
        }

        public RenderTarget2D DrawRenderTarget2D(GameTime time, SpriteBatch batch, DrawableManagerArgs _ = null) {
            //TODO: reuse rendertargets instead of creating new ones every time
            RenderTarget2D target = new RenderTarget2D(FurballGame.Instance.GraphicsDevice, FurballGame.WindowWidth, FurballGame.WindowHeight);
            FurballGame.Instance.GraphicsDevice.SetRenderTarget(target);

            this.Draw(time, batch, _);

            FurballGame.Instance.GraphicsDevice.SetRenderTarget(null);

            return target;
        }

        private static Vector2 CalculateNewOriginPosition(BaseDrawable drawable) {
            return drawable.OriginType switch {
                OriginType.TopLeft     => Vector2.Zero,
                OriginType.TopRight    => new Vector2(drawable.Size.X, 0),
                OriginType.BottomLeft  => new Vector2(0,               drawable.Size.Y),
                OriginType.BottomRight => new Vector2(drawable.Size.X, drawable.Size.Y),
                _                      => throw new ArgumentOutOfRangeException(nameof (drawable.OriginType), "That OriginType is unsupported.")
            };
        }

        public override void Update(GameTime time) {
            // Split _drawables into 2 lists containing the ManagedDrawables and the UnmanagedDrawables
            this._tempUpdateManaged.Clear();
            this._tempUpdateUnmanaged.Clear();

            int tempCount = this._drawables.Count;
            for (int i = 0; i < tempCount; i++) {
                BaseDrawable baseDrawable = this._drawables[i];

                switch (baseDrawable) {
                    case ManagedDrawable managedDrawable:
                        this._tempUpdateManaged.Add(managedDrawable);
                        break;
                    case UnmanagedDrawable unmanagedDrawable:
                        this._tempUpdateUnmanaged.Add(unmanagedDrawable);
                        break;
                }
            }

            this._tempUpdateManaged   = this._tempUpdateManaged.OrderBy(o => o.Depth).ToList();
            this._tempUpdateUnmanaged = this._tempUpdateUnmanaged.OrderBy(o => o.Depth).ToList();

            bool hoverHandled = false;
            bool clickHandled = false;

            tempCount = this._tempUpdateManaged.Count;
            for (int i = 0; i < tempCount; i++) {
                ManagedDrawable currentDrawable = this._tempUpdateManaged[i];

                #region Input

                Point cursor = FurballGame.InputManager.CursorStates[0].State.Position;
                Rectangle rect = new(currentDrawable.Position.ToPoint(), currentDrawable.Size.ToPoint());

                if (rect.Contains(cursor)) {
                    if (FurballGame.InputManager.CursorStates[0].State.LeftButton == ButtonState.Pressed) {
                        if (!clickHandled) {
                            if (currentDrawable.Clickable) {
                                if (!currentDrawable.IsClicked) {
                                    currentDrawable.InvokeOnClick(this, cursor);
                                    currentDrawable.IsClicked = true;
                                } else {
                                    if(!currentDrawable.IsDragging)
                                        currentDrawable.InvokeOnDragBegin(this, cursor);

                                    currentDrawable.InvokeOnDrag(this, cursor);
                                    currentDrawable.IsDragging = true;
                                }
                            }

                            clickHandled = true;
                        }
                    } else {
                        if (currentDrawable.IsClicked) {
                            currentDrawable.InvokeOnClickUp(this, cursor);
                            currentDrawable.IsClicked = false;

                            if (currentDrawable.IsDragging) {
                                currentDrawable.IsDragging = false;
                                currentDrawable.InvokeOnDragEnd(this, cursor);
                            }
                        }
                    }
                    if (!hoverHandled) {
                        if (!currentDrawable.IsHovered && currentDrawable.Hoverable) {
                            currentDrawable.InvokeOnHover(this);
                            currentDrawable.IsHovered = true;
                        }

                        hoverHandled = true;
                    }
                } else {
                    if (FurballGame.InputManager.CursorStates[0].State.LeftButton == ButtonState.Released) {
                        if (currentDrawable.IsClicked) {
                            currentDrawable.InvokeOnClickUp(this, cursor);
                            currentDrawable.IsClicked = false;
                        }
                    }
                    if (currentDrawable.IsHovered) {
                        currentDrawable.InvokeOnHoverLost(this);
                        currentDrawable.IsHovered = false;
                    }
                }

                #endregion


                currentDrawable.UpdateTweens();
                currentDrawable.Update(time);
            }

            tempCount = this._tempUpdateUnmanaged.Count;
            for (int i = 0; i < tempCount; i++) {
                UnmanagedDrawable currentDrawable = this._tempUpdateUnmanaged[i];

                currentDrawable.UpdateTweens();
                currentDrawable.Update(time);
            }
        }

        public void Add(BaseDrawable drawable) => this._drawables.Add(drawable);
    }
}
