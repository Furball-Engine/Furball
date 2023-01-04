using System;
using System.Numerics;
using Furball.Engine.Engine.Input;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input.Events; 

public class MouseDragEventArgs : EventArgs {
    public Vector2      StartPosition;
    public Vector2      LastPosition;
    public Vector2      Position;
    public MouseButton  Button;
    public FurballMouse Mouse;

    public MouseDragEventArgs(Vector2 startPosition, Vector2 lastPosition, Vector2 position, MouseButton button, FurballMouse mouse) {
        this.StartPosition = startPosition;
        this.Position      = position;
        this.Button        = button;
        this.Mouse         = mouse;
        this.LastPosition  = lastPosition;
    }
}
