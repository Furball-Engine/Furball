using Furball.Vixie;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

public class VramUsage : DebugCounterItem {

    public override string GetAsString(double time)
        => $"vram:{FurballGame.Instance.WindowManager.GraphicsBackend.GetVramUsage() / 1024f / 1024f:N2}mb/{FurballGame.Instance.WindowManager.GraphicsBackend.GetTotalVram() / 1024f / 1024f:N2}mb";
}
