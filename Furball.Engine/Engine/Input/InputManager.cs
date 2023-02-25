using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Input.Events;
using Furball.Engine.Engine.Input.InputMethods;
using Furball.Engine.Engine.Timing;
using JetBrains.Annotations;
using Kettu;
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
    internal void InvokeOnKeyDown(KeyEventArgs args) => this.OnKeyDown?.Invoke(this, args);
    /// <summary>
    /// Called when a key is released
    /// </summary>
    public event EventHandler<KeyEventArgs> OnKeyUp;
    internal void InvokeOnKeyUp(KeyEventArgs args) => this.OnKeyUp?.Invoke(this, args);
    /// <summary>
    /// Called when a mouse button is pressed
    /// </summary>
    public event EventHandler<MouseButtonEventArgs> OnMouseDown;
    internal void InvokeOnMouseDown(MouseButtonEventArgs args) => this.OnMouseDown?.Invoke(this, args);
    /// <summary>
    /// Called when a mouse button is released
    /// </summary>
    public event EventHandler<MouseButtonEventArgs> OnMouseUp;
    internal void InvokeOnMouseUp(MouseButtonEventArgs args) => this.OnMouseUp?.Invoke(this, args);
    /// <summary>
    /// Called when a cursor moves
    /// </summary>
    public event EventHandler<MouseMoveEventArgs> OnMouseMove;
    internal void InvokeOnMouseMove(MouseMoveEventArgs args) => this.OnMouseMove?.Invoke(this, args);
    /// <summary>
    /// Called when a cursor drags
    /// </summary>
    public event EventHandler<MouseDragEventArgs> OnMouseDrag;
    internal void InvokeOnMouseDrag(MouseDragEventArgs args) => this.OnMouseDrag?.Invoke(this, args);
    /// <summary>
    /// Called when a cursor starts a drag
    /// </summary>
    public event EventHandler<MouseDragEventArgs> OnMouseDragStart;
    internal void InvokeOnMouseDragStart(MouseDragEventArgs args) => this.OnMouseDragStart?.Invoke(this, args);
    /// <summary>
    /// Called when a cursor ends a drag
    /// </summary>
    public event EventHandler<MouseDragEventArgs> OnMouseDragEnd;
    internal void InvokeOnMouseDragEnd(MouseDragEventArgs args) => this.OnMouseDragEnd?.Invoke(this, args);
    /// <summary>
    /// Called when the cursor scrolls
    /// </summary>
    public event EventHandler<MouseScrollEventArgs> OnMouseScroll;
    internal void InvokeOnMouseScroll(MouseScrollEventArgs args) => this.OnMouseScroll?.Invoke(this, args);
    /// <summary>
    /// Called when the user types a character
    /// </summary>
    public event EventHandler<CharInputEvent> OnCharInput;
    internal void InvokeOnCharInput(CharInputEvent args) => this.OnCharInput?.Invoke(this, args);
    
    public List<FurballKeyboard> Keyboards = new List<FurballKeyboard>();
    public List<FurballMouse>    Mice      = new List<FurballMouse>();

    /// <summary>
    /// How many control keys are being pressed down
    /// </summary>
    internal int ControlCount = 0;

    /// <summary>
    /// Whether or not the control key is being held
    /// </summary>
    public bool ControlHeld => this.ControlCount != 0;

    /// <summary>
    /// How many shift keys are being pressed down
    /// </summary>
    internal int ShiftCount = 0;

    /// <summary>
    /// Whether or not the shift key is being held
    /// </summary>
    public bool ShiftHeld => this.ShiftCount != 0;

    /// <summary>
    /// How many shift keys are being pressed down
    /// </summary>
    internal int AltCount = 0;

    /// <summary>
    /// Whether or not the shift key is being held
    /// </summary>
    public bool AltHeld => this.AltCount != 0;
    
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
        //If we are not on the input thread, lock the input objects
        bool taken = this.InputObjectsLock.TryEnterWriteLock(1);

        this.InputObjects.Add(inputObject);
        this.InputObjects.Sort(DrawableInputComparer.Instance);

        if (taken)
            this.InputObjectsLock.ExitWriteLock();
    }

    public void RemoveInputObject(InputObject inputObject) {
        bool taken = this.InputObjectsLock.TryEnterWriteLock(1);

        this.InputObjects.Remove(inputObject);

        if (taken)
            this.InputObjectsLock.ExitWriteLock();
    }

    internal readonly List<InputObject> InputObjects      = new List<InputObject>();
    private           bool              _sortInputObjects = false;

    public ReaderWriterLockSlim InputObjectsLock = new ReaderWriterLockSlim();

    readonly FurballMouse[] _isClickedTemp = new FurballMouse[(int)(MouseButton.Button12 + 1)];
    private void CheckInputObjects() {
        bool taken = this.InputObjectsLock.TryEnterUpgradeableReadLock(1);

        if (this._sortInputObjects) {
            this.InputObjects.Sort(DrawableInputComparer.Instance);
            this._sortInputObjects = false;
        }

        bool blocked = false;
        for (int inputIndex = 0; inputIndex < this.InputObjects.Count; inputIndex++) {
            InputObject inputObject = this.InputObjects[inputIndex];
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (inputObject.Depth != inputObject.LastDepth) {
                this._sortInputObjects = true;
                inputObject.LastDepth  = inputObject.Depth;
            }

            bool hovered = false;
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
                            this._isClickedTemp[j] = mouse;
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

            for (int i = 0; i < this._isClickedTemp.Length; i++) {
                FurballMouse.DragState dragState = inputObject.DragStates[i];
                FurballMouse           mouse     = this._isClickedTemp[i];

                //If they were clicked last input frame and are no longer clicked
                if (inputObject.LastClicked[i] && mouse == null) {
                    inputObject.Drawable.Click(false, new MouseButtonEventArgs((MouseButton)i, this._isClickedTemp[i]));
                }

                //If the user is dragging and they let go of the mouse button
                if (dragState.Mouse != null && !dragState.Mouse.PressedButtons[i] && dragState.Active) {
                    //Stop dragging
                    dragState.Active = false;
                    inputObject.Drawable.DragState(
                    false,
                    new MouseDragEventArgs(dragState.StartPosition, dragState.LastPosition, dragState.Mouse.Position, (MouseButton)i, dragState.Mouse)
                    );
                    dragState.Mouse.IsDraggingDrawable = false;
                    dragState.Mouse                    = null;
                }

                //If they were not clicked last input frame and are now clicked, and the user just started clicking that button
                if (!inputObject.LastClicked[i] && mouse != null && mouse.JustPressed[i]) {
                    //If its just clicked, then set the start and last position
                    dragState.StartPosition = mouse.Position;
                    dragState.LastPosition  = mouse.Position;
                    dragState.Mouse         = mouse;

                    inputObject.Drawable.Click(true, new MouseButtonEventArgs((MouseButton)i, this._isClickedTemp[i]));
                }

                //If the user is dragging, and the mouse has moved
                if (dragState.Mouse != null && dragState.LastPosition != dragState.Mouse.Position) {
                    if (!dragState.Active) {
                        //If the mouse is null, that means the mouse button was released, so we can't start dragging, nor continue dragging
                        if (mouse == null || mouse.IsDraggingDrawable)
                            goto no_mouse_skip;

                        dragState.Active = true;
                        inputObject.Drawable.DragState(
                        true,
                        new MouseDragEventArgs(dragState.StartPosition, dragState.LastPosition, dragState.Mouse.Position, (MouseButton)i, dragState.Mouse)
                        );
                        mouse.IsDraggingDrawable = true;
                    }

                    inputObject.Drawable.Drag(
                    new MouseDragEventArgs(dragState.StartPosition, dragState.LastPosition, dragState.Mouse.Position, (MouseButton)i, dragState.Mouse)
                    );
                    dragState.LastPosition = dragState.Mouse.Position;
                }
            no_mouse_skip:

                inputObject.LastClicked[i] = mouse != null;

                this._isClickedTemp[i] = null;
            }
        }

        if (taken)
            this.InputObjectsLock.ExitUpgradeableReadLock();
    }

    internal int    CountedInputFrames = 0;
    internal double LastInputFrameTime = 0;

    public readonly List<InputMethod> InputMethods = new List<InputMethod>();
    
    public void AddInputMethod(InputMethod inputMethod) {
        this.InputMethods.Add(inputMethod);
    }
    
    public void RemoveInputMethod(InputMethod inputMethod) {
        inputMethod.Remove = true;
    }
    
    private void InputMethodKeyboardAdded(object sender, FurballKeyboard e) {
        this.Keyboards.Add(e);
    }
    
    private void InputMethodKeyboardRemoved(object sender, FurballKeyboard e) {
        this.Keyboards.Remove(e);
    }
    
    private void InputMethodMouseAdded(object sender, FurballMouse e) {
        this.Mice.Add(e);
    }
    
    private void InputMethodMouseRemoved(object sender, FurballMouse e) {
        this.Mice.Remove(e);
    }
    
    private void Run() {
        using HighResolutionClock clock = new HighResolutionClock(TimeSpan.FromMilliseconds(1));

        Stopwatch stopwatch = Stopwatch.StartNew();

        while (this._run) {
            // Console.WriteLine($"Input frame clock run");
            double start = stopwatch.Elapsed.TotalMilliseconds;

            for (int i = 0; i < this.InputMethods.Count; i++) {
                InputMethod inputMethod = this.InputMethods[i];
                //If the input method is not initialized, do so and register our events
                if (!inputMethod.IsInitialized) {
                    inputMethod.IsInitialized = true;

                    inputMethod.KeyboardAdded   += this.InputMethodKeyboardAdded;
                    inputMethod.KeyboardRemoved += this.InputMethodKeyboardRemoved;
                    inputMethod.MouseAdded      += this.InputMethodMouseAdded;
                    inputMethod.MouseRemoved    += this.InputMethodMouseRemoved;

                    inputMethod.Initialize(this);
                } 
                //If the input method is scheduled to be removed, do so, and unregister our events
                else if (inputMethod.Remove) {
                    inputMethod.IsInitialized = false;
                    inputMethod.Remove        = false;
                    
                    inputMethod.Dispose();

                    inputMethod.KeyboardAdded   -= this.InputMethodKeyboardAdded;
                    inputMethod.KeyboardRemoved -= this.InputMethodKeyboardRemoved;
                    inputMethod.MouseAdded      -= this.InputMethodMouseAdded;
                    inputMethod.MouseRemoved    -= this.InputMethodMouseRemoved;

                    this.InputMethods.Remove(inputMethod);
                }
                
                inputMethod.Update();
            }

            this.CheckInputObjects();

            //Wait the clock 
            if (this._run) {
                Interlocked.Increment(ref this.CountedInputFrames);
                this.LastInputFrameTime = stopwatch.Elapsed.TotalMilliseconds - start;
                clock.WaitFrame();
                // Console.WriteLine($"Input frame delta {elapsed:N2}ms:{1000d / elapsed:N2} per second");
            }
        }
    }

    internal readonly List<Keybind> RegisteredKeybinds = new List<Keybind>();

    public void RegisterKeybind(Keybind bind) {
        if (!this.RegisteredKeybinds.Contains(bind))
            this.RegisteredKeybinds.Add(bind);
    }

    public void UnregisterKeybind(Keybind bind) {
        this.RegisteredKeybinds.Remove(bind);
    }
}
