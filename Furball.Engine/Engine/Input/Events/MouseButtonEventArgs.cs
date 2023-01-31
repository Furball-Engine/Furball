using System;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input.Events; 

public class MouseButtonEventArgs : EventArgs {
    public MouseButton  Button;
    public int MouseId;
    
    public MouseButtonEventArgs(MouseButton button, int mouse) {
        this.Button  = button;
        this.MouseId = mouse;
    }
}
