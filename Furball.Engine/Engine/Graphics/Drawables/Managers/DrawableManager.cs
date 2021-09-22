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

                Rectangle rect = new(currentDrawable.Position.ToPoint(), currentDrawable.Size.ToPoint());

                if (rect.Contains(FurballGame.InputManager.CursorStates[0].State.Position)) {
                    if (FurballGame.InputManager.CursorStates[0].State.LeftButton == ButtonState.Pressed) {
                        if (!clickHandled) {
                            if (!currentDrawable.IsClicked) {
                                currentDrawable.InvokeOnClick(this);
                                currentDrawable.IsClicked = true;
                            }

                            clickHandled = true;
                        }
                    } else {
                        if (currentDrawable.IsClicked) {
                            currentDrawable.InvokeOnUnClick(this);
                            currentDrawable.IsClicked = false;
                        }
                    }
                    if (!hoverHandled) {
                        if (!currentDrawable.IsHovered) {
                            currentDrawable.InvokeOnHover(this);
                            currentDrawable.IsHovered = true;
                        }

                        hoverHandled = true;
                    }
                } else {
                    if (FurballGame.InputManager.CursorStates[0].State.LeftButton == ButtonState.Released) {
                        if (currentDrawable.IsClicked) {
                            currentDrawable.InvokeOnUnClick(this);
                            currentDrawable.IsClicked = false;
                        }
                    }
                    if (currentDrawable.IsHovered) {
                        currentDrawable.InvokeOnHoverLost(this);
                        currentDrawable.IsHovered = false;
                    }
                }


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
