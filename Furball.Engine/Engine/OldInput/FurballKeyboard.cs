using System;
using System.Collections.Generic;
using Silk.NET.Input;

namespace Furball.Engine.Engine.OldInput; 

public class FurballKeyboard {
    public string Name;
    
    public readonly List<Key> PressedKeys = new();

    internal readonly List<Key> QueuedKeyPresses  = new();
    internal readonly List<Key> QueuedKeyReleases = new();

    internal List<char> QueuedTextInputs = new();
    
    public bool IsKeyPressed(Key k) => this.PressedKeys.Contains(k); 
    
    public delegate string ClipboardGetter();
    
    public ClipboardGetter GetClipboard;
    public Action<string> SetClipboard;

    public Action BeginInput;
    public Action EndInput;
}
