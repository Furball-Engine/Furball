using System;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using Xssp.MonoGame.Primitives2D;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    /// <summary>
    /// Creates a Simple Button Object
    /// </summary>
    public class UiButtonDrawable : ManagedDrawable {
        private string _text;
        public string Text => this._text;
        public TextDrawable TextDrawable;

        private float _margin;
        public float Margin => this._margin;

        public Color OutlineColor;
        public Color ButtonColor;

        public Color TextColor {
            get => this.TextDrawable.ColorOverride;
            set => this.TextDrawable.ColorOverride = value;
        }
        public float OutlineThickness = 2f;

        public Vector2 ButtonSize = new();

        public override Vector2 Size {
            get {
                if(this.ButtonSize == Vector2.Zero) {
                    (float textDrawableSizeX, float textDrawableSizeY) = this.TextDrawable.Size;

                    return new Vector2(textDrawableSizeX + this._margin * 2f, textDrawableSizeY + this._margin * 2f) * this.Scale;
                } 
                
                return this.ButtonSize * this.Scale;
            }
        }
        
        /// <summary>
        /// Creates a button
        /// </summary>
        /// <param name="position">Where to Draw the Button</param>
        /// <param name="text">What text should the button display?</param>
        /// <param name="font">What SpriteFont to use</param>
        /// <param name="buttonColor">Button Background Color</param>
        /// <param name="textColor">Button Text Color</param>
        /// <param name="outlineColor">Button Outline Color</param>
        /// <param name="buttonSize">The size of the button, set to Vector2.Zero for it to auto calculate</param>
        /// <param name="onClick">What happens when the button is clicked</param>
        /// <param name="margin">The margin between the text and the side of the button</param>
        public UiButtonDrawable(Vector2 position, string text, SpriteFont font, Color buttonColor, Color textColor, Color outlineColor, Vector2 buttonSize, EventHandler<Point> onClick = null, float margin = 5f) {
            this.Position     = position;
            this.TextDrawable = new TextDrawable(font, text);
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

        /// <summary>
        /// Creates a button
        /// </summary>
        /// <param name="position">Where to Draw the Button</param>
        /// <param name="text">What text should the button display?</param>
        /// <param name="font">What SpriteFont to use</param>
        /// <param name="textSize">What size to Draw at</param>
        /// <param name="buttonColor">Button Background Color</param>
        /// <param name="textColor">Button Text Color</param>
        /// <param name="outlineColor">Button Outline Color</param>
        /// <param name="buttonSize">The size of the button, set to Vector2.Zero for it to auto calculate</param>
        /// <param name="onClick">What happens when the button is clicked</param>
        /// <param name="margin">The margin between the text and the side of the button</param>
        /// <param name="charRange">SpriteFont character range</param>
        public UiButtonDrawable(Vector2 position, string text, byte[] font, float textSize, Color buttonColor, Color textColor, Color outlineColor, Vector2 buttonSize, EventHandler<Point> onClick = null, float margin = 5f, CharacterRange[] charRange = null) {
            this.Position     = position;
            this.TextDrawable = new TextDrawable(Vector2.Zero, font, text, textSize, charRange);
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

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.SpriteBatch.FillRectangle(args.Position * FurballGame.VerticalRatio, this.Size * FurballGame.VerticalRatio, args.Color, 0f);
            batch.SpriteBatch.DrawRectangle(args.Position * FurballGame.VerticalRatio, this.Size * FurballGame.VerticalRatio, this.OutlineColor, this.OutlineThickness * FurballGame.VerticalRatio, 0f);
            
            // FIXME: this is a bit of a hack, it should definitely be done differently
            DrawableManagerArgs tempArgs = args;
            if(this.ButtonSize == Vector2.Zero) {
                tempArgs.Position.X += this._margin;
                tempArgs.Position.Y += this._margin;
            } else {
                (float textX, float textY) = this.TextDrawable.Size;

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
