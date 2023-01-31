namespace Furball.Engine.Engine.Graphics.GML;

public interface IGMLElement {
    public GMLCalculatedElementSize MinimumSize();

    public bool FillWithBackgroundColor();
    
    public void Invalidate();
}
