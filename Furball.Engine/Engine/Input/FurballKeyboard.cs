using System;
using System.Runtime.CompilerServices;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input; 

public class FurballKeyboard {
    public string Name;

    public bool ControlHeld => this.IsKeyPressed(Key.ControlLeft) || this.IsKeyPressed(Key.ControlRight);
    
    /// <summary>
    /// An array of the currently pressed keys
    /// </summary>
    public readonly bool[] PressedKeys = new bool[(int)(Key.Menu + 1)];

    //NOTE: (AggressiveOptimization is not available on NS20, 768 == AggressiveOptimization | AggressiveInlining)
    [MethodImpl((MethodImplOptions)768)]
    public bool IsKeyPressed(Key k) => this.PressedKeys[(int)k]; 
    
    public delegate string ClipboardGetter();
    
    public ClipboardGetter GetClipboard;
    public Action<string> SetClipboard;

    public Action BeginInput;
    public Action EndInput;
}
