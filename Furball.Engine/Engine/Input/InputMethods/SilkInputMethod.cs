using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Channels;
using Furball.Engine.Engine.Input.Events;
using Furball.Vixie.WindowManagement;
using Silk.NET.Input;
using OneOf;

namespace Furball.Engine.Engine.Input.InputMethods;

public class SilkInputMethod : InputMethod {
    /// <summary>
    /// Used to cached Enum.IsDefined calls
    /// </summary>
    private static readonly bool[] DefinedCache = new bool[(int)(Key.Menu + 1)];

    private readonly Channel<OneOf<MouseScrollUpdate, SilkKeyChar>> _channelToInput = Channel.CreateUnbounded<OneOf<MouseScrollUpdate, SilkKeyChar>>();

    private InputManager                                         _inputManager;
    private ChannelReader<OneOf<MouseScrollUpdate, SilkKeyChar>> _reader;

    static SilkInputMethod() {
        for (int i = 0; i < DefinedCache.Length; i++) {
            DefinedCache[i] = Enum.IsDefined(typeof(Key), i);
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

    public SilkInputMethod(bool disableKeyboards) {
        this._disableKeyboards = disableKeyboards;
    }

    readonly bool[] _workingMouseButtons = new bool[(int)(MouseButton.Button12 + 1)];
    readonly bool[] _workingKeyboardKeys  = new bool[(int)(Key.Menu + 1)];
    public override void Update() {
        while (this._reader.TryRead(out var item)) {
            item.Switch(
            update => {
                for (int i = 0; i < this._mice.Count; i++) {
                    IMouse       mouse  = this._mice[i].sMouse;
                    FurballMouse fMouse = this._mice[i].fMouse;
                    if (mouse == update.Mouse) {
                        fMouse.ScrollWheel.X += update.X;
                        fMouse.ScrollWheel.Y  += update.Y;
        
                        this._inputManager.InvokeOnMouseScroll(new MouseScrollEventArgs(new Vector2(update.X, update.Y), fMouse));
        
                        break;
                    }
                }
            },
            silkCharEvent => {
                for (int i = 0; i < this._keyboards.Count; i++) {
                    IKeyboard       silkKeyboard = this._keyboards[i].sKeyboard;
                    FurballKeyboard fKeyboard    = this._keyboards[i].fKeyboard;
        
                    if (silkKeyboard != silkCharEvent.Keyboard)
                        continue;
        
                    CharInputEvent ev = new CharInputEvent(silkCharEvent.Character, fKeyboard);
        
                    this._inputManager.InvokeOnCharInput(ev);
                    this._inputManager.CharInputHandler?.HandleChar(ev);
                }
            }
            );
        } 
        
        for (int i = 0; i < this._mice.Count; i++) {
            (FurballMouse fMouse, IMouse sMouse) = this._mice[i];

            this.SilkMouseButtonCheck(this._workingMouseButtons, sMouse, fMouse);
    
            Vector2 newPosition = sMouse.Position / FurballGame.VerticalRatio;
            this.SilkMousePositionCheck(newPosition, fMouse);
    
            fMouse.Position = newPosition;
        }
        
        for (int i = 0; i < this._keyboards.Count; i++) {
            (FurballKeyboard fKeyboard, IKeyboard sKeyboard) = this._keyboards[i];
 
            this.SilkKeyboardButtonCheck(this._workingKeyboardKeys, sKeyboard, fKeyboard);
        }
    }

    public override void Dispose() {
        foreach ((FurballMouse fMouse, IMouse sMouse) in this._mice) {
            sMouse.Scroll -= this.HandleSilkMouseScroll;
            this.OnMouseRemoved(fMouse);
        }
        foreach ((FurballKeyboard fKeyboard, IKeyboard sKeyboard) in this._keyboards) {
            sKeyboard.KeyChar -= HandleSilkKeyChar;
            this.OnKeyboardRemoved(fKeyboard);
        }

        this._channelToInput.Writer.Complete();
    }

    private readonly List<(FurballMouse fMouse, IMouse sMouse)>             _mice      = new List<(FurballMouse fMouse, IMouse sMouse)>();
    private readonly List<(FurballKeyboard fKeyboard, IKeyboard sKeyboard)> _keyboards = new List<(FurballKeyboard fKeyboard, IKeyboard sKeyboard)>();
    private          ChannelWriter<OneOf<MouseScrollUpdate, SilkKeyChar>>   _writer;
    private readonly bool                                                   _disableKeyboards;

    public override void Initialize(InputManager inputManager) {
        this._inputManager = inputManager;

        this._reader = this._channelToInput.Reader;
        this._writer = this._channelToInput.Writer;
        
        if (FurballGame.Instance.WindowManager is SilkWindowManager silkWindowManager) {
            IReadOnlyList<IMouse> mice = silkWindowManager.InputContext.Mice;

            foreach (IMouse mouse in mice) {
                mouse.Scroll += this.HandleSilkMouseScroll;
                FurballMouse fMouse = new FurballMouse {
                    Name = mouse.Name
                };
                this._mice.Add((fMouse, mouse));
                this.OnMouseAdded(fMouse);
            }

            IReadOnlyList<IKeyboard> keyboards = silkWindowManager.InputContext.Keyboards;

            if (this._disableKeyboards)
                return;
            
            foreach (IKeyboard keyboard in keyboards) {
                keyboard.KeyChar += HandleSilkKeyChar;
                FurballKeyboard fKeyboard = new FurballKeyboard {
                    Name = keyboard.Name,
                    GetClipboard = () => keyboard.ClipboardText,
                    SetClipboard = s => keyboard.ClipboardText = s,
                    BeginInput   = () => keyboard.BeginInput(),
                    EndInput     = () => keyboard.EndInput()
                };
                this._keyboards.Add((fKeyboard, keyboard));
                this.OnKeyboardAdded(fKeyboard);
            }
        }
    }
    
    private void HandleSilkMouseScroll(IMouse arg1, Silk.NET.Input.ScrollWheel arg2) {
        if (arg2.X != 0)
            this._writer.WriteAsync(
            new MouseScrollUpdate {
                X     = arg2.X,
                Mouse = arg1
            }
            );
        if (arg2.Y != 0)
            this._writer.WriteAsync(
            new MouseScrollUpdate {
                Y     = arg2.Y,
                Mouse = arg1
            }
            );
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
                    this._inputManager.InvokeOnKeyDown(new KeyEventArgs(key, keyboard));

                    foreach (Keybind bind in this._inputManager.RegisteredKeybinds) {
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
                            this._inputManager.ControlCount++;
                            break;
                        case Key.ShiftLeft or Key.ShiftRight:
                            this._inputManager.ShiftCount++;
                            break;
                        case Key.AltLeft or Key.AltRight:
                            this._inputManager.AltCount++;
                            break;
                    }

                }
            } else {
                //Set the working key to false
                workingKeyboardKeys[j] = false;

                //If the key was pressed last frame
                if (cur) {
                    this._inputManager.InvokeOnKeyUp(new KeyEventArgs(key, keyboard));

                    switch (key) {
                        case Key.ControlLeft or Key.ControlRight:
                            this._inputManager.ControlCount--;
                            break;
                        case Key.ShiftLeft or Key.ShiftRight:
                            this._inputManager.ShiftCount--;
                            break;
                        case Key.AltLeft or Key.AltRight:
                            this._inputManager.AltCount--;
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
            //Reset the JustPressed state of the button
            mouse.JustPressed[j] = false;
            if (silkMouse.IsButtonPressed((MouseButton)j)) {
                newButtons[j] = true;

                if (!mouse.PressedButtons[j]) {
                    //Mark that the button was just pressed
                    mouse.JustPressed[j] = true;
                    this._inputManager.InvokeOnMouseDown(new MouseButtonEventArgs((MouseButton)j, mouse));
                }
            } else {
                newButtons[j] = false;

                if (mouse.PressedButtons[j]) {
                    this._inputManager.InvokeOnMouseUp(new MouseButtonEventArgs((MouseButton)j, mouse));
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
                        this._inputManager.InvokeOnMouseDrag(new MouseDragEventArgs(dragState.StartPosition, mouse.Position, newPosition, (MouseButton)i, mouse));
                    } else {
                        //Else, start a new drag
                        dragState.Active        = true;
                        dragState.StartPosition = newPosition;

                        this._inputManager.InvokeOnMouseDragStart(new MouseDragEventArgs(dragState.StartPosition, mouse.Position, newPosition, (MouseButton)i, mouse));
                    }
                } else {
                    //Else, if the drag is active
                    if (dragState.Active) {
                        //End the drag
                        dragState.Active = false;

                        this._inputManager.InvokeOnMouseDragEnd(new MouseDragEventArgs(dragState.StartPosition, mouse.Position, newPosition, (MouseButton)i, mouse));
                    }
                }
            }

            this._inputManager.InvokeOnMouseMove(new MouseMoveEventArgs(newPosition, mouse));
        }
    }
}
