using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Channels;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Input.Events;
using Furball.Engine.Engine.Timing;
using Furball.Vixie.WindowManagement;
using JetBrains.Annotations;
using Kettu;
using OneOf;
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
    /// Called when a cursor moves
    /// </summary>
    public event EventHandler<MouseDragEventArgs> OnMouseDrag;
    /// <summary>
    /// Called when the cursor scrolls
    /// </summary>
    public event EventHandler<MouseScrollEventArgs> OnMouseScroll;
    public event EventHandler<CharInputEvent> OnCharInput;

    public bool ControlHeld;

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
            // this.Keyboards.ForEach(x => x.BeginInput());
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
            // this.Keyboards.ForEach(x => x.EndInput());
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
        ChannelWriter<OneOf<MouseScrollUpdate>> writer = this._channelToInput.Writer;
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

        // ReSharper disable once SuggestVarOrType_Elsewhere
        var reader = this._channel.Reader;

        // ReSharper disable once SuggestVarOrType_Elsewhere
        while (reader.TryRead(out var item)) {
            item.Switch(
            mouseMoveEvent => {
                Console.WriteLine($"Mouse move event: {mouseMoveEvent.Position}");
                this.OnMouseMove?.Invoke(this, new MouseMoveEventArgs(mouseMoveEvent.Position, mouseMoveEvent.MouseId));
            },
            mouseDownEvent => {
                Console.WriteLine($"Mouse down event: {mouseDownEvent.Button}");
                this.OnMouseDown?.Invoke(this, new MouseButtonEventArgs(mouseDownEvent.Button, mouseDownEvent.MouseId));
            },
            mouseUpEvent => {
                Console.WriteLine($"Mouse up event: {mouseUpEvent.Button}");
                this.OnMouseUp?.Invoke(this, new MouseButtonEventArgs(mouseUpEvent.Button, mouseUpEvent.MouseId));
            },
            mouseScrollEvent => {
                Console.WriteLine($"Mouse scroll event: {mouseScrollEvent.X}:{mouseScrollEvent.Y}");
                this.OnMouseScroll?.Invoke(this, new MouseScrollEventArgs(new Vector2(mouseScrollEvent.X, mouseScrollEvent.Y), mouseScrollEvent.MouseId));
            },
            keyDownEvent => {
                Console.WriteLine($"Keyboard down event: {keyDownEvent.Key}");
                this.OnKeyDown?.Invoke(this, new KeyEventArgs(keyDownEvent.Key, keyDownEvent.Keyboard));
                
                foreach (Keybind bind in this._registeredKeybinds) {
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (Key bindModifier in bind.Modifiers) {
                        //If one of the modifiers isn't pressed, return out
                        if (!keyDownEvent.Keyboard.IsKeyPressed(bindModifier))
                            return;
                    }

                    if (bind.Key == keyDownEvent.Key) {
                        bind.OnPressed?.Invoke(new KeyEventArgs(keyDownEvent.Key, keyDownEvent.Keyboard)); 
                    }
                }
            },
            keyUpEvent => {
                Console.WriteLine($"Keyboard up event: {keyUpEvent.Key}");
                this.OnKeyUp?.Invoke(this, new KeyEventArgs(keyUpEvent.Key, keyUpEvent.Keyboard));
            }
            );
        }
    }

    private struct MouseMoveEvent {
        public int     MouseId;
        public Vector2 Position;
    }

    private struct KeyDownEvent {
        public FurballKeyboard Keyboard;
        public Key Key;
    }

    private struct KeyUpEvent {
        public FurballKeyboard Keyboard;
        public Key Key;
    }

    private struct MouseDownEvent {
        public int         MouseId;
        public MouseButton Button;
    }

    private struct MouseUpEvent {
        public int         MouseId;
        public MouseButton Button;
    }

    private struct MouseScrollEvent {
        public int   MouseId;
        public float X;
        public float Y;
    }

    private readonly Channel<OneOf<MouseMoveEvent, MouseDownEvent, MouseUpEvent, MouseScrollEvent, KeyDownEvent, KeyUpEvent>> _channel =
        Channel.CreateUnbounded<OneOf<MouseMoveEvent, MouseDownEvent, MouseUpEvent, MouseScrollEvent, KeyDownEvent, KeyUpEvent>>();

    private struct MouseScrollUpdate {
        public IMouse Mouse;
        public float  X;
        public float  Y;
    }

    private readonly Channel<OneOf<MouseScrollUpdate>> _channelToInput = Channel.CreateUnbounded<OneOf<MouseScrollUpdate>>();

    private void Run() {
        using HighResolutionClock clock = new HighResolutionClock(TimeSpan.FromMilliseconds(10));

        // ReSharper disable once SuggestVarOrType_Elsewhere
        var reader = this._channelToInput.Reader;

        Stopwatch stopwatch = Stopwatch.StartNew();

        // ReSharper disable once SuggestVarOrType_Elsewhere
        var writer = this._channel.Writer;

        List<FurballMouse>    mice      = new List<FurballMouse>();
        List<FurballKeyboard> keyboards = new List<FurballKeyboard>();

        IReadOnlyList<IMouse>    silkMice      = new List<IMouse>();
        IReadOnlyList<IKeyboard> silkKeyboards = new List<IKeyboard>();
        if (FurballGame.Instance.WindowManager is SilkWindowManager silkWindowManager) {
            silkMice      = silkWindowManager.InputContext.Mice;
            silkKeyboards = silkWindowManager.InputContext.Keyboards;
        }

        foreach (IMouse mouse in silkMice) {
            mice.Add(
            new FurballMouse() {
                Name = mouse.Name
            }
            );
        }

        foreach (IKeyboard keyboard in silkKeyboards) {
            keyboards.Add(
            new FurballKeyboard {
                Name         = keyboard.Name,
                GetClipboard = () => keyboard.ClipboardText,
                SetClipboard = s => keyboard.ClipboardText = s,
                BeginInput   = () => keyboard.BeginInput(),
                EndInput     = () => keyboard.EndInput()
            }
            );
        }

        bool[] workingMouseButtons = new bool[(int)(MouseButton.Button12 + 1)];
        bool[] workingKeyboardKeys = new bool[(int)(Key.Menu + 1)];

        double start = stopwatch.Elapsed.TotalMilliseconds;
        while (this._run) {
            // Console.WriteLine($"Input frame clock run");

            while (reader.TryRead(out OneOf<MouseScrollUpdate> item)) {
                item.Switch(
                update => {
                    for (int i = 0; i < silkMice.Count; i++) {
                        IMouse mouse = silkMice[i];
                        if (mouse == update.Mouse) {
                            mice[i].ScrollWheel.X += update.X;
                            mice[i].ScrollWheel.Y += update.Y;

                            writer.WriteAsync(
                            new MouseScrollEvent {
                                MouseId = i,
                                X       = update.X,
                                Y       = update.Y
                            }
                            );
                            break;
                        }
                    }
                }
                );
            }

            for (int i = 0; i < mice.Count; i++) {
                FurballMouse mouse = mice[i];

                if (i < silkMice.Count) {
                    IMouse silkMouse = silkMice[i];

                    SilkMouseButtonCheck(workingMouseButtons, silkMouse, mouse, writer, i);

                    Vector2 newPosition = silkMouse.Position;
                    SilkMousePositionCheck(newPosition, mouse, writer, i);

                    mouse.Position = newPosition;
                }
            }

            for (int i = 0; i < keyboards.Count; i++) {
                FurballKeyboard keyboard = keyboards[i];

                if (i < silkKeyboards.Count) {
                    IKeyboard silkKeyboard = silkKeyboards[i];

                    SilkKeyboardButtonCheck(workingKeyboardKeys, silkKeyboard, keyboard, writer, i);
                }
            }

            //Wait the clock 
            if (this._run) {
                clock.WaitFrame();
                double elapsed = stopwatch.Elapsed.TotalMilliseconds - start;
                // Console.WriteLine($"Input frame delta {elapsed:N2}ms:{1000d / elapsed:N2} per second");
            }
            start = stopwatch.Elapsed.TotalMilliseconds;
        }
    }

    /// <summary>
    /// Used to cached Enum.IsDefined calls
    /// </summary>
    private byte[] _definedCache = new byte[(int)(Key.Menu + 1)];

    // ReSharper disable once SuggestBaseTypeForParameter
    private void SilkKeyboardButtonCheck(
        bool[] workingKeyboardKeys, IKeyboard silkKeyboard, FurballKeyboard keyboard,
        ChannelWriter<OneOf<MouseMoveEvent, MouseDownEvent, MouseUpEvent, MouseScrollEvent, KeyDownEvent, KeyUpEvent>> writer, int i
    ) {
        for (int j = 0; j < workingKeyboardKeys.Length; j++) {
            //If this entry is undefined, get the value
            if (this._definedCache[j] == 0) {
                this._definedCache[j] = (byte)(Enum.IsDefined(typeof(Key), j) ? 2 : 1);
            }
            
            //If it is equal to 1, that means that it is not defined, so `continue;`
            if (this._definedCache[j] == 1)
                continue;

            //The current cached state of the pressed key
            bool cur = keyboard.PressedKeys[j];

            //The actual state of the key 
            bool pressed = silkKeyboard.IsKeyPressed((Key)j);
            
            //Set the pressed state of the keyboard before sending the event,
            //to prevent the event being processed before the actual update happens to the keyboard state
            keyboard.PressedKeys[j] = pressed;

            //If the key is pressed
            if (pressed) {
                //Set the working key to true
                workingKeyboardKeys[j] = true;

                //If the key was not pressed last frame
                if (!cur) {
                    //Write to the stream that a key was pressed
                    writer.WriteAsync(
                    new KeyDownEvent {
                        Key        = (Key)j,
                        Keyboard = keyboard
                    }
                    );
                }
            } else {
                //Set the working key to false
                workingKeyboardKeys[j] = false;

                //If the key was pressed last frame
                if (cur) {
                    //Write to the stream that a key was released
                    writer.WriteAsync(
                    new KeyUpEvent {
                        Key        = (Key)j,
                        Keyboard = keyboard
                    }
                    );
                }
            }

        }
    }

    private static void SilkMouseButtonCheck(
        // ReSharper disable once SuggestBaseTypeForParameter
        bool[] newButtons, IMouse silkMouse, FurballMouse mouse,
        ChannelWriter<OneOf<MouseMoveEvent, MouseDownEvent, MouseUpEvent, MouseScrollEvent, KeyDownEvent, KeyUpEvent>> writer, int i
    ) {
        for (int j = 0; j < newButtons.Length; j++) {
            if (silkMouse.IsButtonPressed((MouseButton)j)) {
                newButtons[j] = true;

                if (!mouse.PressedButtons[j]) {
                    writer.WriteAsync(
                    new MouseDownEvent {
                        MouseId = i,
                        Button  = (MouseButton)j
                    }
                    );
                }
            } else {
                newButtons[j] = false;

                if (mouse.PressedButtons[j]) {
                    writer.WriteAsync(
                    new MouseUpEvent {
                        MouseId = i,
                        Button  = (MouseButton)j
                    }
                    );
                }
            }

            mouse.PressedButtons[j] = newButtons[j];
        }
    }
    private static void SilkMousePositionCheck(
        Vector2 newPosition, FurballMouse mouse, ChannelWriter<OneOf<MouseMoveEvent, MouseDownEvent, MouseUpEvent, MouseScrollEvent, KeyDownEvent, KeyUpEvent>> writer,
        int     i
    ) {
        if (newPosition != mouse.Position) {
            writer.WriteAsync(
            new MouseMoveEvent {
                MouseId  = i,
                Position = newPosition
            }
            );
        }
    }

    private readonly List<Keybind> _registeredKeybinds = new List<Keybind>();
    
    public void RegisterKeybind(Keybind bind) {
        if(!this._registeredKeybinds.Contains(bind))
            this._registeredKeybinds.Add(bind);
    }

    public void UnregisterKeybind(Keybind bind) {
        this._registeredKeybinds.Remove(bind);
    }
}
