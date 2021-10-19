using System;
using System.Linq;
using System.Collections.Generic;
using Furball.Engine.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics.Drawables.Managers {
    public class DrawableManager : UnmanagedDrawable {
        private List<BaseDrawable> _drawables = new();

        private List<ManagedDrawable>   _tempDrawManaged     = new();
        private List<ManagedDrawable>   _tempUpdateManaged   = new();
        private List<UnmanagedDrawable> _tempDrawUnmanaged   = new();
        private List<UnmanagedDrawable> _tempUpdateUnmanaged = new();

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
            
            FurballGame.InputManager.OnMouseDown += this.InputManagerOnMouseDown;
            FurballGame.InputManager.OnMouseUp   += this.InputManagerOnMouseUp;
            FurballGame.InputManager.OnMouseMove += this.InputManagerOnMouseMove; 
        }
        
        private List<ManagedDrawable> _tempClickUpManaged = new();
        private void InputManagerOnMouseUp(object sender, ((MouseButton mouseButton, Point position) args, string cursorName) e) {
            // should we lock these here?
            lock(this._tempClickUpManaged) {
                // Split _drawables into 2 lists containing the ManagedDrawables and the UnmanagedDrawables
                this._tempClickUpManaged.Clear();

                int tempCount = this._drawables.Count;
                for (int i = 0; i < tempCount; i++) {
                    BaseDrawable baseDrawable = this._drawables[i];

                    switch (baseDrawable) {
                        case ManagedDrawable managedDrawable:
                            this._tempClickUpManaged.Add(managedDrawable);
                            break;
                    }
                }
                
                this._tempClickUpManaged = this._tempClickUpManaged.OrderBy(o => o.Depth).ToList();

                for (int i = 0; i < this._tempClickUpManaged.Count; i++) {
                    ManagedDrawable drawable = this._tempClickUpManaged[i];

                    if(drawable.IsClicked)
                        drawable.InvokeOnClickUp(this, e.args.position);
                    if(drawable.IsDragging)
                        drawable.InvokeOnDragEnd(this, e.args.position);
                }
            }
        }

        private List<ManagedDrawable> _tempHoverManaged = new();
        private void InputManagerOnMouseMove(object sender, (Point mousePosition, string cursorName) e) {
            // should we lock these here?
            lock(this._tempClickManaged) {
                // Split _drawables into 2 lists containing the ManagedDrawables and the UnmanagedDrawables
                this._tempClickManaged.Clear();

                int tempCount = this._drawables.Count;
                for (int i = 0; i < tempCount; i++) {
                    BaseDrawable baseDrawable = this._drawables[i];

                    switch (baseDrawable) {
                        case ManagedDrawable managedDrawable:
                            this._tempClickManaged.Add(managedDrawable);
                            break;
                    }
                }
                
                this._tempHoverManaged = this._tempHoverManaged.OrderBy(o => o.Depth).ToList();

                for (var i = 0; i < this._tempClickManaged.Count; i++) {
                    ManagedDrawable drawable = this._tempClickManaged[i];

                    if (!drawable.Hoverable) continue;
                    
                    Rectangle rect = new(drawable.Position.ToPoint() - drawable.LastCalculatedOrigin.ToPoint(), drawable.Size.ToPoint());

                    //Whether the we are intersecting with the circle radius of the drawable
                    bool circleIntersect = false;
                    if (drawable.Circular) {
                        // Get the distance of the cursor to the drawable
                        float distanceToCentre = Vector2.Distance(e.mousePosition.ToVector2(), drawable.Position - drawable.LastCalculatedOrigin);
                    
                        // Checks if we are within the radius of the drawable
                        if (distanceToCentre < drawable.CircleRadius)
                            circleIntersect = true;
                    }

                    if (drawable.IsDragging) {
                        drawable.InvokeOnDrag(this, e.mousePosition);
                    }
                    
                    if (rect.Contains(e.mousePosition) && !drawable.Circular || circleIntersect) {
                        if(drawable.IsClicked && !drawable.IsHovered)
                            drawable.InvokeOnDragBegin(this, e.mousePosition);
                        
                        if (drawable.IsHovered) {
                            if (drawable.CoverHovers) break;
                            //If we dont cover hovers, just continue to the next one
                            continue;
                        }
                        
                        drawable.InvokeOnHover(this);
                        if (drawable.CoverHovers) break;
                    } else {
                        if (drawable.IsHovered) {
                            drawable.InvokeOnHoverLost(this);
                        }
                    }
                }
            }
        }
        
        private List<ManagedDrawable> _tempClickManaged = new();
        private void InputManagerOnMouseDown(object sender, ((MouseButton mouseButton, Point position) args, string cursorName) e) {
            // should we lock these here?
            lock(this._tempClickManaged) {
                // Split _drawables into 2 lists containing the ManagedDrawables and the UnmanagedDrawables
                this._tempClickManaged.Clear();

                int tempCount = this._drawables.Count;
                for (int i = 0; i < tempCount; i++) {
                    BaseDrawable baseDrawable = this._drawables[i];

                    switch (baseDrawable) {
                        case ManagedDrawable managedDrawable:
                            this._tempClickManaged.Add(managedDrawable);
                            break;
                    }
                }
                
                this._tempClickManaged = this._tempClickManaged.OrderBy(o => o.Depth).ToList();

                for (var i = 0; i < this._tempClickManaged.Count; i++) {
                    ManagedDrawable drawable = this._tempClickManaged[i];

                    if (!drawable.Clickable) continue;
                    
                    // Calculate the rectangle of the drawable
                    Rectangle rect = new(drawable.Position.ToPoint() - drawable.LastCalculatedOrigin.ToPoint(), drawable.Size.ToPoint());

                    //Whether the we are intersecting with the circle radius of the drawable
                    bool circleIntersect = false;
                    if (drawable.Circular) {
                        // Get the distance of the cursor to the drawable
                        float distanceToCentre = Vector2.Distance(e.args.position.ToVector2(), drawable.Position - drawable.LastCalculatedOrigin);
                    
                        // Checks if we are within the radius of the drawable
                        if (distanceToCentre < drawable.CircleRadius)
                            circleIntersect = true;
                    }
                    
                    if (rect.Contains(e.args.position) && !drawable.Circular || circleIntersect) {
                        drawable.InvokeOnClick(this, e.args.position);
                        if (drawable.CoverClicks) break;
                    }
                }
            }
        }

        public override void Draw(GameTime time, DrawableBatch drawableBatch, DrawableManagerArgs _ = null) {
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

            drawableBatch.Begin();

            this._tempDrawManaged.Sort((x, y) => (int)((y.Depth - x.Depth) * 100f));
            
            tempCount    = this._tempDrawManaged.Count;
            CountManaged = tempCount;

            for (int i = 0; i < tempCount; i++) {
                ManagedDrawable currentDrawable = this._tempDrawManaged[i];
                if (!currentDrawable.Visible) continue;
                
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
                
                Rectangle rect = new((args.Position).ToPoint(), new Point((int)Math.Ceiling(currentDrawable.Size.X * args.Scale.X), (int)Math.Ceiling(currentDrawable.Size.Y * args.Scale.Y)));

                if(rect.Intersects(FurballGame.DisplayRect))
                    currentDrawable.Draw(time, drawableBatch, args);
            }

            drawableBatch.End();

            this._tempDrawUnmanaged.Sort((x, y) => (int)((y.Depth - x.Depth) * 100f));

            tempCount      = this._tempDrawUnmanaged.Count;
            CountUnmanaged = tempCount;

            for (int i = 0; i < tempCount; i++) {
                UnmanagedDrawable currentDrawable = this._tempDrawUnmanaged[i];
                if (!currentDrawable.Visible) continue;
                
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

        private RenderTarget2D _target2D;
        public RenderTarget2D DrawRenderTarget2D(GameTime time, DrawableBatch batch, DrawableManagerArgs _ = null) {
            if(this._target2D?.Width != FurballGame.WindowWidth || this._target2D?.Height != FurballGame.WindowHeight)
                this._target2D = new RenderTarget2D(FurballGame.Instance.GraphicsDevice, FurballGame.WindowWidth, FurballGame.WindowHeight);
            
            FurballGame.Instance.GraphicsDevice.SetRenderTarget(this._target2D);

            FurballGame.Instance.GraphicsDevice.Clear(Color.Transparent);
            this.Draw(time, batch, _);

            FurballGame.Instance.GraphicsDevice.SetRenderTarget(null);

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

            // bool hoverHandled = false;
            // bool clickHandled = false;

            tempCount = this._tempUpdateManaged.Count;
            for (int i = 0; i < tempCount; i++) {
                ManagedDrawable currentDrawable = this._tempUpdateManaged[i];

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

        public void Add(BaseDrawable    drawable) => this._drawables.Add(drawable);
        public void Remove(BaseDrawable drawable) => this._drawables.Remove(drawable);

        public override void Dispose(bool disposing) {
            for (var i = 0; i < this._drawables.Count; i++) 
                this._drawables[i].Dispose(disposing);

            lock (StatLock) {
                Instances--;
                DrawableManagers.Remove(this);
            }
            
            FurballGame.InputManager.OnMouseDown -= this.InputManagerOnMouseDown;
            FurballGame.InputManager.OnMouseUp   -= this.InputManagerOnMouseUp;
            FurballGame.InputManager.OnMouseMove -= this.InputManagerOnMouseMove; 

            base.Dispose(disposing);
        }
    }
}
