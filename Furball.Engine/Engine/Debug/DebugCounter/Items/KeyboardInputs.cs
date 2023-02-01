using System.Collections.Generic;
using System.Text;
using Furball.Engine.Engine.Input;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

internal class KeyboardInputs : DebugCounterItem {
    public override bool ForceNewLine => true;

    public override string GetAsString(double time) {
        StringBuilder builder = new();

        builder.Append("keyboard: {");

        List<FurballKeyboard> keyboards = FurballGame.InputManager.Keyboards;

        for (int i = 0; i < keyboards.Count; i++) {
            FurballKeyboard kb = keyboards[i];
            builder.Append($"[{i}]({kb.Name}, ");

            builder.Append("{");
            for (int k = 0; k < kb.PressedKeys.Length; k++) {
                bool state = kb.PressedKeys[k];

                if (!state)
                    continue;
                
                Key key = (Key)k;

                builder.Append(key.ToString());
                
                builder.Append(", ");
            }
            builder.Append("})");
        }

        builder.Append("} ");

        builder.Append("shift: ");
        builder.Append(FurballGame.InputManager.ShiftHeld);

        builder.Append(" ctrl: ");
        builder.Append(FurballGame.InputManager.ControlHeld);
        
        return builder.ToString();
    }
}
