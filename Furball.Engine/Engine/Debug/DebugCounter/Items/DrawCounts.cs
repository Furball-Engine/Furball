namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

public class DrawCounts : DebugCounterItem {
    public static ulong LastVertexCount;
    public static ulong LastIndexCount;

    public override string GetAsString(double time) => $"vtx: {LastVertexCount} idx: {LastIndexCount}";
}
