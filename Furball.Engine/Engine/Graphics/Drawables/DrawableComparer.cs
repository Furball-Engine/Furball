using System.Collections.Generic;
using Furball.Engine.Engine.Input;

namespace Furball.Engine.Engine.Graphics.Drawables;

public class DrawableDrawComparer : IComparer<Drawable> {
    public static readonly DrawableDrawComparer Instance = new();
    
    private DrawableDrawComparer() {}
    
    public int Compare(Drawable x, Drawable y) {
        if (ReferenceEquals(x, y))
            return 0;
        if (ReferenceEquals(null, y))
            return 1;
        if (ReferenceEquals(null, x))
            return -1;
        return y.Depth.CompareTo(x.Depth);
    }
}

public class DrawableInputComparer : IComparer<InputObject> {
    public static readonly DrawableInputComparer Instance = new();
    
    private DrawableInputComparer() {}
    
    public int Compare(InputObject y, InputObject x) {
        if (ReferenceEquals(x, y))
            return 0;
        if (ReferenceEquals(null, y))
            return 1;
        if (ReferenceEquals(null, x))
            return -1;
        return x.Depth.CompareTo(y.Depth);
    }
}
