
using Microsoft.Xna.Framework.Input;

namespace Furball.Engine.Engine.Input {
    public class MouseState {
        public Point       Position;
        public string      Name;
        public ButtonState LeftButton;
        public ButtonState MiddleButton;
        public ButtonState RightButton;
        public ButtonState XButton1;
        public ButtonState XButton2;
        public int         ScrollWheelValue;
        public int         HorizontalScrollWheelValue;

        public MouseState(string name) {
            this.Name  = name;
        }
    }
}
