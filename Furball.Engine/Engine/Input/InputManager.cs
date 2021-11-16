using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Furball.Engine.Engine.DevConsole;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Engine.Engine.Helpers;
using Silk.NET.Input;
using Silk.NET.Input.Extensions;

namespace Furball.Engine.Engine.Input {
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

        private static void DrawableOnMouseMove(object sender, (Vector2 position, string cursorName) e) {
            List<ManagedDrawable> drawables = new();
            DrawableManager.DrawableManagers.Where(x => x.Visible).ToList().ForEach(
            x => drawables.AddRange(x.Drawables.Where(y => y is ManagedDrawable && y.Hoverable).Cast<ManagedDrawable>())
            );

            drawables = drawables.OrderBy(o => o.Depth).ToList();

            bool tooltipSet = false;
            for (int i = 0; i < drawables.Count; i++) {
                ManagedDrawable drawable = drawables[i];

                if (drawable.Contains(e.position.ToPoint())) {
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
                            e.position + new Vector2(10f),
                            FurballGame.Time,
                            FurballGame.Time + 1
                            )
                            );
                            FurballGame.TooltipDrawable.Visible = true;

                            tooltipSet = true;

                            FurballGame.TooltipDrawable.OriginType =
                                FurballGame.TooltipDrawable.Position.X + FurballGame.TooltipDrawable.Size.X > FurballGame.DEFAULT_WINDOW_WIDTH ? OriginType.TopRight
                                    : OriginType.TopLeft;

                        } else {
                            FurballGame.TooltipDrawable.Visible = false;
                        }
                    }
                } else {
                    drawable.Hover(false);
                }
            }
        }

        private static void DrawableOnMouseDrag(object sender, ((Vector2 lastPosition, Vector2 newPosition), string cursorName) e) {
            List<ManagedDrawable> drawables = new();
            DrawableManager.DrawableManagers.Where(x => x.Visible).ToList()
                           .ForEach(x => drawables.AddRange(x.Drawables.Where(y => y is ManagedDrawable).Cast<ManagedDrawable>()));

            for (int i = 0; i < drawables.Count; i++) {
                ManagedDrawable drawable = drawables[i];

                if (drawable.IsClicked && !drawable.IsDragging)
                    drawable.DragState(true, e.Item1.newPosition.ToPoint());

                if (drawable.IsDragging)
                    drawable.Drag(e.Item1.newPosition.ToPoint());
            }
        }

        private static void DrawableOnMouseUp(object _, ((MouseButton mouseButton, Vector2 position) args, string cursorName) e) {
            List<ManagedDrawable> drawables = new();
            DrawableManager.DrawableManagers.Where(x => x.Visible).ToList()
                           .ForEach(x => drawables.AddRange(x.Drawables.Where(y => y is ManagedDrawable).Cast<ManagedDrawable>()));

            for (int i = 0; i < drawables.Count; i++) {
                ManagedDrawable drawable = drawables[i];

                if (drawable.IsClicked)
                    drawable.Click(false, e.args.position.ToPoint());
                if (drawable.IsDragging)
                    drawable.DragState(false, e.args.position.ToPoint());
            }
        }

        private static void DrawableOnMouseDown(object _, ((MouseButton mouseButton, Vector2 position) args, string cursorName) e) {
            List<ManagedDrawable> drawables = new();
            DrawableManager.DrawableManagers.Where(x => x.Visible).ToList().ForEach(
            x => drawables.AddRange(x.Drawables.Where(y => y is ManagedDrawable && y.Clickable && y.Visible).Cast<ManagedDrawable>())
            );

            drawables = drawables.OrderBy(o => o.Depth).ToList();

            for (int i = 0; i < drawables.Count; i++) {
                ManagedDrawable drawable = drawables[i];

                if (drawable.Contains(e.args.position.ToPoint())) {
                    drawable.Click(true, e.args.position.ToPoint());

                    if (drawable.CoverClicks) break;
                }
            }
        }

        /// <summary>
        /// The currently held Keyboard keys
        /// </summary>
        public List<Key> HeldKeys {
            get {
                List<Key> temp = new();

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
        public event EventHandler<Key> OnKeyDown;
        /// <summary>
        /// Called when a key is released
        /// </summary>
        public event EventHandler<Key> OnKeyUp;
        /// <summary>
        /// Called when a mouse button is pressed
        /// </summary>
        public event EventHandler<((MouseButton mouseButton, Vector2 position) args, string cursorName)> OnMouseDown;
        /// <summary>
        /// Called when a mouse button is released
        /// </summary>
        public event EventHandler<((MouseButton mouseButton, Vector2 position) args, string cursorName)> OnMouseUp;
        /// <summary>
        /// Called when a cursor moves
        /// </summary>
        public event EventHandler<(Vector2 position, string cursorName)> OnMouseMove;
        /// <summary>
        /// Called when a cursor moves
        /// </summary>
        public event EventHandler<((Vector2 lastPosition, Vector2 newPosition), string cursorName)> OnMouseDrag;
        /// <summary>
        /// Called when the cursor scrolls
        /// </summary>
        public event EventHandler<((int scrollWheelId, float scrollAmount) scroll, string cursorName)> OnMouseScroll;

        private List<Key> _diffKeysPressed  = new();
        private List<Key> _diffKeysReleased = new();

        /// <summary>
        /// Updates all registered InputMethods and calls the necessary events
        /// </summary>
        public void Update() {
            List<MouseState> oldCursorStates = this.CursorStates.ToList();
            List<Key>        oldKeys         = this.HeldKeys.ToList();

            for (int i = 0; i < this.registeredInputMethods.Count; i++) {
                InputMethod method = this.registeredInputMethods[i];

                method.Update();
            }

            #region OnKeyUp/Down

            //TODO: fix keyboard input not working for whatever reason

            this._diffKeysPressed  = this.HeldKeys.Except(oldKeys).ToList();
            this._diffKeysReleased = oldKeys.Except(this.HeldKeys).ToList();

            for (int i = 0; i < this._diffKeysPressed.Count; i++)
                this.OnKeyDown?.Invoke(this, this._diffKeysPressed[i]);

            for (int i = 0; i < this._diffKeysReleased.Count; i++)
                this.OnKeyUp?.Invoke(this, this._diffKeysReleased[i]);

            #endregion

            #region OnMouseUp/Down/Move/Scroll

            for (int i = 0; i < oldCursorStates.Count; i++) {
                MouseState        oldState             = oldCursorStates[i];
                Span<ScrollWheel> oldStateScrollWheels = oldState.GetScrollWheels();

                List<MouseState> filteredStates = new();

                int cursorStateSize = this.CursorStates.Count;
                //Filtering States of the same name
                for (int k = 0; k < cursorStateSize; k++)
                    if (oldState.Name == this.CursorStates[k].Name)
                        filteredStates.Add(this.CursorStates[k]);

                for (int j = 0; j < filteredStates.Count; j++) {
                    MouseState newState = filteredStates[i];

                    // Handling Mouse Movement by comparing to the last Input Frame
                     if (oldState.Position != newState.Position) {
                         this.OnMouseMove?.Invoke(this, (newState.Position, newState.Name));
                    
                         //We only are going to handle drags with M1
                         if (oldState.IsButtonPressed(MouseButton.Left) && newState.IsButtonPressed(MouseButton.Left))
                             this.OnMouseDrag?.Invoke(this, ((oldState.Position, newState.Position), newState.Name));
                     }
                    
                     //Handling The Left Mouse Button by comparing to the last Input Frame
                     if (oldState.IsButtonPressed(MouseButton.Left) != newState.IsButtonPressed(MouseButton.Left))
                         if (!oldState.IsButtonPressed(MouseButton.Left))
                             this.OnMouseDown?.Invoke(this, ((MouseButton.Left, newState.Position), newState.Name));
                         else
                             this.OnMouseUp?.Invoke(this, ((MouseButton.Left, newState.Position), newState.Name));
                    
                    
                     //Handling The Right Mouse Button by comparing to the last Input Frame
                     if (oldState.IsButtonPressed(MouseButton.Right) != newState.IsButtonPressed(MouseButton.Right))
                         if (!oldState.IsButtonPressed(MouseButton.Right))
                             this.OnMouseDown?.Invoke(this, ((MouseButton.Right, newState.Position), newState.Name));
                         else
                             this.OnMouseUp?.Invoke(this, ((MouseButton.Right, newState.Position), newState.Name));
                    
                     //Handling the Middle Mouse Button by comparing to the last Input Frame
                     if (oldState.IsButtonPressed(MouseButton.Middle) != newState.IsButtonPressed(MouseButton.Middle))
                         if (!oldState.IsButtonPressed(MouseButton.Middle))
                             this.OnMouseDown?.Invoke(this, ((MouseButton.Middle, newState.Position), newState.Name));
                         else
                             this.OnMouseUp?.Invoke(this, ((MouseButton.Middle, newState.Position), newState.Name));

                     Span<ScrollWheel> newStateScrollWheels = newState.GetScrollWheels();

                     for (int i2 = 0; i2 < newStateScrollWheels.Length; i2++) {
                         ScrollWheel newWheel = newStateScrollWheels[i2];
                         ScrollWheel oldWheel = oldStateScrollWheels[i2];
                         //Handling Scrolling by comparing to the last Input Frame
                         if (Math.Abs(oldWheel.Y - newWheel.Y) > 0.01f)
                             this.OnMouseScroll?.Invoke(this, ((i2, newWheel.Y - oldWheel.Y), newState.Name));
                     }
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
