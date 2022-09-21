using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Furball.Engine.Engine.DevConsole;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Input.Events;
using Furball.Engine.Engine.Input.InputMethods;
using JetBrains.Annotations;
using Kettu;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input;

public class InputManager {
    public SilkWindowingMouseInputMethod    SilkWindowingMouseInputMethod;
    public SilkWindowingKeyboardInputMethod SilkWindowingKeyboardInputMethod;

    /// <summary>
    /// Called when a key is pressed
    /// </summary>
    public event EventHandler<KeyEventArgs> OnKeyDown;
    /// <summary>
    /// Called when a key is released
    /// </summary>
    public event EventHandler<KeyEventArgs> OnKeyUp;
    /// <summary>
    /// Called when a mouse button is pressed
    /// </summary>
    public event EventHandler<MouseButtonEventArgs> OnMouseDown;
    /// <summary>
    /// Called when a mouse button is released
    /// </summary>
    public event EventHandler<MouseButtonEventArgs> OnMouseUp;
    /// <summary>
    /// Called when a cursor moves
    /// </summary>
    public event EventHandler<MouseMoveEventArgs> OnMouseMove;
    /// <summary>
    /// Called when a cursor moves
    /// </summary>
    public event EventHandler<MouseDragEventArgs> OnMouseDrag;
    /// <summary>
    /// Called when the cursor scrolls
    /// </summary>
    public event EventHandler<MouseScrollEventArgs> OnMouseScroll;
    public event EventHandler<CharInputEvent> OnCharInput;

    [CanBeNull]
    public ICharInputHandler CharInputHandler => this._charInputHandlers.Count == 0 ? null : this._charInputHandlers.Last.Value;

    //We would use a `Stack<T>` here but that doesnt have arbitrary removal of items
    private readonly LinkedList<ICharInputHandler> _charInputHandlers = new();

    public void TakeTextFocus([NotNull] ICharInputHandler handler) {
        //If someone tries to take focus twice, ignore this call
        if (this.CharInputHandler == handler)
            return;

        bool wasEmpty = this._charInputHandlers.Count == 0;

        //If the current handler should not be saved in the stack, defocus it and drop it from the stack
        if (!CharInputHandler?.SaveInStack ?? false) {
            Logger.Log($"{CharInputHandler.GetType().Name} will not be saved on the stack!", LoggerLevelInput.InstanceInfo);
            this.CharInputHandler.HandleDefocus();
            this._charInputHandlers.RemoveLast();
        } else if (this.CharInputHandler is {
                       SaveInStack: true
                   }) {
            //Tell the current top of the stack to defocus
            this.CharInputHandler.HandleDefocus();
        }

        //Push the new handler to the stack
        this._charInputHandlers.AddLast(handler);
        //Focus the new handler
        handler.HandleFocus();

        // Tell all keyboard to begin input if we started with nothing in the stack
        // this is to prevent us beginning the input multiple times, which might not behave well on all platform
        if (wasEmpty)
            this.Keyboards.ForEach(x => x.BeginInput());

        Logger.Log($"{handler.GetType().Name} has taken text focus!", LoggerLevelInput.InstanceInfo);
    }

    public void ReleaseTextFocus([NotNull] ICharInputHandler handler) {
        LinkedListNode<ICharInputHandler> foundNode = this._charInputHandlers.FindLast(handler);

        //If we currently arent in the stack, then ignore the call
        if (foundNode == null)
            return;

        this._charInputHandlers.Remove(foundNode);

        //Tell the handler to do their logic on defocus
        handler.HandleDefocus();

        Logger.Log($"{handler.GetType().Name} has lost text focus!", LoggerLevelInput.InstanceInfo);

        //If the stack is now empty, end text input
        if (this._charInputHandlers.Count == 0) {
            this.Keyboards.ForEach(x => x.EndInput());
            Logger.Log("The text stack is now empty!", LoggerLevelInput.InstanceInfo);
        } else {
            //Tell the new top of the stack to focus again
            this.CharInputHandler!.HandleFocus();
        }
    }

    public void ReleaseTextFocus() {
        if (this.CharInputHandler == null)
            return;

        this.ReleaseTextFocus(this.CharInputHandler);
    }

    /// <summary>
    /// The positions of all cursors and their states
    /// </summary>
    public List<FurballMouse> CursorStates {
        get {
            List<FurballMouse> temp = new();

            foreach (InputMethod method in this._registeredInputMethods) {
                temp.AddRange(method.Mice);
            }

            return temp;
        }
    }

    public InputManager() {
        this.OnMouseDown += DrawableOnMouseDown;
        this.OnMouseUp   += DrawableOnMouseUp;
        this.OnMouseDrag += DrawableOnMouseDrag;
        this.OnMouseMove += DrawableOnMouseMove;

        this.OnKeyDown += KeybindOnKeyDown;
    }

    private void KeybindOnKeyDown(object sender, KeyEventArgs keyEventArgs) {
        foreach (Keybind registeredKeybind in this.RegisteredKeybinds) {
            if (registeredKeybind.Enabled && registeredKeybind.Key == keyEventArgs.Key)
                registeredKeybind.OnPressed.Invoke(keyEventArgs.Keyboard);
        }
    }

    /// <summary>
    ///     Recurses composite drawables and adds their drawables to the list
    /// </summary>
    /// <param name="drawablesToAddTo">A pre sorted list of drawables</param>
    /// <param name="compositeDrawableToIterate">The composite drawable to iterate and add</param>
    /// <param name="indexToAddAt">The index to add them to</param>
    public static int RecurseCompositeDrawables(ref List<Drawable> drawablesToAddTo, CompositeDrawable compositeDrawableToIterate, int indexToAddAt) {
        int added = 0;

        for (int i = compositeDrawableToIterate.Drawables.Count - 1; i >= 0; i--) {
            Drawable drawable = compositeDrawableToIterate.Drawables[i];

            if (drawable is CompositeDrawable compositeDrawable) {
                if (!compositeDrawable.InvisibleToInput) {
                    drawablesToAddTo.Insert(indexToAddAt, compositeDrawable);
                    indexToAddAt++;
                    added++;
                }
                added += RecurseCompositeDrawables(ref drawablesToAddTo, compositeDrawable, indexToAddAt);
            } else {
                if (compositeDrawableToIterate.ChildrenInvisibleToInput)
                    continue;

                drawablesToAddTo.Insert(indexToAddAt, drawable);
                indexToAddAt++;
                added++;
            }
        }

        return added;
    }

    private static readonly List<Drawable> KnownHovers = new();
    private static void DrawableOnMouseMove(object sender, MouseMoveEventArgs e) {
        List<Drawable> drawables = new();
        DrawableManager.DrawableManagers.Where(x => x.Visible).ToList().ForEach(x => drawables.AddRange(x.Drawables.Where(y => y.Hoverable)));

        drawables.Sort(DrawableInputComparer.Instance);

        List<Drawable> drawablesTempIterate = new(drawables);

        int fakei = 0;

        for (int i = 0; i < drawablesTempIterate.Count; i++) {
            Drawable drawable = drawablesTempIterate[i];
            if (drawable is CompositeDrawable compositeDrawable)
                fakei += RecurseCompositeDrawables(ref drawables, compositeDrawable, fakei);

            fakei++;
        }

        drawables.RemoveAll(
        x => x is CompositeDrawable {
            InvisibleToInput: true
        }
        );

        bool doHover    = true;
        bool tooltipSet = false;
        for (int i = 0; i < drawables.Count; i++) {
            Drawable drawable = drawables[i];

            if (drawable.RealContains(e.Position)) {
                if (drawable.IsHovered && drawable.CoverHovers) {
                    // for (int i2 = 0; i2 < _knownHovers.Count; i2++) {
                    //     ManagedDrawable managedDrawable = _knownHovers[i2];
                    //     // if (managedDrawable != drawable)
                    //         // managedDrawable.Hover(false);
                    // }
                    // _knownHovers.RemoveAll(x => x != drawable);
                    doHover = false;
                }

                if (!drawable.IsHovered && drawable.Hoverable) {
                    if (doHover) {
                        KnownHovers.Remove(drawable);

                        KnownHovers.ForEach(x => x.Hover(false));
                        KnownHovers.Clear();

                        KnownHovers.Add(drawable);
                        drawable.Hover(true);
                    }

                    if (drawable.CoverHovers) {
                        // for (int i2 = 0; i2 < _knownHovers.Count; i2++) {
                        //     ManagedDrawable managedDrawable = _knownHovers[i2];
                        //     // if (managedDrawable != drawable)
                        //         // managedDrawable.Hover(false);
                        // }
                        // _knownHovers.RemoveAll(x => x != drawable);
                        doHover = false;
                    }
                }
                if (drawable.Hoverable && !tooltipSet/* && doHover */) {
                    if (drawable.ToolTip != string.Empty && ConVars.ToolTips) {
                        FurballGame.TooltipDrawable.SetTooltip(drawable.ToolTip);
                        FurballGame.TooltipDrawable.MoveTo(e.Position + new Vector2(10f));
                        FurballGame.TooltipDrawable.Visible = true;

                        tooltipSet = true;

                        if (FurballGame.TooltipDrawable.Position.Y + FurballGame.TooltipDrawable.Size.Y < FurballGame.DEFAULT_WINDOW_HEIGHT)
                            FurballGame.TooltipDrawable.OriginType =
                                FurballGame.TooltipDrawable.Position.X + FurballGame.TooltipDrawable.Size.X > FurballGame.WindowWidth
                                    ? OriginType.TopRight
                                    : OriginType.TopLeft;
                        else
                            FurballGame.TooltipDrawable.OriginType =
                                FurballGame.TooltipDrawable.Position.X + FurballGame.TooltipDrawable.Size.X > FurballGame.WindowWidth
                                    ? OriginType.BottomRight
                                    : OriginType.BottomLeft;

                    } else {
                        FurballGame.TooltipDrawable.Visible = false;
                    }
                }
            } else {
                KnownHovers.Remove(drawable);
                drawable.Hover(false);
            }
        }
        if (!tooltipSet)
            FurballGame.TooltipDrawable.Visible = false;
    }

    private static void DrawableOnMouseDrag(object sender, MouseDragEventArgs e) {
        List<Drawable> drawables = new();
        DrawableManager.DrawableManagers.Where(x => x.Visible).ToList().ForEach(x => drawables.AddRange(x.Drawables));

        List<Drawable> drawablesTempIterate = new(drawables);

        int fakei = 0;

        for (int i = 0; i < drawablesTempIterate.Count; i++) {
            Drawable drawable = drawablesTempIterate[i];
            if (drawable is CompositeDrawable compositeDrawable)
                fakei += RecurseCompositeDrawables(ref drawables, compositeDrawable, fakei);

            fakei++;
        }

        drawables.RemoveAll(
        x => x is CompositeDrawable {
            InvisibleToInput: true
        }
        );

        for (int i = 0; i < drawables.Count; i++) {
            Drawable drawable = drawables[i];

            if (drawable.IsClicked && !drawable.IsDragging)
                drawable.DragState(true, e);

            if (drawable.IsDragging)
                drawable.Drag(e);
        }
    }

    private static void DrawableOnMouseUp(object _, MouseButtonEventArgs e) {
        List<Drawable> drawables = new();
        DrawableManager.DrawableManagers.Where(x => x.Visible).ToList().ForEach(x => drawables.AddRange(x.Drawables));

        List<Drawable> drawablesTempIterate = new(drawables);

        int fakei = 0;

        for (int i = 0; i < drawablesTempIterate.Count; i++) {
            Drawable drawable = drawablesTempIterate[i];
            if (drawable is CompositeDrawable compositeDrawable)
                fakei += RecurseCompositeDrawables(ref drawables, compositeDrawable, fakei);

            fakei++;
        }

        drawables.RemoveAll(
        x => x is CompositeDrawable {
            InvisibleToInput: true
        }
        );

        for (int i = 0; i < drawables.Count; i++) {
            Drawable drawable = drawables[i];

            if (drawable.IsClicked)
                drawable.Click(false, e);
            if (drawable.IsDragging)
                drawable.DragState(false, new MouseDragEventArgs(e.Mouse.DragStates[(int)e.Button].StartPosition, e.Mouse.Position, e.Mouse.Position, e.Button, e.Mouse));
        }
    }

    private static void DrawableOnMouseDown(object _, MouseButtonEventArgs e) {
        List<Drawable> drawables = new();
        DrawableManager.DrawableManagers.Where(x => x.Visible).ToList().ForEach(
        x => drawables.AddRange(x.Drawables.Where(y => (y.Clickable || y is CompositeDrawable) && y.Visible))
        );

        drawables.Sort(DrawableInputComparer.Instance);

        List<Drawable> drawablesTempIterate = new(drawables);

        int fakei = 0;

        for (int i = 0; i < drawablesTempIterate.Count; i++) {
            Drawable drawable = drawablesTempIterate[i];
            if (drawable is CompositeDrawable compositeDrawable)
                fakei += RecurseCompositeDrawables(ref drawables, compositeDrawable, fakei);

            fakei++;
        }

        drawables.RemoveAll(
        x => x is CompositeDrawable {
            InvisibleToInput: true
        }
        );

        for (int i = 0; i < drawables.Count; i++) {
            Drawable drawable = drawables[i];

            if (drawable.RealContains(e.Mouse.Position)) {
                if (drawable.Clickable)
                    drawable.Click(true, e);

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

            foreach (InputMethod method in this._registeredInputMethods) {
                foreach (FurballKeyboard kb in method.Keyboards) {
                    temp.AddRange(kb.PressedKeys);
                }
            }

            return temp;
        }
    }

    public List<FurballKeyboard> Keyboards {
        get {
            List<FurballKeyboard> temp = new();

            foreach (InputMethod method in this._registeredInputMethods) {
                temp.AddRange(method.Keyboards);
            }

            return temp;
        }
    }

    public List<FurballMouse> Mice {
        get {
            List<FurballMouse> temp = new();

            foreach (InputMethod method in this._registeredInputMethods) {
                temp.AddRange(method.Mice);
            }

            return temp;
        }
    }

    public readonly List<Keybind> RegisteredKeybinds = new();

    public void RegisterKeybind(Keybind bind) {
        this.RegisteredKeybinds.Add(bind);

        Logger.Log($"Registered keybind for key {bind.Key} (default:{bind.DefaultKey})", LoggerLevelInput.InstanceInfo);
    }

    public void UnregisterKeybind(Keybind bind) {
        if (!this.RegisteredKeybinds.Remove(bind))
            Logger.Log($"Called unregister with a non-registered keybind {bind.Key} (default:{bind.DefaultKey})!", LoggerLevelInput.InstanceWarning);
    }

    /// <summary>
    /// The currently registered InputMethods
    /// </summary>
    private readonly List<InputMethod> _registeredInputMethods = new();

    public IReadOnlyList<InputMethod> RegisteredInputMethods {
        get => this._registeredInputMethods.AsReadOnly();
    }

    /// <summary>
    /// Updates all registered InputMethods and calls the necessary events
    /// </summary>
    public void Update() {
        //Create a copy of all the scroll wheels
        foreach (FurballMouse mouse in this.Mice) {
            mouse.ScrollWheelCache = mouse.ScrollWheel;
            mouse.PositionCache    = mouse.Position;
        }

        for (int i = 0; i < this._registeredInputMethods.Count; i++) {
            InputMethod method = this._registeredInputMethods[i];

            method.Update();
        }

        #region OnKeyUp/Down

        foreach (FurballKeyboard keyboard in this.Keyboards) {
            foreach (char c in keyboard.QueuedTextInputs) {
                this.OnCharInput?.Invoke(this, new CharInputEvent(c, keyboard));
                this.CharInputHandler?.HandleChar(new CharInputEvent(c, keyboard));
            }

            foreach (Key press in keyboard.QueuedKeyPresses) {
                this.OnKeyDown?.Invoke(this, new KeyEventArgs(press, keyboard));
            }

            foreach (Key release in keyboard.QueuedKeyReleases) {
                this.OnKeyUp?.Invoke(this, new KeyEventArgs(release, keyboard));
            }

            keyboard.QueuedTextInputs.Clear();
            keyboard.QueuedKeyPresses.Clear();
            keyboard.QueuedKeyReleases.Clear();
        }

        #endregion

        #region OnMouseUp/Down/Move/Scroll

        foreach (FurballMouse mouse in this.Mice) {
            foreach (MouseButton x in mouse.QueuedButtonPresses) {
                //Set the drag states
                mouse.DragStates[(int)x].Active        = true;
                mouse.DragStates[(int)x].Button        = x;
                mouse.DragStates[(int)x].StartPosition = mouse.Position;

                this.OnMouseDown?.Invoke(this, new MouseButtonEventArgs(x, mouse));
            }
            foreach (MouseButton x in mouse.QueuedButtonReleases) {
                mouse.DragStates[(int)x].Active = false;
                
                this.OnMouseUp?.Invoke(this, new MouseButtonEventArgs(x, mouse));
            }

            if (!mouse.ScrollWheel.Equals(mouse.ScrollWheelCache))
                this.OnMouseScroll?.Invoke(
                this,
                new MouseScrollEventArgs(new Vector2(mouse.ScrollWheel.X - mouse.ScrollWheelCache.X, mouse.ScrollWheel.Y - mouse.ScrollWheelCache.Y), mouse)
                );

            if (mouse.Position != mouse.PositionCache) {
                foreach (FurballMouse.DragState dragState in mouse.DragStates) {
                    if (dragState.Active)
                        this.OnMouseDrag?.Invoke(this, new MouseDragEventArgs(dragState.StartPosition, mouse.PositionCache, mouse.Position, dragState.Button, mouse));
                }

                this.OnMouseMove?.Invoke(this, new MouseMoveEventArgs(mouse.Position, mouse));
            }

            mouse.QueuedButtonPresses.Clear();
            mouse.QueuedButtonReleases.Clear();
        }

        #endregion

    }

    /// <summary>
    /// Registers a new input method and calls its Initialize method
    /// </summary>
    /// <param name="method">The InputMethod to add</param>
    public void RegisterInputMethod(InputMethod method) {
        Profiler.StartProfile($"register_input_method_{method.GetType().Name}");
        this._registeredInputMethods.Add(method);
        method.Initialize();
        Profiler.EndProfileAndPrint($"register_input_method_{method.GetType().Name}");
    }
    /// <summary>
    /// Removes an input method and calls its Dispose method
    /// </summary>
    /// <param name="method">The InputMethod to remove</param>
    public void RemoveInputMethod(InputMethod method) {
        method.Dispose();
        this._registeredInputMethods.Remove(method);
    }

    public bool ControlHeld => (this.HeldKeys.Contains(Key.ControlLeft) || this.HeldKeys.Contains(Key.ControlRight));
    public bool ShiftHeld => (this.HeldKeys.Contains(Key.ShiftLeft) || this.HeldKeys.Contains(Key.ShiftRight));
    public bool AltHeld => (this.HeldKeys.Contains(Key.AltLeft) || this.HeldKeys.Contains(Key.AltRight));

    [NotNull]
    public string Clipboard {
        get {
            foreach (FurballKeyboard keyboard in this.Keyboards)
                return keyboard.GetClipboard();

            return "";
        }
        set {
            foreach (FurballKeyboard keyboard in this.Keyboards) {
                keyboard.SetClipboard(value);
            }
        }
    }
}
