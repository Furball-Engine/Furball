using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Furball.Engine.Engine.Helpers;
using EvDevSharp;
using EvDevSharp.Enums;
using EvDevSharp.EventArgs;
using Furball.Engine.Engine.Input.Events;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input.InputMethods; 

public class EvDevInputMethod : InputMethod {
    private readonly List<(FurballKeyboard fKeyboard, EvDevDevice eKeyboard)> _trackedDevices  = new List<(FurballKeyboard fKeyboard, EvDevDevice eKeyboard)>();
    private readonly ConcurrentQueue<(int i, OnKeyEventArgs args)>            _queuedKeyEvents = new ConcurrentQueue<(int i, OnKeyEventArgs args)>();
    private          InputManager                                             _inputManager;

    public override void Update() {
        while (this._queuedKeyEvents.TryDequeue(out (int i, OnKeyEventArgs args) item)) {
            Key             k  = item.args.Key.ToSilk();
            FurballKeyboard keyboard = this._trackedDevices[item.i].fKeyboard;
            switch (item.args.Type) {
                case EvDevKeyValue.KeyDown:
                    if (!FurballGame.Instance.WindowManager.Focused)
                        break;

                    keyboard.PressedKeys[(int)k] = true;
                    this._inputManager.InvokeOnKeyDown(new KeyEventArgs(k, keyboard));
                    
                    foreach (Keybind bind in this._inputManager.RegisteredKeybinds) {
                        // ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (Key bindModifier in bind.Modifiers) {
                            //If one of the modifiers isn't pressed, return out
                            if (!keyboard.IsKeyPressed(bindModifier))
                                return;
                        }

                        if (bind.Key == k) {
                            bind.OnPressed?.Invoke(new KeyEventArgs(k, keyboard));
                        }
                    }
                
                    //Warning: hacky shit ahead
                
                    if (k == Key.Space) {
                        CharInputEvent ev = new CharInputEvent(' ', keyboard);
        
                        this._inputManager.InvokeOnCharInput(ev);
                        this._inputManager.CharInputHandler?.HandleChar(ev);
                        
                        break;
                    }
                    
                    string kStr  = k.ToString();
                    char   kChar = char.ToLower(Convert.ToChar(kStr.Substring(0, 1)));
                    if (kStr.Length == 1 && char.IsLetter(kChar)) {
                        if(keyboard.IsKeyPressed(Key.ShiftRight) || keyboard.IsKeyPressed(Key.ShiftLeft))
                            kChar = char.ToUpper(kChar);
                        
                        CharInputEvent ev = new CharInputEvent(kChar, keyboard);
        
                        this._inputManager.InvokeOnCharInput(ev);
                        this._inputManager.CharInputHandler?.HandleChar(ev);
                
                        break;
                    }
                
                    if (kStr.Length == "NumberX".Length && kStr.StartsWith("Number")) {
                        CharInputEvent ev = new CharInputEvent(Convert.ToChar(kStr.TrimStart("Number".ToCharArray()).Substring(0, 1)), keyboard);
        
                        this._inputManager.InvokeOnCharInput(ev);
                        this._inputManager.CharInputHandler?.HandleChar(ev);
                    }
                    
                    break;
                case EvDevKeyValue.KeyUp:
                    keyboard.PressedKeys[(int)k] = false;
                    break;
            }
        }
    }
    
    public override void Dispose() {
        foreach ((FurballKeyboard fKeyboard, EvDevDevice eKeyboard) in this._trackedDevices) {
            this.OnKeyboardRemoved(fKeyboard);
            eKeyboard.StopMonitoring();
        }
    }
    
    public override void Initialize(InputManager inputManager) {
        this._inputManager = inputManager;
        
        List<EvDevDevice> devices = EvDevDevice.GetDevices().Where(device => device.GuessedDeviceType == EvDevGuessedDeviceType.Keyboard).ToList();

        for (int i = 0; i < devices.Count; i++) {
            EvDevDevice device = devices[i];
            
            FurballKeyboard keyboard = new() {
                Name         = device.Name,
                BeginInput   = () => {},
                EndInput     = () => {},
                GetClipboard = () => "",
                SetClipboard = _ => {}
            };

            this.OnKeyboardAdded(keyboard);
            
            device.StartMonitoring();
            this._trackedDevices.Add((keyboard, device));

            int j = i;
            device.OnKeyEvent += (_, args) => this.OnDeviceKeyEvent(j, args);
        }
    }
    
    private void OnDeviceKeyEvent(int i, OnKeyEventArgs args) {
        this._queuedKeyEvents.Enqueue((i, args));
    }
}
