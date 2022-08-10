
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Helpers.Logger;
using Kettu;

namespace Furball.Engine.Engine.Graphics.Drawables; 

/// <summary>
/// Simple way to Draw Text
/// </summary>
public class TextDrawable : Drawable {
    /// <summary>
    /// SpriteFont that gets used during Drawing
    /// </summary>
    public DynamicSpriteFont Font;
    public  DynamicSpriteFont RealFont;
    private string            _text;

    /// <summary>
    /// Text that gets drawn
    /// </summary>
    public string Text {
        get => this._text;
        set {
            this._text = value;
            this.RecalculateSize();
        }
    }

    private Vector2 _sizeCache;

    private void RecalculateSize() => this._sizeCache = this.Font.MeasureString(this.Text);

    /// <summary>
    /// The height of the text
    /// </summary>
    public override Vector2 Size => this._sizeCache * this.Scale;
    public List<Rectangle> TextRectangles => this.Font.GetGlyphRects(this.Text, Vector2.Zero, Vector2.Zero, this.Scale);

    public string PreWrapText { get; protected set; }

    public void Wrap(float width, bool setPrewrap = true) {
        if(setPrewrap)
            this.PreWrapText = this.Text;

        List<Rectangle> rects = this.TextRectangles;

        for (int i = 0; i < rects.Count; i++) {
            Rectangle rectangle = rects[i];

            if (rectangle.Right > width) {
                this.Text = this.Text.Insert(i, "\n");

                rects = this.TextRectangles;

                i -= 1;
            }
        }
    }
        
    /// <summary>
    ///     The color type of the text, Solid means a single color, Repeating means the pattern in Colors repeats, and Stretch
    ///     means the colours stretch to fit
    /// </summary>
    public TextColorType ColorType = TextColorType.Solid;
    /// <summary>
    ///     An array of colours for the text drawable to use depending on the TextColorType
    /// </summary>
    public Color[] Colors = {
        Color.Cyan,
        Color.Pink,
        Color.White,
        Color.Pink,
        Color.Cyan
    };
        
    /// <summary>
    /// Creates a new TextDrawable
    /// </summary>
    /// <param name="position">Where to Draw</param>
    /// <param name="font">A byte[] containing the font in ttf form)</param>
    /// <param name="text">What Text to Draw (can be changed later)</param>
    /// <param name="fontSize">The size of the text as a float</param>
    public TextDrawable(Vector2 position, FontSystem font, string text, int fontSize) {
        this.Position = position;

        this.SetFont(font, fontSize);

        this.Text = text;

        FurballGame.Instance.WindowManager.OnFramebufferResize += this.OnFramebufferResize;
    }

    private float _difference = 1;
    private void OnFramebufferResize(object sender, Vector2 e) {
        this._difference = (int)(this.Font.FontSize * FurballGame.VerticalRatio) / (this.Font.FontSize * FurballGame.VerticalRatio);

        this.Font.FontSystem.Reset();
        this.RealFont = this.Font.FontSystem.GetFont((int)(this.Font.FontSize * FurballGame.VerticalRatio));
    }

    public override void Dispose() {
        base.Dispose();

        this.ClearEvents();
    }

    public override void ClearEvents() {
        base.ClearEvents();

        FurballGame.Instance.WindowManager.OnFramebufferResize -= this.OnFramebufferResize;
    }

    public void SetFont(FontSystem font, int fontSize) {
        if (!ContentManager.FSS_CACHE.TryGetValue((font, fontSize), out this.Font)) {
            this.Font = font.GetFont(fontSize);
            ContentManager.FSS_CACHE.Add((font, fontSize), this.Font);
            Logger.Log($"Caching DynamicSpriteFont of size {fontSize}", LoggerLevelCacheEvent.Instance);
        }

        this.Font = font.GetFont(fontSize);
        this.OnFramebufferResize(this, FurballGame.Instance.WindowManager.WindowSize);
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        switch (this.ColorType) {
            case TextColorType.Repeating: {
                batch.DrawString(
                this.RealFont,
                this.Text,
                args.Position,
                this.Colors,
                args.Rotation,
                args.Scale / FurballGame.VerticalRatio / this._difference,
                this.RotationOrigin
                );
                break;
            }
            case TextColorType.Solid: {
                batch.DrawString(
                this.RealFont,
                this.Text,
                args.Position,
                args.Color,
                args.Rotation,
                args.Scale / FurballGame.VerticalRatio / this._difference,
                this.RotationOrigin
                );
                break;
            }
            case TextColorType.Stretch: {
                batch.DrawString(
                this.Font,
                this.Text,
                args.Position,
                ArrayHelper.FitElementsInANewArray(this.Colors, this.Text.Length),
                args.Rotation,
                args.Scale / FurballGame.VerticalRatio / this._difference,
                this.RotationOrigin
                );
                break;
            }
        }
    }
}

public enum TextColorType {
    /// <summary>
    ///     A single color for the whole text
    /// </summary>
    Solid,
    /// <summary>
    ///     A few colors repeated constantly
    /// </summary>
    Repeating,
    /// <summary>
    ///     A few colors stretched to fit the text
    /// </summary>
    Stretch
}