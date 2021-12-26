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

namespace Furball.Engine.Engine.Input {
    public class InputManager {
        /// <summary>
        /// The positions of all cursors and their states
        /// </summary>
        public List<FurballMouseState> CursorStates {
            get {
                List<FurballMouseState> temp = new();

                foreach (InputMethod method in this.registeredInputMethods) {
                    temp.AddRange(method.MouseStates);
                }
                
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

        private static readonly List<ManagedDrawable> _knownHovers = new();
        private static void DrawableOnMouseMove(object sender, (Vector2 position, string cursorName) e) {

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

            bool doHover    = true;
            bool tooltipSet = false;
            for (int i = 0; i < drawables.Count; i++) {
                ManagedDrawable drawable = drawables[i];

                if (drawable.RealContains(e.position.ToPoint())) {
                    if (drawable.IsHovered && drawable.CoverHovers) {
                        for (int i2 = 0; i2 < _knownHovers.Count; i2++) {
                            ManagedDrawable managedDrawable = _knownHovers[i2];
                            if (managedDrawable != drawable)
                                managedDrawable.Hover(false);
                        }
                        _knownHovers.RemoveAll(x => x != drawable);
                        doHover = false;
                    }
                    
                    if (!drawable.IsHovered && drawable.Hoverable) {
                        if (doHover) {
                            _knownHovers.Remove(drawable);
                            _knownHovers.Add(drawable);
                            drawable.Hover(true);
                        }

                        if (drawable.CoverHovers) {
                            for (int i2 = 0; i2 < _knownHovers.Count; i2++) {
                                ManagedDrawable managedDrawable = _knownHovers[i2];
                                if (managedDrawable != drawable)
                                    managedDrawable.Hover(false);
                            }
                            _knownHovers.RemoveAll(x => x != drawable);
                            doHover = false;
                        }
                    } else if (drawable.Hoverable && !tooltipSet && doHover) {
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
                    _knownHovers.Remove(drawable);
                    drawable.Hover(false);
                }
            }
        }

        private static void DrawableOnMouseDrag(object sender, ((Vector2 lastPosition, Vector2 newPosition), string cursorName) e) {
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
                    drawable.DragState(true, e.Item1.newPosition.ToPoint());

                if (drawable.IsDragging)
                    drawable.Drag(e.Item1.newPosition.ToPoint());
            }
        }

        private static void DrawableOnMouseUp(object _, ((MouseButton mouseButton, Vector2 position) args, string cursorName) e) {
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
                    drawable.Click(false, e.args.position.ToPoint());
                if (drawable.IsDragging)
                    drawable.DragState(false, e.args.position.ToPoint());
            }
        }

        private static void DrawableOnMouseDown(object _, ((MouseButton mouseButton, Vector2 position) args, string cursorName) e) {
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

                if (drawable.RealContains(e.args.position.ToPoint())) {
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

                foreach (InputMethod method in this.registeredInputMethods) {
                    temp.AddRange(method.HeldKeys);
                }

                return temp;
            }
        }

        public List<IKeyboard> Keyboards {
            get {
                List<IKeyboard> temp = new();

                foreach (InputMethod method in this.registeredInputMethods) {
                    temp.AddRange(method.Keyboards);
                }
                
                return temp;
            }
        }

        public List<IMouse> Mice {
            get {
                List<IMouse> temp = new();

                foreach (InputMethod method in this.registeredInputMethods) {
                    temp.AddRange(method.Mice);
                }
                
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
        public event EventHandler<(IKeyboard keyboard, char character)> OnCharInput;

        private List<Key> _diffKeysPressed  = new();
        private List<Key> _diffKeysReleased = new();

        /// <summary>
        /// Updates all registered InputMethods and calls the necessary events
        /// </summary>
        public void Update() {
            List<FurballMouseState> oldCursorStates = this.CursorStates;
            List<Key>               oldKeys         = this.HeldKeys;

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
                FurballMouseState        oldState             = oldCursorStates[i];
                Span<ScrollWheel> oldStateScrollWheels = oldState.ScrollWheels;

                List<FurballMouseState> filteredStates = new();

                int cursorStateSize = this.CursorStates.Count;
                //Filtering States of the same name
                for (int k = 0; k < cursorStateSize; k++)
                    if (oldState.Name == this.CursorStates[k].Name)
                        filteredStates.Add(this.CursorStates[k]);

                for (int j = 0; j < filteredStates.Count; j++) {
                    FurballMouseState newState = filteredStates[i];

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

                     for (int i2 = 0; i2 < newState.ScrollWheels.Length; i2++) {
                         ScrollWheel newWheel = newState.ScrollWheels[i2];
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
            
            //Register to the keyboards OnCharInput
            foreach (IKeyboard keyboard in method.Keyboards) {
                keyboard.KeyChar += delegate(IKeyboard keyboard, char c) {
                    this.OnCharInput?.Invoke(this, (keyboard, c));
                };
            }
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
