using System.Numerics;
using Eto.Forms;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.Tweens;
using Furball.Engine.Engine.Graphics.Drawables.Tweens.TweenTypes;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Input.Events;
using Furball.Vixie.Backends.Shared;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements;

public class DrawableColorPicker : Drawable {
    private readonly float             _fontSize;
    private readonly DynamicSpriteFont _spriteFont;

    private string _text = "";

    public readonly Bindable<Color> Color;

    private ColorTween _colorSquareTween;

    private const float GAP = 5;

    public override Vector2 Size => (this._spriteFont.MeasureString(this._text) + new Vector2(GAP + this._fontSize, 0)) * this.Scale;

    public DrawableColorPicker(Vector2 position, FontSystem font, float fontSize, Color initialColor) {
        this._fontSize   = fontSize;
        this._spriteFont = font.GetFont(this._fontSize);

        this.Position = position;
        this.Color    = new Bindable<Color>(initialColor);

        this.OnClick += this.OnColorDisplayClick;

        this.Color.OnChange += this.OnColorChange;

        this.OnColorChange(this, this.Color);

        this.RegisterForInput();
    }

    public override void Update(double time) {
        base.Update(time);

        this._colorSquareTween.Update(this.DrawableTime);
    }

    private void OnColorChange(object sender, Color e) {
        this._text = e.ToHexString();

        this._colorSquareTween = new ColorTween(
        TweenType.Color,
        this._colorSquareTween?.GetCurrent() ?? this.Color,
        this.Color,
        this.DrawableTime,
        this.DrawableTime + 100
        );
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        //Draw the text
        batch.DrawString(this._spriteFont, this._text, args.Position, args.Color, args.Rotation, args.Scale);
        //Draw the square
        batch.Draw(
        FurballGame.WhitePixel,
        args.Position + new Vector2(this._spriteFont.MeasureString(this._text).X + GAP, 0) * args.Scale,
        args.Scale * this._fontSize,
        this._colorSquareTween.GetCurrent()
        );
    }

    private void OnColorDisplayClick(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
        EtoHelper.OpenColorPicker(
        (_, args) => {
            (DialogResult result, Color color) = args;

            if (result != DialogResult.Ok)
                return;

            this.Color.Value = color;
        },
        this.Color.Value
        );
    }
}
