using Furball.Vixie.Backends.OpenGL;
using Furball.Vixie.Backends.Shared.Backends;
using Furball.Vixie.Backends.Veldrid;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items; 

public class GraphicsBackend : DebugCounterItem {
    private string str;
        
    public GraphicsBackend() {
        if (Vixie.GraphicsBackend.Current is VeldridBackend veldridBackend) {
            str = $"Backend: Veldrid ({veldridBackend.ChosenBackend})";
            return;
        }

        if (Vixie.GraphicsBackend.Current is OpenGLBackend openGlBackend) {
            if(openGlBackend.CreationBackend == Backend.OpenGL)
                str = $"Backend: OpenGL ({Vixie.Backends.Shared.Global.LatestSupportedGL.GL.MajorVersion}.{Vixie.Backends.Shared.Global.LatestSupportedGL.GL.MinorVersion})";
            else
                str = $"Backend: OpenGLES ({Vixie.Backends.Shared.Global.LatestSupportedGL.GLES.MajorVersion}.{Vixie.Backends.Shared.Global.LatestSupportedGL.GLES.MinorVersion})";
            return;
        }

        str = $"Backend: {FurballGame.Instance.WindowManager.Backend}";
    }
        
    public override string GetAsString(double time) {
        return str;
    }
}