namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

/// <summary>
/// Displays information about the update loops
/// </summary>
internal class UpdateLoops : DebugCounterItem {
    public override string GetAsString(double time) => $"bound: {(FurballGame.Instance.LastDrawTime > FurballGame.Instance.LastUpdateTime ? "draw" : "update")} " +
                                                       $"(d: {FurballGame.Instance.LastDrawTime:N4}ms; u: {FurballGame.Instance.LastUpdateTime:N4}ms; i: {FurballGame.InputManager.LastInputFrameTime:N4}ms)";
}
