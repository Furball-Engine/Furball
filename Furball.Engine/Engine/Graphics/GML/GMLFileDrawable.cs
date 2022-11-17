using System;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables;
using GMLSharp;

namespace Furball.Engine.Engine.Graphics.GML;

public class GMLFileDrawable : Drawable, IGMLElement {
    private Vector2 _wantedSize;

    public GMLFile File {
        get; 
        private set;
    }

    public GMLFileDrawable(Vector2 position, Vector2 wantedSize) {
        this.Position    = position;
        this._wantedSize = wantedSize;
    }

    public void SetGMLFile(GMLFile file) {
        this.File = file;
        this.Invalidate();
    }
    
    public void Invalidate() {
        throw new NotImplementedException();
    }
}
