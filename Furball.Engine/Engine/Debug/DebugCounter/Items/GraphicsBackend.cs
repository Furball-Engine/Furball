using Furball.Vixie.Backends.OpenGL;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Backends.Shared.Backends;
using Furball.Vixie.Backends.Veldrid;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items; 

public class GraphicsBackend : DebugCounterItem {
    private readonly string _stringCache;
        
    //TODO: handle backend changing
    public GraphicsBackend() {
        switch (FurballGame.Instance.WindowManager.GraphicsBackend) {
            case VeldridBackend veldridBackend:
                this._stringCache = $"Backend: Veldrid ({veldridBackend.ChosenBackend})";
                return;
            case OpenGLBackend openGlBackend: {
                this._stringCache = openGlBackend.CreationBackend == Backend.OpenGL 
                                        ? $"Backend: OpenGL ({Global.LatestSupportedGl.GL.MajorVersion}.{Global.LatestSupportedGl.GL.MinorVersion})" 
                                        : $"Backend: OpenGLES ({Global.LatestSupportedGl.GLES.MajorVersion}.{Global.LatestSupportedGl.GLES.MinorVersion})";
                return;
            }
            default:
                this._stringCache = $"Backend: {FurballGame.Instance.WindowManager.Backend}";
                break;
        }

    }
        
    public override string GetAsString(double time) {
        return this._stringCache;
    }
}