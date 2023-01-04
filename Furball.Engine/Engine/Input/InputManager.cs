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
        ChannelReader<OneOf<MouseMoveEvent>> reader = this._channel.Reader;

        while (reader.TryRead(out OneOf<MouseMoveEvent> item)) {
            switch (item.Value) {
                case MouseMoveEvent mouseMoveEvent: {
                    Console.WriteLine($"Mouse move event: {mouseMoveEvent.Position}");
                    this.OnMouseMove?.Invoke(this, new MouseMoveEventArgs(mouseMoveEvent.Position, null));//TODO: mouse
                    break;
                }
            }
        }
    }

    private struct MouseMoveEvent {
        public int     MouseId;
        public Vector2 Position;
    }

    private Channel<OneOf<MouseMoveEvent>> _channel = Channel.CreateUnbounded<OneOf<MouseMoveEvent>>();

    private void Run() {
        using HighResolutionClock clock = new HighResolutionClock(TimeSpan.FromMilliseconds(10));

        Stopwatch stopwatch = Stopwatch.StartNew();

        ChannelWriter<OneOf<MouseMoveEvent>> writer = this._channel.Writer;

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

        double start = stopwatch.Elapsed.TotalMilliseconds;
        while (this._run) {
            // Console.WriteLine($"Input frame clock run");

            for (int i = 0; i < mice.Count; i++) {
                FurballMouse mouse = mice[i];
                
                if(i < silkMice.Count) {
                    IMouse silkMouse = silkMice[i];

                    Vector2 newPosition = silkMouse.Position;

                    if (newPosition != mouse.Position) {
                        writer.WriteAsync(new MouseMoveEvent {
                            Position = newPosition
                        });
                    }
                    
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

    public void RegisterKeybind(Keybind toggleDebugOverlay) {
        // throw new NotImplementedException();
    }

    public void UnregisterKeybind(Keybind toggleDebugOverlay) {
        // throw new NotImplementedException();
    }
}
