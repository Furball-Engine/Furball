using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Channels;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Input.Events;
using Furball.Engine.Engine.Timing;
using Furball.Vixie.WindowManagement;
using JetBrains.Annotations;
using Kettu;
using OneOf;
using Silk.NET.Core;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input;

public class InputManager {
    private Thread _thread;

    private bool _run = true;

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
    /// Called when a cursor drags
    /// </summary>
    public event EventHandler<MouseDragEventArgs> OnMouseDrag;
    /// <summary>
    /// Called when a cursor starts a drag
    /// </summary>
    public event EventHandler<MouseDragEventArgs> OnMouseDragStart;
    /// <summary>
    /// Called when a cursor ends a drag
    /// </summary>
    public event EventHandler<MouseDragEventArgs> OnMouseDragEnd;
    /// <summary>
    /// Called when the cursor scrolls
    /// </summary>
    public event EventHandler<MouseScrollEventArgs> OnMouseScroll;
    /// <summary>
    /// Called when the user types a character
    /// </summary>
    public event EventHandler<CharInputEvent> OnCharInput;

    public List<FurballKeyboard> Keyboards = new List<FurballKeyboard>();
    public List<FurballMouse>    Mice      = new List<FurballMouse>();

    /// <summary>
    /// How many control keys are being pressed down
    /// </summary>
    private int _controlCount = 0;

    /// <summary>
    /// Whether or not the control key is being held
    /// </summary>
    public bool ControlHeld => this._controlCount != 0;

    /// <summary>
    /// How many shift keys are being pressed down
    /// </summary>
    private int _shiftCount = 0;

    /// <summary>
    /// Whether or not the shift key is being held
    /// </summary>
    public bool ShiftHeld => this._shiftCount != 0;

    public InputManager() {
        this._thread = new(this.Run);
    }

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
        if (!this.CharInputHandler?.SaveInStack ?? false) {
            Logger.Log($"{this.CharInputHandler.GetType().Name} will not be saved on the stack!", LoggerLevelInput.InstanceInfo);
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
        if (wasEmpty) {
            this.Keyboards.ForEach(x => x.BeginInput());
        }

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

    public void Start() {
        this._thread.Start();
    }

    public void End() {
        this._run = false;
        this._thread.Join(1000);
    }

    public void Update() {
        // ReSharper disable once SuggestVarOrType_Elsewhere
        var writer = this._channelToInput.Writer;
        if (FurballGame.Instance.WindowManager is SilkWindowManager silkWindowManager) {
            IReadOnlyList<IMouse> mice = silkWindowManager.InputContext.Mice;

            foreach (IMouse mouse in mice) {
                if (mouse.ScrollWheels[0].X != 0)
                    writer.WriteAsync(
                    new MouseScrollUpdate {
                        X     = mouse.ScrollWheels[0].X,
                        Mouse = mouse
                    }
                    );
                if (mouse.ScrollWheels[0].Y != 0)
                    writer.WriteAsync(
                    new MouseScrollUpdate {
                        Y     = mouse.ScrollWheels[0].Y,
                        Mouse = mouse
                    }
                    );
            }
        }
    }

    private struct MouseScrollUpdate {
        public IMouse Mouse;
        public float  X;
        public float  Y;
    }

    private struct SilkKeyChar {
        public IKeyboard Keyboard;
        public char      Character;
    }

    public int InputObjectIndex = 0;

    public InputObject CreateInputObject(Drawable drawable) {
        int index = this.InputObjectIndex;
        unchecked {
            this.InputObjectIndex++;
        }

        InputObject inputObject = new InputObject(index);

        //Set the position of the input object
        Vector2 realPos = drawable.RealPosition;
        inputObject.Position.X = realPos.X;
        inputObject.Position.Y = realPos.Y;

        //Set the size of the input object
        Vector2 realSize = drawable.RealSize;
        inputObject.Size.X = realSize.X;
        inputObject.Size.Y = realSize.Y;

        inputObject.Drawable = drawable;
        inputObject.Depth    = drawable.Depth;

        return inputObject;
    }

    public void AddInputObject(InputObject inputObject) {
        bool taken = false;
        this.Lock.Enter(ref taken);

        this._inputObjects.Add(inputObject);
        this._inputObjects.Sort(DrawableInputComparer.Instance);

        if (taken)
            this.Lock.Exit();
    }

    public void RemoveInputObject(InputObject inputObject) {
        bool taken = false;
        this.Lock.Enter(ref taken);

        this._inputObjects.Remove(inputObject);

        if (taken)
            this.Lock.Exit();
    }

    private readonly List<InputObject> _inputObjects     = new List<InputObject>();
    private          bool              _sortInputObjects = false;

    public BreakneckLock Lock = new BreakneckLock();

    private readonly Channel<OneOf<MouseScrollUpdate, SilkKeyChar>> _channelToInput = Channel.CreateUnbounded<OneOf<MouseScrollUpdate, SilkKeyChar>>();

    private void CheckInputObjects() {
        bool taken = false;
        this.Lock.Enter(ref taken);

        if (this._sortInputObjects) {
            this._inputObjects.Sort(DrawableInputComparer.Instance);
            this._sortInputObjects = false;
        }

        bool blocked = false;
        foreach (InputObject inputObject in this._inputObjects) {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (inputObject.Depth != inputObject.LastDepth) {
                this._sortInputObjects = true;
                inputObject.LastDepth  = inputObject.Depth;
            }

            FurballMouse[] isClicked = new FurballMouse[(int)(MouseButton.Button12 + 1)];
            bool           hovered   = false;
            for (int i = 0; i < this.Mice.Count; i++) {
                FurballMouse mouse = this.Mice[i];
                if (inputObject.Contains(mouse.Position)) {
                    if (blocked)
                        break;

                    hovered = true;

                    //Iterate through all buttons on the mouse
                    for (int j = 0; j < mouse.PressedButtons.Length; j++) {
                        bool pressed = mouse.PressedButtons[j];
                        //If the mouse button is being pressed over the input object, mark it as such
                        if (pressed) {
                            isClicked[j] = mouse;
                        }
                    }

                    blocked = true;
                }
            }

            //Check for new hover
            if (!inputObject.LastHovered && hovered) {
                inputObject.Drawable.Hover(true);
            }

            //Check for new unhover
            if (inputObject.LastHovered && !hovered) {
                inputObject.Drawable.Hover(false);
            }

            inputObject.LastHovered = hovered;

            for (int i = 0; i < isClicked.Length; i++) {
                FurballMouse.DragState dragState = inputObject.DragStates[i];
                FurballMouse           mouse     = isClicked[i];

                //If they were clicked last input frame and are no longer clicked
                if (inputObject.LastClicked[i] && mouse == null) {
                    inputObject.Drawable.Click(false, new MouseButtonEventArgs((MouseButton)i, isClicked[i]));
                }

                //If the user is dragging and they let go of the mouse button
                if (dragState.Mouse != null && !dragState.Mouse.PressedButtons[i] && dragState.Active) {
                    //Stop dragging
                    dragState.Active = false;
                    inputObject.Drawable.DragState(false, new MouseDragEventArgs(dragState.StartPosition, dragState.LastPosition, dragState.Mouse.Position, (MouseButton)i, dragState.Mouse));
                    dragState.Mouse.IsDraggingDrawable = false;
                    dragState.Mouse                    = null;
                }

                //If they were not clicked last input frame and are now clicked
                if (!inputObject.LastClicked[i] && mouse != null) {
                    //If its just clicked, then set the start and last position
                    dragState.StartPosition = mouse.Position;
                    dragState.LastPosition  = mouse.Position;
                    dragState.Mouse         = mouse;

                    inputObject.Drawable.Click(true, new MouseButtonEventArgs((MouseButton)i, isClicked[i]));
                }

                //If the user is dragging, and the mouse has moved
                if (dragState.Mouse != null && dragState.LastPosition != dragState.Mouse.Position) {
                    if (!dragState.Active) {
                        //If the mouse is null, that means the mouse button was released, so we can't start dragging, nor continue dragging
                        if (mouse == null || mouse.IsDraggingDrawable)
                            goto no_mouse_skip;
                        
                        dragState.Active = true;
                        inputObject.Drawable.DragState(true, new MouseDragEventArgs(dragState.StartPosition, dragState.LastPosition, dragState.Mouse.Position, (MouseButton)i, dragState.Mouse));
                        mouse.IsDraggingDrawable = true;
                    }
                    
                    inputObject.Drawable.Drag(new MouseDragEventArgs(dragState.StartPosition, dragState.LastPosition, dragState.Mouse.Position, (MouseButton)i, dragState.Mouse));
                    dragState.LastPosition = dragState.Mouse.Position;
                }
no_mouse_skip:
                
                inputObject.LastClicked[i] = mouse != null;
            }
        }

        if (taken)
            this.Lock.Exit();
    }

    private void Run() {
        using HighResolutionClock clock = new HighResolutionClock(TimeSpan.FromMilliseconds(10));

        // ReSharper disable once SuggestVarOrType_Elsewhere
        var reader = this._channelToInput.Reader;

        // Stopwatch stopwatch = Stopwatch.StartNew();

        IReadOnlyList<IMouse>    silkMice      = new List<IMouse>();
        IReadOnlyList<IKeyboard> silkKeyboards = new List<IKeyboard>();
        if (FurballGame.Instance.WindowManager is SilkWindowManager silkWindowManager) {
            silkMice      = silkWindowManager.InputContext.Mice;
            silkKeyboards = silkWindowManager.InputContext.Keyboards;
        }

        foreach (IMouse mouse in silkMice) {
            this.Mice.Add(
            new FurballMouse() {
                Name = mouse.Name
            }
            );
        }

        foreach (IKeyboard keyboard in silkKeyboards) {
            this.Keyboards.Add(
            new FurballKeyboard {
                Name         = keyboard.Name,
                GetClipboard = () => keyboard.ClipboardText,
                SetClipboard = s => keyboard.ClipboardText = s,
                BeginInput   = () => keyboard.BeginInput(),
                EndInput     = () => keyboard.EndInput()
            }
            );

            keyboard.KeyChar += HandleSilkKeyChar;
        }

        bool[] workingMouseButtons = new bool[(int)(MouseButton.Button12 + 1)];
        bool[] workingKeyboardKeys = new bool[(int)(Key.Menu + 1)];

        // double start = stopwatch.Elapsed.TotalMilliseconds;
        while (this._run) {
            // Console.WriteLine($"Input frame clock run");

            // ReSharper disable once SuggestVarOrType_Elsewhere
            while (reader.TryRead(out var item)) {
                item.Switch(
                update => {
                    for (int i = 0; i < silkMice.Count; i++) {
                        IMouse mouse = silkMice[i];
                        if (mouse == update.Mouse) {
                            this.Mice[i].ScrollWheel.X += update.X;
                            this.Mice[i].ScrollWheel.Y += update.Y;

                            this.OnMouseScroll?.Invoke(this, new MouseScrollEventArgs(new Vector2(update.X, update.Y), this.Mice[i]));

                            break;
                        }
                    }
                },
                silkCharEvent => {
                    for (int i = 0; i < silkKeyboards.Count; i++) {
                        IKeyboard silkKeyboard = silkKeyboards[i];

                        if (silkKeyboard != silkCharEvent.Keyboard)
                            continue;

                        CharInputEvent ev = new CharInputEvent(silkCharEvent.Character, this.Keyboards[i]);

                        this.OnCharInput?.Invoke(this, ev);
                        this.CharInputHandler?.HandleChar(ev);
                    }

                }
                );
            }

            for (int i = 0; i < this.Mice.Count; i++) {
                FurballMouse mouse = this.Mice[i];

                if (i < silkMice.Count) {
                    IMouse silkMouse = silkMice[i];

                    this.SilkMouseButtonCheck(workingMouseButtons, silkMouse, mouse);

                    Vector2 newPosition = silkMouse.Position / FurballGame.VerticalRatio;
                    this.SilkMousePositionCheck(newPosition, mouse);

                    mouse.Position = newPosition;
                }
            }

            for (int i = 0; i < this.Keyboards.Count; i++) {
                FurballKeyboard keyboard = this.Keyboards[i];

                if (i < silkKeyboards.Count) {
                    IKeyboard silkKeyboard = silkKeyboards[i];

                    this.SilkKeyboardButtonCheck(workingKeyboardKeys, silkKeyboard, keyboard);
                }
            }

            this.CheckInputObjects();

            //Wait the clock 
            if (this._run) {
                clock.WaitFrame();
                // double elapsed = stopwatch.Elapsed.TotalMilliseconds - start;
                // Console.WriteLine($"Input frame delta {elapsed:N2}ms:{1000d / elapsed:N2} per second");
            }
            // start = stopwatch.Elapsed.TotalMilliseconds;
        }
    }

    private void HandleSilkKeyChar(IKeyboard keyboard, char character) {
        //Tell the input thread that we have a new key event from a silk keyboard
        _ = this._channelToInput.Writer.WriteAsync(
        new SilkKeyChar {
            Keyboard  = keyboard,
            Character = character
        }
        );
    }

    /// <summary>
    /// Used to cached Enum.IsDefined calls
    /// </summary>
    private static readonly bool[] DefinedCache = new bool[(int)(Key.Menu + 1)];

    static InputManager() {
        for (int i = 0; i < DefinedCache.Length; i++) {
            DefinedCache[i] = Enum.IsDefined(typeof(Key), i);
        }
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private void SilkKeyboardButtonCheck(bool[] workingKeyboardKeys, IKeyboard silkKeyboard, FurballKeyboard keyboard) {
        for (int j = 0; j < workingKeyboardKeys.Length; j++) {
            //If its not defined in the enum, just continue and do the next key
            if (!DefinedCache[j])
                continue;

            Key key = (Key)j;

            //The current cached state of the pressed key
            bool cur = keyboard.PressedKeys[j];

            //The actual state of the key 
            bool pressed = silkKeyboard.IsKeyPressed(key);

            //Set the pressed state of the keyboard before sending the event,
            //to prevent the event being processed before the actual update happens to the keyboard state
            keyboard.PressedKeys[j] = pressed;

            //If the key is pressed
            if (pressed) {
                //Set the working key to true
                workingKeyboardKeys[j] = true;

                //If the key was not pressed last frame
                if (!cur) {
                    this.OnKeyDown?.Invoke(this, new KeyEventArgs(key, keyboard));

                    foreach (Keybind bind in this._registeredKeybinds) {
                        // ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (Key bindModifier in bind.Modifiers) {
                            //If one of the modifiers isn't pressed, return out
                            if (!keyboard.IsKeyPressed(bindModifier))
                                return;
                        }

                        if (bind.Key == key) {
                            bind.OnPressed?.Invoke(new KeyEventArgs(key, keyboard));
                        }
                    }

                    switch (key) {
                        case Key.ControlLeft or Key.ControlRight:
                            this._controlCount++;
                            break;
                        case Key.ShiftLeft or Key.ShiftRight:
                            this._shiftCount++;
                            break;
                    }

                }
            } else {
                //Set the working key to false
                workingKeyboardKeys[j] = false;

                //If the key was pressed last frame
                if (cur) {
                    this.OnKeyUp?.Invoke(this, new KeyEventArgs(key, keyboard));

                    switch (key) {
                        case Key.ControlLeft or Key.ControlRight:
                            this._controlCount--;
                            break;
                        case Key.ShiftLeft or Key.ShiftRight:
                            this._shiftCount--;
                            break;
                    }

                }
            }

        }
    }

    private void SilkMouseButtonCheck(
        // ReSharper disable once SuggestBaseTypeForParameter
        bool[] newButtons, IMouse silkMouse, FurballMouse mouse
    ) {
        for (int j = 0; j < newButtons.Length; j++) {
            if (silkMouse.IsButtonPressed((MouseButton)j)) {
                newButtons[j] = true;

                if (!mouse.PressedButtons[j]) {
                    this.OnMouseDown?.Invoke(this, new MouseButtonEventArgs((MouseButton)j, mouse));
                }
            } else {
                newButtons[j] = false;

                if (mouse.PressedButtons[j]) {
                    this.OnMouseUp?.Invoke(this, new MouseButtonEventArgs((MouseButton)j, mouse));
                }
            }

            mouse.PressedButtons[j] = newButtons[j];
        }
    }

    private void SilkMousePositionCheck(Vector2 newPosition, FurballMouse mouse) {
        if (newPosition != mouse.Position) {
            for (int i = 0; i < mouse.PressedButtons.Length; i++) {
                bool                   button    = mouse.PressedButtons[i];
                FurballMouse.DragState dragState = mouse.DragStates[i];

                //If the user is holding the button down,
                if (button) {
                    //If a drag is active
                    if (dragState.Active) {
                        //Send the event that the mouse moved with the drag
                        this.OnMouseDrag?.Invoke(this, new MouseDragEventArgs(dragState.StartPosition, mouse.Position, newPosition, (MouseButton)i, mouse));
                    } else {
                        //Else, start a new drag
                        dragState.Active        = true;
                        dragState.StartPosition = newPosition;

                        this.OnMouseDragStart?.Invoke(this, new MouseDragEventArgs(dragState.StartPosition, mouse.Position, newPosition, (MouseButton)i, mouse));
                    }
                } else {
                    //Else, if the drag is active
                    if (dragState.Active) {
                        //End the drag
                        dragState.Active = false;

                        this.OnMouseDragEnd?.Invoke(this, new MouseDragEventArgs(dragState.StartPosition, mouse.Position, newPosition, (MouseButton)i, mouse));
                    }
                }
            }

            this.OnMouseMove?.Invoke(this, new MouseMoveEventArgs(newPosition, mouse));
        }
    }

    private readonly List<Keybind> _registeredKeybinds = new List<Keybind>();

    public void RegisterKeybind(Keybind bind) {
        if (!this._registeredKeybinds.Contains(bind))
            this._registeredKeybinds.Add(bind);
    }

    public void UnregisterKeybind(Keybind bind) {
        this._registeredKeybinds.Remove(bind);
    }
}
