using Furball.Vixie.Backends.OpenGL;
using Furball.Vixie.Backends.Shared.Backends;
using Furball.Vixie.Backends.Veldrid;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items; 

public class GraphicsBackend : DebugCounterItem {
    private readonly string _stringCache;
        
    //TODO: handle backend changing
    public GraphicsBackend() {
        if (Vixie.GraphicsBackend.Current is VeldridBackend veldridBackend) {
            this._stringCache = $"Backend: Veldrid ({veldridBackend.ChosenBackend})";
            return;
        }

        if (Vixie.GraphicsBackend.Current is OpenGLBackend openGlBackend) {
            if(openGlBackend.CreationBackend == Backend.OpenGL)
                this._stringCache = $"Backend: OpenGL ({Vixie.Backends.Shared.Global.LatestSupportedGl.GL.MajorVersion}.{Vixie.Backends.Shared.Global.LatestSupportedGl.GL.MinorVersion})";
            else
                this._stringCache = $"Backend: OpenGLES ({Vixie.Backends.Shared.Global.LatestSupportedGl.GLES.MajorVersion}.{Vixie.Backends.Shared.Global.LatestSupportedGl.GLES.MinorVersion})";
            return;
        }

        this._stringCache = $"Backend: {FurballGame.Instance.WindowManager.Backend}";
    }
        
    public override string GetAsString(double time) {
        return this._stringCache;
    }
}