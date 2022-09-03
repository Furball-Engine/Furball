using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Input.Events;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements; 

public class DrawableColorPicker : CompositeDrawable {
    public readonly Bindable<Color> Color;

    private readonly TexturedDrawable _colorDisplay;
    private readonly TextDrawable     _colorText;

    public DrawableColorPicker(Vector2 position, FontSystem font, int fontSize, Color initialColor) {
        this.Position = position;
        this.Color    = new Bindable<Color>(initialColor);

        this._colorText = new TextDrawable(new Vector2(0), font, this.Color.Value.ToHexString(), fontSize) {
            Clickable   = false,
            CoverClicks = false,
            Hoverable   = false,
            CoverHovers = false
        };
        this._colorDisplay = new TexturedDrawable(FurballGame.WhitePixel, new Vector2(this._colorText.Size.X + 10, 0)) {
            Scale         = new Vector2(this._colorText.Size.Y),
            ColorOverride = this.Color,
            Clickable     = false,
            CoverClicks   = false,
            Hoverable     = false,
            CoverHovers   = false
        };

        this.OnClick += this.OnColorDisplayClick;

        this.Color.OnChange += this.OnColorChange;

        this.Drawables.Add(this._colorText);
        this.Drawables.Add(this._colorDisplay);
    }

    private void OnColorChange(object sender, Color e) {
        this._colorDisplay.FadeColor(e, 100);
        this._colorText.Text = e.ToHexString();

        this._colorDisplay.MoveTo(new Vector2(this._colorText.Size.X + 10, 0));
    }

    private void OnColorDisplayClick(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
        EtoHelper.OpenColorPicker(
        (_, color) => {
            this.Color.Value = color;
        },
        this.Color.Value
        );
    }
}