using System.Text;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

internal class KeyboardInputs : DebugCounterItem {
    public override bool ForceNewLine => true;

    public override string GetAsString(double time) {
        StringBuilder builder = new();

        builder.Append("keyboard: {");

        //TODO
        // List<FurballKeyboard> keyboards = FurballGame.InputManager.Keyboards;
        // for (int i = 0; i < keyboards.Count; i++) {
        //     FurballKeyboard keyboard = keyboards[i];
        //     builder.Append($"[{i}]({keyboard.Name}, {string.Join(",", keyboard.PressedKeys)})");
        //     if (i != keyboards.Count - 1) {
        //         builder.Append('\n');
        //     }
        // }

        builder.Append("} ");

        builder.Append("shift: ");
        builder.Append(FurballGame.InputManager.ShiftHeld);

        builder.Append(" ctrl: ");
        builder.Append(FurballGame.InputManager.ControlHeld);
        
        return builder.ToString();
    }
}
