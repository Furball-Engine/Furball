using System.Collections.Generic;
using System.Text;
using Furball.Engine.Engine.Input;

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

        builder.Append("}");
        return builder.ToString();
    }
}
