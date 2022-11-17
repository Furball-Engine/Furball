using System.Numerics;

namespace Furball.Engine.Engine.Graphics.GML; 

public interface IGMLElement {
    public Vector2 ElementSize();

    public bool FillWithBackgroundColor();
    
    public void Invalidate();
}
