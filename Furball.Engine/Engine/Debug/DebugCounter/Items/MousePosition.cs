using System.Text;
using Furball.Engine.Engine.Input;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

/// <summary>
/// Displays the Desktop Mouse Position
/// </summary>
internal class MousePosition : DebugCounterItem {
    public override string GetAsString(double time) {
        StringBuilder builder = new("mouse: ");

        for (int i = 0; i < FurballGame.InputManager.Mice.Count; i++) {
            FurballMouse cursor = FurballGame.InputManager.Mice[i];
            builder.Append($"[{i}]({cursor.Position.X}x{cursor.Position.Y}, {cursor.Name}, (");
            builder.Append($"{string.Join(",", cursor.PressedButtons)}");
            builder.Append($"), {cursor.ScrollWheel.X}x{cursor.ScrollWheel.Y}) ");
        }

        return builder.ToString().Trim();
    }
}
