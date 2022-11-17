using System;
using System.Linq;
using System.Numerics;
using FontStashSharp;
using GMLSharp;
using Object=GMLSharp.Object;

namespace Furball.Engine.Engine.Graphics.GML.Elements;

public class GMLButtonElement : IGMLElement {
    private readonly Object   _object;
    private readonly GMLTheme _theme;

    public string            Text;
    public DynamicSpriteFont Font;

    public GMLButtonElement(Object obj, GMLTheme theme) {
        this._object = obj;
        this._theme  = theme;

        this.Invalidate();
    }

    public GMLCalculatedElementSize MinimumSize() {
        GMLSize? x = null;
        GMLSize? y = null;

        float? fixedW = this._object.FixedWidth();
        float? fixedH = this._object.FixedHeight();

        if (fixedW != null)
            x = new GMLSize(fixedW.Value);
        if (fixedH != null)
            y = new GMLSize(fixedH.Value);

        //The default width behaviopur of buttons is to expand to fit, with no max or min size
        x ??= new GMLSize {
            Type = SizeType.ExpandToFit
        };
        //The default height of buttons is a fixed 22px
        y ??= new GMLSize(22);

        return new GMLCalculatedElementSize {
            Width  = x.Value,
            Height = y.Value
        };
    }

    public bool FillWithBackgroundColor() {
        return this._object.FillWithBackgroundColor();
    }
    
    public void Invalidate() {
        this.Text = "";

        if (this._object.Properties.LastOrDefault(
            x => x is KeyValuePair {
                Key: "text"
            }
            ) is KeyValuePair {
                Value: JsonValueNode {
                    Value: {}
                } text
            })
            this.Text = Convert.ToString(text.Value);

        //TODO: support variable font sized in GML
        this.Font = this._theme.Font.GetFont(this._theme.DefaultFontSize);
    }
}
