using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Furball.Engine.Engine.Helpers.Logger;
using Furball.Engine.Engine.Input.Events;
using Furball.Engine.Engine.Timing;
using JetBrains.Annotations;
using Kettu;

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
        //TODO: process events from the channel and invoke them on the main thread
    }
    
    private void Run() {
        //currently at 100ms (10 per second) for testing
        using HighResolutionClock clock = new HighResolutionClock(TimeSpan.FromMilliseconds(100));
        
        Stopwatch stopwatch = Stopwatch.StartNew();

        double start = stopwatch.Elapsed.TotalMilliseconds;
        while (this._run) {
            Console.WriteLine($"Input frame clock run");
            
            //Wait the clock 
            if (this._run) {
                clock.WaitFrame();
                double elapsed = stopwatch.Elapsed.TotalMilliseconds - start;
                Console.WriteLine($"Input frame delta {elapsed:N2}ms:{1000d / elapsed:N2} per second");
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
