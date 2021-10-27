using System;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    public class BoundByDrawUpdate : DebugCounterItem {
        public override string GetAsString(GameTime time) => $"bound: {(FurballGame.Instance.LastDrawTime > FurballGame.Instance.LastUpdateTime ? "draw" : "update")} " +
                                                             $"(d: {Math.Round(FurballGame.Instance.LastDrawTime, 2).ToString(CultureInfo.InvariantCulture)}ms; u: {Math.Round(FurballGame.Instance.LastUpdateTime, 2).ToString(CultureInfo.InvariantCulture)}ms";
    }
}
