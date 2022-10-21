namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

internal class DrawCounts : DebugCounterItem {
    public static ulong LastVertexCount;
    public static ulong LastIndexCount;

    public override string GetAsString(double time) => $"vtx: {LastVertexCount} idx: {LastIndexCount} tris: {LastIndexCount / 3}";
}
