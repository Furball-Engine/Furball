using Furball.Vixie.Backends.Veldrid;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    public class GraphicsBackend : DebugCounterItem {
        public override string GetAsString(double time) {
            if (Vixie.GraphicsBackend.Current is VeldridBackend veldridBackend) {
                return $"Backend: Veldrid ({veldridBackend.ChosenBackend})";
            }
            
            return $"Backend: {FurballGame.Instance.WindowManager.Backend}";
        }
    }
}
