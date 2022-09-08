namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

public class VramUsage : DebugCounterItem {

    public override string GetAsString(double time)
        => $"vram:{Vixie.GraphicsBackend.Current.GetVramUsage() / 1024f / 1024f:N2}mb/{Vixie.GraphicsBackend.Current.GetTotalVram() / 1024f / 1024f:N2}mb";
}
