

using System.Text;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items; 

public class KeyboardInputs : DebugCounterItem {
    public override bool ForceNewLine { get; } = true;

    public override string GetAsString(double time) {
        StringBuilder builder = new(FurballGame.InputManager.HeldKeys.Count + 2);

        builder.Append("keyboard: {");

        for (int i = 0; i != FurballGame.InputManager.HeldKeys.Count; i++) {
            builder.Append($"{FurballGame.InputManager.HeldKeys[i].ToString()}, ");
        }

        builder.Append("}");
        return builder.ToString();
    }
}