using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using Silk.NET.Input.Extensions;

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
                    writer.WriteAsync(new MouseScrollUpdate {
                        X = mouse.ScrollWheels[0].X, 
                        Mouse = mouse
                    });
                if (mouse.ScrollWheels[0].Y != 0)
                    writer.WriteAsync(new MouseScrollUpdate {
                        Y = mouse.ScrollWheels[0].Y, 
                        Mouse = mouse
                    }); 
            }
        }
        
        // ReSharper disable once SuggestVarOrType_Elsewhere
        var reader = this._channel.Reader;

        // ReSharper disable once SuggestVarOrType_Elsewhere
        while (reader.TryRead(out var item)) {
            switch (item.Value) {
                case MouseMoveEvent mouseMoveEvent: {
                    Console.WriteLine($"Mouse move event: {mouseMoveEvent.Position}");
                    this.OnMouseMove?.Invoke(this, new MouseMoveEventArgs(mouseMoveEvent.Position, null));//TODO: mouse
                    break;
                }
                case MouseUpEvent mouseUpEvent: {
                    Console.WriteLine($"Mouse up event: {mouseUpEvent.Button}");
                    this.OnMouseUp?.Invoke(this, new MouseButtonEventArgs(mouseUpEvent.Button, null));//TODO: mouse
                    break;
                }
                case MouseDownEvent mouseDownEvent: {
                    Console.WriteLine($"Mouse down event: {mouseDownEvent.Button}");
                    this.OnMouseDown?.Invoke(this, new MouseButtonEventArgs(mouseDownEvent.Button, null));//TODO: mouse
                    break;
                }
                case MouseScrollEvent mouseScrollEvent: {
                    Console.WriteLine($"Mouse scroll event: {mouseScrollEvent.X}:{mouseScrollEvent.Y}");
                    this.OnMouseScroll?.Invoke(this, new MouseScrollEventArgs(new Vector2(mouseScrollEvent.X, mouseScrollEvent.Y), null));//TODO: mouse
                    break;
                }
            }
        }
    }

    private struct MouseMoveEvent {
        public int     MouseId;
        public Vector2 Position;
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

    private readonly Channel<OneOf<MouseMoveEvent, MouseDownEvent, MouseUpEvent, MouseScrollEvent>> _channel =
        Channel.CreateUnbounded<OneOf<MouseMoveEvent, MouseDownEvent, MouseUpEvent, MouseScrollEvent>>();

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

        bool[] newButtons = new bool[(int)(MouseButton.Button12 + 1)];

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
                            
                            writer.WriteAsync(new MouseScrollEvent {
                                MouseId = i, 
                                X       = update.X, 
                                Y       = update.Y
                            });
                            break;
                        }
                    }
                });
            }

            for (int i = 0; i < mice.Count; i++) {
                FurballMouse mouse = mice[i];

                if (i < silkMice.Count) {
                    IMouse silkMouse = silkMice[i];

                    SilkMouseButtonCheck(newButtons, silkMouse, mouse, writer, i);

                    Vector2 newPosition = silkMouse.Position;
                    SilkMousePositionCheck(newPosition, mouse, writer, i);

                    mouse.Position = newPosition;
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

    private static void SilkMouseWheelCheck(
        Silk.NET.Input.ScrollWheel wheel, FurballMouse mouse, ChannelWriter<OneOf<MouseMoveEvent, MouseDownEvent, MouseUpEvent, MouseScrollEvent>> writer, int i
    ) {
        if (wheel.X != mouse.ScrollWheel.X) {
            writer.WriteAsync(
            new MouseScrollEvent {
                MouseId = i,
                X       = wheel.X - mouse.ScrollWheel.X
            }
            );
        }
        if (wheel.Y != mouse.ScrollWheel.Y) {
            writer.WriteAsync(
            new MouseScrollEvent {
                MouseId = i,
                Y       = wheel.Y - mouse.ScrollWheel.Y
            }
            );
        }
    }

    private static void SilkMouseButtonCheck(
        bool[] newButtons, IMouse silkMouse, FurballMouse mouse, ChannelWriter<OneOf<MouseMoveEvent, MouseDownEvent, MouseUpEvent, MouseScrollEvent>> writer, int i
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
        Vector2 newPosition, FurballMouse mouse, ChannelWriter<OneOf<MouseMoveEvent, MouseDownEvent, MouseUpEvent, MouseScrollEvent>> writer, int i
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

    public void RegisterKeybind(Keybind toggleDebugOverlay) {
        // throw new NotImplementedException();
    }

    public void UnregisterKeybind(Keybind toggleDebugOverlay) {
        // throw new NotImplementedException();
    }
}
