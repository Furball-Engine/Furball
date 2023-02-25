namespace Furball.Engine.Engine.Graphics.GML;

public enum SizeType {
    ExpandToFit,
    Fixed
}

public struct GMLSize {
    public GMLSize() {
        this.Type    = SizeType.ExpandToFit;
        this.Size    = 0;
        this.MaxSize = float.PositiveInfinity;
    }
    
    public GMLSize(float size) {
        this.Size = size;
        this.Type = SizeType.Fixed;
    }

    public SizeType Type;
    /// <summary>
    /// This is the Fixed size in fixed mode, and the min size in ExpandToFit mode
    /// </summary>
    public float Size;
    /// <summary>
    /// The max size in ExpandToFit mode, in fixed mode this does nothing
    /// </summary>
    public float MaxSize = float.PositiveInfinity;
}

public struct GMLCalculatedElementSize {
    public GMLSize Width;
    public GMLSize Height;
}
