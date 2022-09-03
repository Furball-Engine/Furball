using Furball.Vixie;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items; 

public class TrackedVixieResources : DebugCounterItem {

    public override string GetAsString(double time) => $"bt: {Global.TrackedTextures.Count:N0} brt: {Global.TrackedRenderTargets.Count:N0}";
}