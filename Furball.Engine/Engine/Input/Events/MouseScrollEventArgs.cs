using System;
using System.Numerics;
using Furball.Engine.Engine.Input;

namespace Furball.Engine.Engine.Input.Events; 

public class MouseScrollEventArgs : EventArgs {
    public Vector2 ScrollAmount;
    public int     MouseId;
    
    public MouseScrollEventArgs(Vector2 scrollAmount, int mouse) {
        this.ScrollAmount = scrollAmount;
        this.MouseId = mouse;
    }
}
