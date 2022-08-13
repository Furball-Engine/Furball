namespace Furball.Engine.Engine.Debug.DebugCounter.Items; 

/// <summary>
/// Displays whether the Draw method or the Update Method is currently running slower
/// </summary>
public class BoundByDrawUpdate : DebugCounterItem {
    public override string GetAsString(double time) =>
        $"bound: {(FurballGame.Instance.LastDrawTime > FurballGame.Instance.LastUpdateTime ? "draw" : "update")} " +
        $"(d: {FurballGame.Instance.LastDrawTime:N2}ms; u: {FurballGame.Instance.LastUpdateTime:N2}ms";
}