using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    public class GameTimeSourceTime : DebugCounterItem {
        public override string GetAsString(GameTime time) => $"gt: {FurballGame.GameTimeSource.GetCurrentTime()}";
    }
}
