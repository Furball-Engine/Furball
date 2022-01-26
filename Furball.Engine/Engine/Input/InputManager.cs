using System;
using System.Collections.Generic;
using System.Linq;
using Furball.Engine.Engine.DevConsole;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Furball.Engine.Engine.Input {
    public enum MouseButton {
        LeftButton,
        MiddleButton,
        RightButton
    }

    public class InputManager {
        /// <summary>
        /// The positions of all cursors and their states
        /// </summary>
        public List<MouseState> CursorStates {
            get {
                List<MouseState> temp = new();

                for (int i = 0; i < this.registeredInputMethods.Count; i++)
                    temp.AddRange(this.registeredInputMethods[i].CursorPositions);

                return temp;
            }
        }

        public InputManager() {
            this.OnMouseDown += DrawableOnMouseDown;
            this.OnMouseUp   += DrawableOnMouseUp;
            this.OnMouseDrag += DrawableOnMouseDrag;
            this.OnMouseMove += DrawableOnMouseMove;
        }

        /// <summary>
        ///     Recurses composite drawables and adds their drawables to the list
        /// </summary>
        /// <param name="drawablesToAddTo">A pre sorted list of drawables</param>
        public static int RecurseCompositeDrawables(ref List<ManagedDrawable> drawablesToAddTo, CompositeDrawable compositeDrawableToIterate, int indexToAddAt) {
            int added = 0;

            for (int i = 0; i < compositeDrawableToIterate.Drawables.Count; i++) {
                ManagedDrawable drawable = compositeDrawableToIterate.Drawables[i];

                if (drawable is CompositeDrawable compositeDrawable) {
                    drawablesToAddTo.Insert(indexToAddAt, compositeDrawable);
                    indexToAddAt++;
                    added++;
                    added += RecurseCompositeDrawables(ref drawablesToAddTo, compositeDrawable, indexToAddAt);
                } else {
                    drawablesToAddTo.Insert(indexToAddAt, drawable);
                    indexToAddAt++;
                    added++;
                }
            }

            return added;
        }

        private static void DrawableOnMouseMove(object sender, (Point mousePosition, string cursorName) e) {
            List<ManagedDrawable> drawables = new();
            DrawableManager.DrawableManagers.Where(x => x.Visible).ToList().ForEach(
            x => drawables.AddRange(x.Drawables.Where(y => y is ManagedDrawable && y.Hoverable).Cast<ManagedDrawable>())
            );

            drawables = drawables.OrderBy(o => o.Depth).ToList();

            List<ManagedDrawable> drawablesTempIterate = new(drawables);

            int fakei = 0;
            
            for (int i = 0; i < drawablesTempIterate.Count; i++) {
                ManagedDrawable drawable = drawablesTempIterate[i];
                if (drawable is CompositeDrawable compositeDrawable)
                    fakei += RecurseCompositeDrawables(ref drawables, compositeDrawable, fakei);

                fakei++;
            }

            bool tooltipSet = false;
            for (int i = 0; i < drawables.Count; i++) {
                ManagedDrawable drawable = drawables[i];

                if (drawable.RealContains(e.mousePosition)) {
                    if (!drawable.IsHovered && drawable.Hoverable) {
                        drawable.Hover(true);

                        if (drawable.CoverHovers)
                            break;
                    } else if (drawable.Hoverable && !tooltipSet) {
                        if (drawable.ToolTip != string.Empty && ConVars.ToolTips.Value == 1) {
                            FurballGame.TooltipDrawable.SetTooltip(drawable.ToolTip);
                            FurballGame.TooltipDrawable.Tweens.Clear();
                            FurballGame.TooltipDrawable.Tweens.Add(
                            new VectorTween(
                            TweenType.Movement,
                            FurballGame.TooltipDrawable.Position,
                            e.mousePosition.ToVector2() + new Vector2(10f),
                            FurballGame.Time,
                            FurballGame.Time + 1
                            )
                            );
                            FurballGame.TooltipDrawable.Visible = true;

                            tooltipSet = true;

                            if (FurballGame.TooltipDrawable.Position.Y + FurballGame.TooltipDrawable.Size.Y < FurballGame.DEFAULT_WINDOW_HEIGHT)
                                FurballGame.TooltipDrawable.OriginType =
                                    FurballGame.TooltipDrawable.Position.X + FurballGame.TooltipDrawable.Size.X > FurballGame.DEFAULT_WINDOW_WIDTH
                                        ? OriginType.TopRight : OriginType.TopLeft;
                            else
                                FurballGame.TooltipDrawable.OriginType =
                                    FurballGame.TooltipDrawable.Position.X + FurballGame.TooltipDrawable.Size.X > FurballGame.DEFAULT_WINDOW_WIDTH
                                        ? OriginType.BottomRight : OriginType.BottomLeft;

                        } else {
                            FurballGame.TooltipDrawable.Visible = false;
                        }
                    }
                } else {
                    drawable.Hover(false);
                }
            }
        }

        private static void DrawableOnMouseDrag(object sender, ((Point lastPosition, Point newPosition), string cursorName) e) {
            List<ManagedDrawable> drawables = new();
            DrawableManager.DrawableManagers.Where(x => x.Visible).ToList()
                           .ForEach(x => drawables.AddRange(x.Drawables.Where(y => y is ManagedDrawable).Cast<ManagedDrawable>()));

            List<ManagedDrawable> drawablesTempIterate = new(drawables);

            int fakei = 0;
            
            for (int i = 0; i < drawablesTempIterate.Count; i++) {
                ManagedDrawable drawable = drawablesTempIterate[i];
                if (drawable is CompositeDrawable compositeDrawable)
                    fakei += RecurseCompositeDrawables(ref drawables, compositeDrawable, fakei);

                fakei++;
            }

            for (int i = 0; i < drawables.Count; i++) {
                ManagedDrawable drawable = drawables[i];

                if (drawable.IsClicked && !drawable.IsDragging)
                    drawable.DragState(true, e.Item1.newPosition);

                if (drawable.IsDragging)
                    drawable.Drag(e.Item1.newPosition);
            }
        }

        private static void DrawableOnMouseUp(object _, ((MouseButton mouseButton, Point position) args, string cursorName) e) {
            List<ManagedDrawable> drawables = new();
            DrawableManager.DrawableManagers.Where(x => x.Visible).ToList()
                           .ForEach(x => drawables.AddRange(x.Drawables.Where(y => y is ManagedDrawable).Cast<ManagedDrawable>()));

            List<ManagedDrawable> drawablesTempIterate = new(drawables);

            int fakei = 0;
            
            for (int i = 0; i < drawablesTempIterate.Count; i++) {
                ManagedDrawable drawable = drawablesTempIterate[i];
                if (drawable is CompositeDrawable compositeDrawable)
                    fakei += RecurseCompositeDrawables(ref drawables, compositeDrawable, fakei);

                fakei++;
            }

            for (int i = 0; i < drawables.Count; i++) {
                ManagedDrawable drawable = drawables[i];

                if (drawable.IsClicked)
                    drawable.Click(false, e.args.position, e.args.mouseButton);
                if (drawable.IsDragging)
                    drawable.DragState(false, e.args.position);
            }
        }

        private static void DrawableOnMouseDown(object _, ((MouseButton mouseButton, Point position) args, string cursorName) e) {
            List<ManagedDrawable> drawables = new();
            DrawableManager.DrawableManagers.Where(x => x.Visible).ToList().ForEach(
            x => drawables.AddRange(x.Drawables.Where(y => y is ManagedDrawable && (y.Clickable || y is CompositeDrawable) && y.Visible).Cast<ManagedDrawable>())
            );

            drawables = drawables.OrderBy(o => o.Depth).ToList();

            List<ManagedDrawable> drawablesTempIterate = new(drawables);

            int fakei = 0;
            
            for (int i = 0; i < drawablesTempIterate.Count; i++) {
                ManagedDrawable drawable = drawablesTempIterate[i];
                if (drawable is CompositeDrawable compositeDrawable)
                    fakei += RecurseCompositeDrawables(ref drawables, compositeDrawable, fakei);

                fakei++;
            }

            for (int i = 0; i < drawables.Count; i++) {
                ManagedDrawable drawable = drawables[i];

                if (drawable.Clickable && drawable.RealContains(e.args.position)) {
                    drawable.Click(true, e.args.position, e.args.mouseButton);

                    if (drawable.CoverClicks) break;
                }
            }
        }

        /// <summary>
        /// The currently held Keyboard keys
        /// </summary>
        public List<Keys> HeldKeys {
            get {
                List<Keys> temp = new();

                for (int i = 0; i < this.registeredInputMethods.Count; i++)
                    temp.AddRange(this.registeredInputMethods[i].HeldKeys);

                return temp;
            }
        }

        /// <summary>
        /// The currently registered InputMethods
        /// </summary>
        private List<InputMethod> registeredInputMethods = new();

        public IReadOnlyList<InputMethod> RegisteredInputMethods => this.registeredInputMethods.AsReadOnly();

        /// <summary>
        /// Called when a key is pressed
        /// </summary>
        public event EventHandler<Keys> OnKeyDown;
        /// <summary>
        /// Called when a key is released
        /// </summary>
        public event EventHandler<Keys> OnKeyUp;
        /// <summary>
        /// Called when a mouse button is pressed
        /// </summary>
        public event EventHandler<((MouseButton mouseButton, Point position) args, string cursorName)> OnMouseDown;
        /// <summary>
        /// Called when a mouse button is released
        /// </summary>
        public event EventHandler<((MouseButton mouseButton, Point position) args, string cursorName)> OnMouseUp;
        /// <summary>
        /// Called when a cursor moves
        /// </summary>
        public event EventHandler<(Point mousePosition, string cursorName)> OnMouseMove;
        /// <summary>
        /// Called when a cursor moves
        /// </summary>
        public event EventHandler<((Point lastPosition, Point newPosition), string cursorName)> OnMouseDrag;
        /// <summary>
        /// Called when the cursor scrolls
        /// </summary>
        public event EventHandler<(int scrollAmount, string cursorName)> OnMouseScroll;

        private List<Keys> _diffKeysPressed  = new();
        private List<Keys> _diffKeysReleased = new();

        /// <summary>
        /// Updates all registered InputMethods and calls the necessary events
        /// </summary>
        public void Update() {
            List<MouseState> oldCursorStates = this.CursorStates.ToList();
            List<Keys>       oldKeys         = this.HeldKeys.ToList();

            for (int i = 0; i < this.registeredInputMethods.Count; i++) {
                InputMethod method = this.registeredInputMethods[i];

                method.Update();
            }

            #region OnKeyUp/Down

            this._diffKeysPressed  = this.HeldKeys.Except(oldKeys).ToList();
            this._diffKeysReleased = oldKeys.Except(this.HeldKeys).ToList();

            for (int i = 0; i < this._diffKeysPressed.Count; i++)
                this.OnKeyDown?.Invoke(this, this._diffKeysPressed[i]);

            for (int i = 0; i < this._diffKeysReleased.Count; i++)
                this.OnKeyUp?.Invoke(this, this._diffKeysReleased[i]);

            #endregion

            #region OnMouseUp/Down/Move/Scroll

            for (int i = 0; i < oldCursorStates.Count; i++) {
                MouseState oldState = oldCursorStates[i];

                List<MouseState> filteredStates = new();

                int cursorStateSize = this.CursorStates.Count;
                //Filtering States of the same name
                for (int k = 0; k < cursorStateSize; k++)
                    if (oldState.Name == this.CursorStates[k].Name)
                        filteredStates.Add(this.CursorStates[k]);

                for (int j = 0; j < filteredStates.Count; j++) {
                    MouseState newState = filteredStates[i];

                    //Handling Mouse Movement by comparing to the last Input Frame
                    if (oldState.Position != newState.Position) {
                        this.OnMouseMove?.Invoke(this, (newState.Position, newState.Name));

                        //We only are going to handle drags with M1
                        if (oldState.LeftButton == ButtonState.Pressed && newState.LeftButton == ButtonState.Pressed)
                            this.OnMouseDrag?.Invoke(this, ((oldState.Position, newState.Position), newState.Name));
                    }


                    //Handling The Left Mouse Button by comparing to the last Input Frame
                    if (oldState.LeftButton != newState.LeftButton)
                        if (oldState.LeftButton == ButtonState.Released)
                            this.OnMouseDown?.Invoke(this, ((MouseButton.LeftButton, newState.Position), newState.Name));
                        else
                            this.OnMouseUp?.Invoke(this, ((MouseButton.LeftButton, newState.Position), newState.Name));


                    //Handling The Right Mouse Button by comparing to the last Input Frame
                    if (oldState.RightButton != newState.RightButton)
                        if (oldState.RightButton == ButtonState.Released)
                            this.OnMouseDown?.Invoke(this, ((MouseButton.RightButton, newState.Position), newState.Name));
                        else
                            this.OnMouseUp?.Invoke(this, ((MouseButton.RightButton, newState.Position), newState.Name));


                    //Handling the Middle Mouse Button by comparing to the last Input Frame
                    if (oldState.MiddleButton != newState.MiddleButton)
                        if (oldState.MiddleButton == ButtonState.Released)
                            this.OnMouseDown?.Invoke(this, ((MouseButton.MiddleButton, newState.Position), newState.Name));
                        else
                            this.OnMouseUp?.Invoke(this, ((MouseButton.MiddleButton, newState.Position), newState.Name));


                    //Handling Scrolling by comparing to the last Input Frame
                    if (oldState.ScrollWheelValue != newState.ScrollWheelValue)
                        this.OnMouseScroll?.Invoke(this, (newState.ScrollWheelValue - oldState.ScrollWheelValue, newState.Name));
                }
            }

            #endregion
        }

        /// <summary>
        /// Registers a new input method and calls its Initialize method
        /// </summary>
        /// <param name="method">The InputMethod to add</param>
        public void RegisterInputMethod(InputMethod method) {
            this.registeredInputMethods.Add(method);
            method.Initialize();
        }
        /// <summary>
        /// Removes an input method and calls its Dispose method
        /// </summary>
        /// <param name="method">The InputMethod to remove</param>
        public void RemoveInputMethod(InputMethod method) {
            method.Dispose();
            this.registeredInputMethods.Remove(method);
        }
    }
}
