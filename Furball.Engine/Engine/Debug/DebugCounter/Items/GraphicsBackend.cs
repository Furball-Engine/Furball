using Furball.Vixie.Backends.OpenGL;
using Furball.Vixie.Backends.Shared;
using Furball.Vixie.Backends.Shared.Backends;

namespace Furball.Engine.Engine.Debug.DebugCounter.Items;

internal class GraphicsBackend : DebugCounterItem {
    private readonly string _stringCache;
        
    //TODO: handle backend changing
    public GraphicsBackend() {
        switch (FurballGame.Instance.WindowManager.GraphicsBackend) {
            case OpenGLBackend openGlBackend: {
                this._stringCache = openGlBackend.CreationBackend == Backend.OpenGL 
                                        ? $"Backend: OpenGL ({Global.LatestSupportedGl.Value.GL.MajorVersion}.{Global.LatestSupportedGl.Value.GL.MinorVersion})" 
                                        : $"Backend: OpenGLES ({Global.LatestSupportedGl.Value.GLES.MajorVersion}.{Global.LatestSupportedGl.Value.GLES.MinorVersion})";
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