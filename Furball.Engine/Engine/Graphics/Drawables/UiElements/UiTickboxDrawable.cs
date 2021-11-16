using System.Drawing;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Helpers;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    public class UiTickboxDrawable : CompositeDrawable {
        public string Text {
            get => this._textDrawable.Text;
            set => this._textDrawable.Text = value;
        }

        public readonly Bindable<bool> Selected = new(false);

        private readonly TextDrawable               _textDrawable;
        private readonly RectanglePrimitiveDrawable _rectangleDrawable;

        public UiTickboxDrawable(Vector2 position, string text, int size, bool initialState = false, bool managed = false) {
            this.Position       = position;
            this.Selected.Value = initialState;

            this._textDrawable = new(new(30, 0), FurballGame.DEFAULT_FONT, text, size);

            float fontHeight = this._textDrawable.Font.MeasureString("A").Y;

            this._rectangleDrawable = new(Vector2.Zero, new(fontHeight - 5f), 1f, initialState);

            this._textDrawable.Position = new(fontHeight + 5f, -7.5f);

            this.Drawables.Add(this._textDrawable);
            this.Drawables.Add(this._rectangleDrawable);

            this.Selected.OnChange += this.OnSelectChange;

            if (!managed)
                this.OnClick += this.OnDrawableClick;
        }

        private void OnDrawableClick(object sender, Point e) {
            this.Selected.Value = !this.Selected;
        }

        private void OnSelectChange(object sender, bool e) {
            this._rectangleDrawable.Filled = e;
        }

        public override void Dispose() {
            this.Selected.OnChange -= this.OnSelectChange;
            this.OnClick           -= this.OnDrawableClick;

            base.Dispose();
        }
    }
}
