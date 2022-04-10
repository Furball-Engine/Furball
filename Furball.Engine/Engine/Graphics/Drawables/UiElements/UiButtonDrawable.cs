using System;
using System.Drawing;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Silk.NET.Input;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    /// <summary>
    /// Creates a Simple Button Object
    /// </summary>
    public class UiButtonDrawable : ManagedDrawable {
        /// <summary>
        ///     The text to display on the button
        /// </summary>
        private string _text;
        /// <summary>
        ///     The text to display on the button
        /// </summary>
        public string Text => this._text;
        /// <summary>
        ///     The internal TextDrawable used to display the text
        /// </summary>
        public TextDrawable TextDrawable;

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
        public Color OutlineColor;
        /// <summary>
        ///     The fill color of the button
        /// </summary>
        public Color ButtonColor;

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
        public float OutlineThickness = 5f;

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
        /// Creates a button
        /// </summary>
        /// <param name="position">Where to Draw the Button</param>
        /// <param name="text">What text should the button display?</param>
        /// <param name="font">What font to use</param>
        /// <param name="textSize">What size to Draw at</param>
        /// <param name="buttonColor">Button Background Color</param>
        /// <param name="textColor">Button Text Color</param>
        /// <param name="outlineColor">Button Outline Color</param>
        /// <param name="buttonSize">The size of the button, set to Vector2.Zero for it to auto calculate</param>
        /// <param name="onClick">What happens when the button is clicked</param>
        /// <param name="margin">The margin between the text and the side of the button</param>
        public UiButtonDrawable(Vector2 position, string text, FontSystem font, int textSize, Color buttonColor, Color textColor, Color outlineColor, Vector2 buttonSize, EventHandler<
                                    (MouseButton, Point)> onClick = null, float margin = 5f) {
            this.Position     = position;
            this.TextDrawable = new TextDrawable(Vector2.Zero, font, text, textSize);
            this._margin      = margin;
            this._text        = text;

            this.TextColor     = textColor;
            this.OutlineColor  = outlineColor;
            this.ButtonColor   = buttonColor;
            this.ColorOverride = buttonColor;
            this.ButtonSize    = buttonSize;

            this.OnClick += onClick;

            this.TextDrawable.OriginType = OriginType.Center;

            this.OnHover += delegate {
                if (!this.Clickable) return;
                
                this.Tweens.Add(
                    new ColorTween(
                        TweenType.Color,
                        this.ButtonColor,
                        new Color(this.ButtonColor.R + 50, this.ButtonColor.G + 50, this.ButtonColor.B + 50),
                        this.TimeSource.GetCurrentTime(),
                        this.TimeSource.GetCurrentTime() + 150
                    )
                );
            };
            this.OnHoverLost += delegate {
                if (!this.Clickable) return;
            
                this.Tweens.Add(
                    new ColorTween(TweenType.Color, this.ColorOverride, this.ButtonColor, this.TimeSource.GetCurrentTime(), this.TimeSource.GetCurrentTime() + 150)
                );
            };
        }

        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.FillRectangle(args.Position * FurballGame.VerticalRatio, this.Size * FurballGame.VerticalRatio, args.Color, 0f);
            batch.DrawRectangle(args.Position * FurballGame.VerticalRatio, this.Size * FurballGame.VerticalRatio, this.OutlineThickness * FurballGame.VerticalRatio, this.OutlineColor);

            // FIXME: this is a bit of a hack, it should definitely be done differently
            DrawableManagerArgs tempArgs = args;
            if(this.ButtonSize == Vector2.Zero) {
                tempArgs.Position.X += this._margin;
                tempArgs.Position.Y += this._margin;
            } else {
                float textX = this.TextDrawable.Size.X;
                float textY = this.TextDrawable.Size.Y;

                switch (this.TextDrawable.OriginType) {
                    case OriginType.Center: {
                        tempArgs.Position.X += this.ButtonSize.X / 2 - textX / 2;
                        tempArgs.Position.Y += this.ButtonSize.Y / 2 - textY / 2;
                        
                        break;
                    }
                    case OriginType.TopLeft: {
                        tempArgs.Position.X += this.Margin;
                        tempArgs.Position.Y += this.Margin;
                        
                        break;
                    }
                    case OriginType.BottomRight: {
                        tempArgs.Position.X += this.Size.X - this.Margin - textX;
                        tempArgs.Position.Y += this.Size.Y - this.Margin - textY;
                        
                        break;
                    }
                    case OriginType.TopRight: {
                        tempArgs.Position.X += this.Size.X - this.Margin - textX;
                        tempArgs.Position.Y += this.Margin;
                        
                        break;
                    }
                    case OriginType.BottomLeft: {
                        tempArgs.Position.X += this.Margin;
                        tempArgs.Position.Y += this.Size.Y - this.Margin - textY;
                        
                        break;
                    }
                    case OriginType.TopCenter: {
                        tempArgs.Position.X += this.ButtonSize.X / 2 - textX / 2;
                        tempArgs.Position.Y += this.Margin;
                        
                        break;
                    }
                    case OriginType.BottomCenter: {
                        tempArgs.Position.X += this.ButtonSize.X / 2 - textX / 2;
                        tempArgs.Position.Y += this.Size.Y - this.Margin - textY;
                        
                        break;
                    }
                    case OriginType.LeftCenter: {
                        tempArgs.Position.X += this.Margin;
                        tempArgs.Position.Y += this.ButtonSize.Y / 2 - textY / 2;
                        
                        break;
                    }
                    case OriginType.RightCenter: {
                        tempArgs.Position.X += this.Size.X - this.Margin - textX;
                        tempArgs.Position.Y += this.ButtonSize.Y / 2 - textY / 2;
                        
                        break;
                    }
                }
            }
            tempArgs.Color      =  this.TextDrawable.ColorOverride;
            this.TextDrawable.Draw(time, batch, tempArgs);
        }
    }
}
