using Furball.Vixie;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

internal class TrackedVixieResources : DebugCounterItem {

    public override string GetAsString(double time) => $"bt: {Global.TrackedTextures.Count:N0} brt: {Global.TrackedRenderTargets.Count:N0}";
}