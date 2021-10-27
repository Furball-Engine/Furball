using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    /// <summary>
    /// Displays the Current Game Time as provided by FurballGame.GameTimeSource
    /// </summary>
    public class GameTimeSourceTime : DebugCounterItem {
        public override string GetAsString(GameTime time) => $"gt: {FurballGame.GameTimeSource.GetCurrentTime()}";
    }
}
