using System;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input.Events;

public class KeyEventArgs : EventArgs {
    public Key             Key;
    public FurballKeyboard Keyboard;
        
    public KeyEventArgs(Key key, FurballKeyboard keyboard) {
        this.Key      = key;
        this.Keyboard = keyboard;
    }
}
