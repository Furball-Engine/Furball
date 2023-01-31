using System;
using System.Numerics;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input.Events; 

public class MouseDragEventArgs : EventArgs {
    public Vector2     StartPosition;
    public Vector2     LastPosition;
    public Vector2     Position;
    public MouseButton Button;
    public int         MouseId;

    public MouseDragEventArgs(Vector2 startPosition, Vector2 lastPosition, Vector2 position, MouseButton button, int mouse) {
        this.StartPosition = startPosition;
        this.Position      = position;
        this.Button        = button;
        this.MouseId       = mouse;
        this.LastPosition  = lastPosition;
    }
}
