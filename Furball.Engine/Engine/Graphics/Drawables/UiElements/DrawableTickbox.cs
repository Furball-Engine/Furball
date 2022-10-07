using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Input.Events;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements; 

public class DrawableTickbox : CompositeDrawable {
    public string Text {
        get => this._textDrawable.Text;
        set => this._textDrawable.Text = value;
    }

    public readonly Bindable<bool> Selected = new(false);

    private readonly TextDrawable               _textDrawable;
    private readonly RectanglePrimitiveDrawable _rectangleDrawable;

    public DrawableTickbox(Vector2 position, FontSystem font, int fontSize, string text, bool initialState = false, bool managed = false) {
        this.Position       = position;
        this.Selected.Value = initialState;

        this._textDrawable = new TextDrawable(new Vector2(30, 0), font, text, fontSize) {
            Clickable   = false,
            CoverClicks = false
        };

        float fontHeight = this._textDrawable.Font.MeasureString("A").Y;

        this._rectangleDrawable = new RectanglePrimitiveDrawable(Vector2.Zero, new Vector2(fontHeight - 5f), 1f, initialState) {
            Clickable   = false,
            CoverClicks = false
        };

        this._textDrawable.Position = new Vector2(fontHeight + 5f, -7.5f);

        this.Children!.Add(this._textDrawable);
        this.Children.Add(this._rectangleDrawable);

        this.Selected.OnChange += this.OnSelectChange;

        if (!managed)
            this.OnClick += this.OnDrawableClick;
    }

    private void OnDrawableClick(object sender, MouseButtonEventArgs mouseButtonEventArgs) {
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