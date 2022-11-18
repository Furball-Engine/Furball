using System;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Graphics.GML.Elements;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.GML; 

public class GMLElementDrawable : CompositeDrawable {
    private readonly GMLTheme                 _theme;
    public readonly  GMLCalculatedElementSize MinimumSize;
    public readonly  IGMLElement              Element;

    public Vector2 CalculatedSize;

    public override Vector2 Size => this.CalculatedSize;

    public GMLElementDrawable(IGMLElement element, GMLTheme theme) {
        this.Element = element;
        
        this._theme      = theme;
        this.MinimumSize = element.MinimumSize();
        
        this.Invalidate();

        this.InvisibleToInput = true;
    }

    public void Invalidate() {
        this.Children.Clear();

        //If the size of the element is zero, just do nothing 
        if (this.CalculatedSize == Vector2.Zero)
            return;
        
        switch (this.Element) {
            case GMLLabelElement label: {
                Vector2 position = label.OriginType switch {
                    OriginType.TopLeft      => Vector2.Zero,
                    OriginType.TopRight     => new Vector2(this.CalculatedSize.X, 0),
                    OriginType.BottomLeft   => new Vector2(0,                     this.CalculatedSize.Y),
                    OriginType.BottomRight  => this.CalculatedSize,
                    OriginType.Center       => this.CalculatedSize / 2f,
                    OriginType.TopCenter    => new Vector2(this.CalculatedSize.X / 2f, 0),
                    OriginType.BottomCenter => new Vector2(this.CalculatedSize.X / 2f, this.CalculatedSize.Y),
                    OriginType.LeftCenter   => new Vector2(0,                          this.CalculatedSize.Y / 2f),
                    OriginType.RightCenter  => new Vector2(this.CalculatedSize.X,      this.CalculatedSize.Y / 2f),
                    _                       => throw new ArgumentOutOfRangeException()
                };

                this.Children.Add(new TextDrawable(position, label.Font, label.Text) {
                    OriginType = label.OriginType
                });
                
                break;
            }
            case GMLButtonElement button: {
                this.Children.Add(
                new DrawableButton(Vector2.Zero, button.Font.FontSystem, button.Font.FontSize, button.Text, Color.Blue, Color.White, Color.Black, this.CalculatedSize) {
                    OutlineThickness = 1
                }
                );

                break;
            }
        }
    }
}
