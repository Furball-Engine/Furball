using System.Numerics;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Input; 

public struct FurballMouseState {
    public Vector2       Position;
    public string        Name;
    public MouseButton[] PressedButtons;
    public ScrollWheel   ScrollWheel;

    public bool IsButtonPressed(MouseButton button) {
        for (int i = 0; i < this.PressedButtons.Length; i++) {
            MouseButton pressedButton = this.PressedButtons[i];

            if (pressedButton == button)
                return true;
        }

        return false;
    }
}