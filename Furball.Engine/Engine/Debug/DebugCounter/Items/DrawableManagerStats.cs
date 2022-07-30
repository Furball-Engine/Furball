using Furball.Engine.Engine.Graphics.Drawables.Managers;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

/// <summary>
///     Displays some basic DrawableManager stats, like the amount of UnmanagedDrawables total compares to ManagedDrawables
///     total
///     aswell as how many DrawableManager instances exist
/// </summary>
public class DrawableManagerStats : DebugCounterItem {
    private int    _lastUpdatedManagedDrawables;
    private int    _lastUpdatedUnmanagedDrawables;
    private int    _lastUpdatedInstanceCount;
    private double _deltaTime;

    public override void Update(double time) {
        this._deltaTime += time;

        if (this._deltaTime >= 1.0) {
            this._lastUpdatedManagedDrawables   = 0;
            this._lastUpdatedUnmanagedDrawables = 0;

            int drawableManagerCount = DrawableManager.DrawableManagers.Count;

            this._lastUpdatedInstanceCount = drawableManagerCount;

            for (int i = 0; i != drawableManagerCount; i++) {
                DrawableManager current = DrawableManager.DrawableManagers[i];

                this._lastUpdatedManagedDrawables += current.CountManaged;
            }

            this._deltaTime = 0.0;
        }

        base.Update(time);
    }

    public override string GetAsString(double time) => $"dmi: {this._lastUpdatedInstanceCount}; md: {this._lastUpdatedManagedDrawables}";
}
