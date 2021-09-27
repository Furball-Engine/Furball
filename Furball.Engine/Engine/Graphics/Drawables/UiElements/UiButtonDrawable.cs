using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Microsoft.Xna.Framework;
using SpriteFontPlus;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    public class UiButtonDrawable : ManagedDrawable {
        private string _text;
        public string Text => this._text;
        public TextDrawable TextDrawable;

        private float _margin;
        public float Margin => this._margin;

        public Color OutlineColor;
        public Color ButtonColor;
        public float OutlineThickness = 2f;

        public override Vector2 Size {
            get {
                (float textDrawableSizeX, float textDrawableSizeY) = this.TextDrawable.Size;

                return new Vector2(textDrawableSizeX + this._margin * 2f, textDrawableSizeY + this._margin * 2f);
            }
        }

        public UiButtonDrawable(string text, byte[] font, float size, Color buttonColor, Color textColor, Color outlineColor, float margin = 5f, CharacterRange[] charRange = null) {
            this.TextDrawable = new TextDrawable(Vector2.Zero, font, text, size, charRange);
            this._margin      = margin;
            this._text        = text;

            this.TextDrawable.ColorOverride = textColor;
            this.OutlineColor               = outlineColor;
            this.ButtonColor                = buttonColor;
            this.ColorOverride              = buttonColor;

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
            // batch.ShapeBatch.FillRectangle(args.Position - args.Origin, this.Size, args.Color);
            batch.ShapeBatch.DrawRectangle(args.Position - args.Origin, this.Size, args.Color, this.OutlineColor, this.OutlineThickness);

            // FIXME: this is a bit of a hack, it should definitely be done differently
            DrawableManagerArgs tempArgs = args;
            tempArgs.Position.X += this._margin;
            tempArgs.Position.Y += this._margin;
            tempArgs.Color      =  this.TextDrawable.ColorOverride;
            this.TextDrawable.Draw(time, batch, tempArgs);
        }
    }
}
