#nullable enable
using System;
using System.Linq;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables;
using GMLSharp;
using Object=GMLSharp.Object;

namespace Furball.Engine.Engine.Graphics.GML.Elements;

public class GMLLabelElement : IGMLElement {
    private readonly Object   _object;
    private readonly GMLTheme _theme;

    private string            _text = null!;
    private DynamicSpriteFont _font = null!;

    public GMLLabelElement(Object obj, GMLTheme theme) {
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

        //TODO: Add support for text wrapping, this is part of the default behaviour of GUI::Label
        Vector2 measuredSize = this._font.MeasureString(this._text);

        //If the size is not set by fixed_width/height, just use the measured size, that can expand to fit
        x ??= new GMLSize {
            Type = SizeType.ExpandToFit, 
            Size = measuredSize.X
        };
        y ??= new GMLSize {
            Type = SizeType.ExpandToFit, 
            Size = measuredSize.Y
        };

        return new GMLCalculatedElementSize {
            Width  = x.Value,
            Height = y.Value
        };
    }

    public OriginType GetOriginType() {
        return this._object.TextAlignment() switch {
            "Center"      => OriginType.Center,
            "CenterLeft"  => OriginType.LeftCenter,
            "CenterRight" => OriginType.RightCenter,
            "TopLeft"     => OriginType.TopLeft,
            "TopRight"    => OriginType.TopRight,
            "BottomLeft"  => OriginType.BottomLeft,
            "BottomRight" => OriginType.BottomRight,
            _             => OriginType.Center
        };
    }

    public bool FillWithBackgroundColor() {
        return this._object.FillWithBackgroundColor();
    }

    public void Invalidate() {
        this._text = "";

        if (this._object.Properties.LastOrDefault(
            x => x is KeyValuePair {
                Key: "text"
            }
            ) is KeyValuePair {
                Value: JsonValueNode {
                    Value: {}
                } text
            })
            this._text = Convert.ToString(text.Value);

        //TODO: support variable font sized in GML
        this._font = this._theme.Font.GetFont(this._theme.DefaultFontSize);
    }
}
