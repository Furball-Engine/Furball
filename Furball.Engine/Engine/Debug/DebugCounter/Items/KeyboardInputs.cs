

using System.Text;
using Furball.Engine.Engine.Input;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items; 

public class KeyboardInputs : DebugCounterItem {
    public override bool ForceNewLine {
        get => true;
    }

    public override string GetAsString(double time) {
        StringBuilder builder = new(FurballGame.InputManager.HeldKeys.Count + 2);

        builder.Append("keyboard: {");

        for (int i = 0; i < FurballGame.InputManager.Keyboards.Count; i++) {
            FurballKeyboard keyboard = FurballGame.InputManager.Keyboards[i];
            builder.AppendLine($"[{i}]({keyboard.Name}, {string.Join(",", keyboard.PressedKeys)})");
        }

        builder.Append("}");
        return builder.ToString();
    }
}