using System;
using System.Globalization;


namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    /// <summary>
    /// Displays whether the Draw method or the Update Method is currently running slower
    /// </summary>
    public class BoundByDrawUpdate : DebugCounterItem {
        public override string GetAsString(double time) =>
            $"bound: {(FurballGame.Instance.LastDrawTime > FurballGame.Instance.LastUpdateTime ? "draw" : "update")} " +
            $"(d: {Math.Round(FurballGame.Instance.LastDrawTime, 2).ToString(CultureInfo.InvariantCulture)}ms; u: {Math.Round(FurballGame.Instance.LastUpdateTime, 2).ToString(CultureInfo.InvariantCulture)}ms";
    }
}
