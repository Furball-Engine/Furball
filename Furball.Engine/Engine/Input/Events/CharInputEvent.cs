using System;

namespace Furball.Engine.Engine.Input.Events; 

public class CharInputEvent : EventArgs {
    public char            Char;
    public FurballKeyboard Keyboard;

    public CharInputEvent(char c, FurballKeyboard keyboard) {
        this.Char     = c;
        this.Keyboard = keyboard;
    }
}
