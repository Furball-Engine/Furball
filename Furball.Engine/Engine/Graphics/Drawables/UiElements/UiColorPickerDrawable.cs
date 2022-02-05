using System.Drawing;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Helpers;
using Silk.NET.Input;
using Color=Furball.Vixie.Graphics.Color;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    public class UiColorPickerDrawable : CompositeDrawable {
        public readonly Bindable<Color> Color;

        private readonly TexturedDrawable _colorDisplay;
        private readonly TextDrawable     _colorText;

        public UiColorPickerDrawable(Vector2 position, FontSystem font, int size, Color initialColor) {
            this.Position = position;
            this.Color    = new(initialColor);

            this._colorText = new(new(0), font, this.Color.Value.ToHexString(), size);
            this._colorDisplay = new(FurballGame.WhitePixel, new(this._colorText.Size.X + 10, 0)) {
                Scale         = new(this._colorText.Size.Y),
                ColorOverride = this.Color
            };

            this.OnClick += this.OnColorDisplayClick;

            this.Color.OnChange += this.OnColorChange;

            this._drawables.Add(this._colorText);
            this._drawables.Add(this._colorDisplay);
        }

        private void OnColorChange(object sender, Color e) {
            this._colorDisplay.FadeColor(e, 100);
            this._colorText.Text = e.ToHexString();

            this._colorDisplay.MoveTo(new(this._colorText.Size.X + 10, 0));
        }

        private void OnColorDisplayClick(object? sender, (MouseButton button, Point pos) valueTuple) {
            EtoHelper.OpenColorPicker(
            (o, color) => {
                this.Color.Value = color;
            },
            this.Color.Value
            );
        }
    }
}
