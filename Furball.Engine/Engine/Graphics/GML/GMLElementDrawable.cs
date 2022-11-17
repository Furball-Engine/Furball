using Furball.Engine.Engine.Graphics.Drawables;

namespace Furball.Engine.Engine.Graphics.GML; 

public class GMLElementDrawable : Drawable {
    private readonly GMLTheme                 _theme;
    private readonly GMLCalculatedElementSize MinimumSize;
    
    public GMLElementDrawable(IGMLElement element, GMLTheme theme) {
        this._theme      = theme;
        this.MinimumSize = element.MinimumSize();
    }
}
