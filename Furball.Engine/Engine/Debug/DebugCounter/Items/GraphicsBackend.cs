using Furball.Vixie.Backends.OpenGL20;
using Furball.Vixie.Backends.OpenGL41;
using Furball.Vixie.Backends.OpenGLES;
using Furball.Vixie.Backends.Veldrid;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    public class GraphicsBackend : DebugCounterItem {
        private string str;
        
        public GraphicsBackend() {
            if (Vixie.GraphicsBackend.Current is VeldridBackend veldridBackend) {
                str = $"Backend: Veldrid ({veldridBackend.ChosenBackend})";
                return;
            }

            if (Vixie.GraphicsBackend.Current is OpenGL41Backend openGl41Backend) {
                str = $"Backend: ModernOpenGL ({Vixie.Backends.Shared.Global.LatestSupportedGL.GL.MajorVersion}.{Vixie.Backends.Shared.Global.LatestSupportedGL.GL.MinorVersion})";
                return;
            }
            
            if (Vixie.GraphicsBackend.Current is OpenGL20Backend openGl20Backend) {
                str = $"Backend: LegacyOpenGL ({Vixie.Backends.Shared.Global.LatestSupportedGL.GL.MajorVersion}.{Vixie.Backends.Shared.Global.LatestSupportedGL.GL.MinorVersion})";
                return;
            }
            
            if (Vixie.GraphicsBackend.Current is OpenGLESBackend openGlesBackend) {
                str = $"Backend: OpenGLES ({Vixie.Backends.Shared.Global.LatestSupportedGL.GLES.MajorVersion}.{Vixie.Backends.Shared.Global.LatestSupportedGL.GLES.MinorVersion})";
                return;
            }

            str = $"Backend: {FurballGame.Instance.WindowManager.Backend}";
        }
        
        public override string GetAsString(double time) {
            return str;
        }
    }
}
