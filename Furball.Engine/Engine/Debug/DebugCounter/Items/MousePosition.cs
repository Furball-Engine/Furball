using Microsoft.Xna.Framework;

#if DESKTOP

namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    /// <summary>
    /// Displays the Desktop Mouse Position
    /// </summary>
    public class MousePosition : DebugCounterItem {
        public override string GetAsString(GameTime time) => $"mouse: {FurballGame.InputManager.CursorStates[0].Position.ToVector2().ToString()}";
    }
}

#endif
