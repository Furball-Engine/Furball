using System;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Engine.Engine.Input.Events;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements; 

/// <summary>
/// Creates a Simple Button Object
/// </summary>
public class DrawableButton : CompositeDrawable {
    /// <summary>
    ///     The text to display on the button
    /// </summary>
    public string Text {
        get => this.TextDrawable.Text;
        set => this.TextDrawable.Text = value;
    }

    /// <summary>
    ///     The internal TextDrawable used to display the text
    /// </summary>
    public TextDrawable TextDrawable;
    private RectanglePrimitiveDrawable _backgroundDrawable;
    private RectanglePrimitiveDrawable _outlineDrawable;
    
    /// <summary>
    ///     The margin between the text and the button's edge
    /// </summary>
    private float _margin;
    /// <summary>
    ///     The margin between the text and the button's edge
    /// </summary>
    public float Margin => this._margin;

    /// <summary>
    ///     The outline color of the button
    /// </summary>
    public Color OutlineColor {
        get => this._outlineDrawable.ColorOverride;
        set => this._outlineDrawable.ColorOverride = value;
    }

    private Color _buttonColor;
    /// <summary>
    ///     The fill color of the button
    /// </summary>
    public Color ButtonColor {
        get => this._buttonColor;
        set {
            this._backgroundDrawable.ColorOverride = value;
            this._buttonColor = value;
        }
    }

    /// <summary>
    ///     The color of the text
    /// </summary>
    public Color TextColor {
        get => this.TextDrawable.ColorOverride;
        set => this.TextDrawable.ColorOverride = value;
    }
    /// <summary>
    ///     The thickness of the outline
    /// </summary>
    private float _outlineThickness = 5f;

    /// <summary>
    ///     The size of the button, Vector2.Zero means autosize it
    /// </summary>
    public Vector2 ButtonSize = new();

    public override Vector2 Size {
        get {
            if(this.ButtonSize == Vector2.Zero) {
                return new Vector2(this.TextDrawable.Size.X + this._margin * 2f, this.TextDrawable.Size.Y + this._margin * 2f) * this.Scale;
            } 
                
            return this.ButtonSize * this.Scale;
        }
    }

    /// <summary>
    ///     The thickness of the outline
    /// </summary>
    public float OutlineThickness {
        get => this._outlineThickness;
        set {
            this._outlineThickness = value;

            this._outlineDrawable.Thickness = value;
        }
    }

    /// <summary>
    /// Creates a button
    /// </summary>
    /// <param name="position">Where to Draw the Button</param>
    /// <param name="font">What font to use</param>
    /// <param name="textSize">What size to Draw at</param>
    /// <param name="text">What text should the button display?</param>
    /// <param name="buttonColor">Button Background Color</param>
    /// <param name="textColor">Button Text Color</param>
    /// <param name="outlineColor">Button Outline Color</param>
    /// <param name="buttonSize">The size of the button, set to Vector2.Zero for it to auto calculate</param>
    /// <param name="onClick">What happens when the button is clicked</param>
    /// <param name="margin">The margin between the text and the side of the button</param>
    public DrawableButton(
        Vector2 position, FontSystem font, float textSize, string text, Color buttonColor, Color textColor, Color outlineColor, Vector2 buttonSize,
        EventHandler<MouseButtonEventArgs> onClick = null, float margin = 5f
    ) {
        this.Position           = position;
        this.TextDrawable       = new TextDrawable(Vector2.Zero, font, text, textSize);
        this._backgroundDrawable = new RectanglePrimitiveDrawable(Vector2.Zero, this.ButtonSize, 0,                     true);
        this._outlineDrawable    = new RectanglePrimitiveDrawable(Vector2.Zero, this.ButtonSize, this.OutlineThickness, false);
        this._margin            = margin;
        this.Text               = text;

        this.TextColor    = textColor;
        this.OutlineColor = outlineColor;
        this.ButtonColor  = buttonColor;
        this.ButtonSize   = buttonSize;
        
        this.OnClick += onClick;

        this.TextDrawable.OriginType = OriginType.Center;
        
        this.Children!.Add(this._backgroundDrawable);
        this.Children.Add(this._outlineDrawable);
        this.Children.Add(this.TextDrawable);

        this.ChildrenInvisibleToInput = true;

        this.OnHover += delegate {
            if (!this.Clickable) return;
                
            this._backgroundDrawable.Tweens.Add(
            new ColorTween(
            TweenType.Color,
            this.ButtonColor,
            new Color(this.ButtonColor.R - 50, this.ButtonColor.G - 50, this.ButtonColor.B - 50),
            this.TimeSource.GetCurrentTime(),
            this.TimeSource.GetCurrentTime() + 150
            )
            );
        };
        this.OnHoverLost += delegate {
            if (!this.Clickable) return;
            
            this._backgroundDrawable.Tweens.Add(
            new ColorTween(TweenType.Color, this.ColorOverride, this.ButtonColor, this.TimeSource.GetCurrentTime(), this.TimeSource.GetCurrentTime() + 150)
            );
        };
        
        this.RegisterForInput();
    }

    public override void Update(double time) {
        base.Update(time);

        this._backgroundDrawable.RectSize = this.Size / this.Scale;
        this._outlineDrawable.RectSize = this.Size / this.Scale;
        
        if (this.ButtonSize == Vector2.Zero) {
            this.TextDrawable.Position   = new Vector2(this.Margin);
            this.TextDrawable.OriginType = OriginType.TopLeft;
        } else {
            Vector2 scaledMargin = new(this.Margin);
            
            switch (this.TextDrawable.OriginType) {
                case OriginType.Center: {
                    this.TextDrawable.Position = this.ButtonSize / 2f;
                        
                    break;
                }
                case OriginType.TopLeft: {
                    this.TextDrawable.Position = scaledMargin;
                    
                    break;
                }
                case OriginType.BottomRight: {
                    this.TextDrawable.Position.X += this.ButtonSize.X - scaledMargin.X;
                    this.TextDrawable.Position.Y += this.ButtonSize.Y - scaledMargin.Y;
                        
                    break;
                }
                case OriginType.TopRight: {
                    this.TextDrawable.Position.X += this.ButtonSize.X - scaledMargin.X;
                    this.TextDrawable.Position.Y += scaledMargin.Y;
                        
                    break;
                }
                case OriginType.BottomLeft: {
                    this.TextDrawable.Position.X += scaledMargin.X;
                    this.TextDrawable.Position.Y += this.ButtonSize.Y - scaledMargin.Y;
                        
                    break;
                }
                case OriginType.TopCenter: {
                    this.TextDrawable.Position.X += this.ButtonSize.X / 2;
                    this.TextDrawable.Position.Y += scaledMargin.Y;
                        
                    break;
                }
                case OriginType.BottomCenter: {
                    this.TextDrawable.Position.X += this.ButtonSize.X / 2;
                    this.TextDrawable.Position.Y += this.ButtonSize.Y - scaledMargin.Y;
                        
                    break;
                }
                case OriginType.LeftCenter: {
                    this.TextDrawable.Position.X += scaledMargin.X;
                    this.TextDrawable.Position.Y += this.ButtonSize.Y / 2;
                        
                    break;
                }
                case OriginType.RightCenter: {
                    this.TextDrawable.Position.X += this.ButtonSize.X - scaledMargin.X;
                    this.TextDrawable.Position.Y += this.ButtonSize.Y / 2;
                        
                    break;
                }
            }
        }
    }

    // public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
    //     batch.FillRectangle(args.Position, this.RealSize, args.Color);
    //     batch.DrawRectangle(args.Position, this.RealSize, this.OutlineThickness * args.Scale.Y, this.OutlineColor);
    //
    //     base.Draw(time, batch, args);
    // }
}