using System;
using Furball.Engine.Engine.Input;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input.Events;

public class KeyEventArgs : EventArgs {
    public Key             Key;
    public int Keyboard;
        
    public KeyEventArgs(Key key, int keyboard) {
        this.Key      = key;
        this.Keyboard = keyboard;
    }
}
