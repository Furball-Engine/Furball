

#if DESKTOP

namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    /// <summary>
    /// Displays the Desktop Mouse Position
    /// </summary>
    public class MousePosition : DebugCounterItem {
        //public override string GetAsString(double time) => $"mouse: {FurballGame.InputManager.CursorStates[0].Position.ToString()}";
        public override string GetAsString(double time) => $"";
    }
}

#endif
