
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables; 

public class TooltipDrawable : CompositeDrawable {
    public readonly  TextDrawable               TextDrawable;
    private readonly RectanglePrimitiveDrawable _backgroundRect;

    public TooltipDrawable() {
        this._backgroundRect = new RectanglePrimitiveDrawable(new Vector2(0), new Vector2(100, 20), 2, true) {
            ColorOverride = new Color(0, 0, 0, 155)
        };
        this.TextDrawable = new TextDrawable(new Vector2(0), FurballGame.DefaultFont, "", 20) {
            ColorOverride = Color.White
        };

        this.Children!.Add(this._backgroundRect);
        this.Children.Add(this.TextDrawable);

        this.Clickable   = false;
        this.CoverClicks = false;
        this.Hoverable   = false;
        this.CoverHovers = false;
    }

    /// <summary>
    ///     Sets the tooltip to the specified text
    /// </summary>
    /// <param name="text"></param>
    public void SetTooltip(string text) {
        this.TextDrawable.Text        = text;
        this._backgroundRect.RectSize = this.TextDrawable.Size;
    }
}