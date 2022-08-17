 
using System;
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
    public DynamicSpriteFont RealFont;

    private string _text;

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

        int fontSize = (int)(this.Font.FontSize * FurballGame.VerticalRatio);

        (FontSystem FontSystem, int fontSize) key = (this.Font.FontSystem, fontSize);
        
        ContentManager.FSS_CACHE.TryGetValue(key, out WeakReference<DynamicSpriteFont> fontRef);

        DynamicSpriteFont font = null;

        bool valid = fontRef?.TryGetTarget(out font) ?? false;

        if (!valid)
            ContentManager.FSS_CACHE.Remove(key);
        
        this.Font.FontSystem.Reset();
        if (fontRef == null || !valid) {
            this.RealFont = this.Font.FontSystem.GetFont(fontSize);
            ContentManager.FSS_CACHE.Add((this.Font.FontSystem, fontSize), new WeakReference<DynamicSpriteFont>(this.RealFont));
            Logger.Log($"Caching DynamicSpriteFont of size {fontSize}", LoggerLevelCacheEvent.Instance);
        } else {
            this.RealFont = font;
        }
    }

    private bool _isDisposed;
    public override void Dispose() {
        if (this._isDisposed)
            return;

        this._isDisposed = true;
        
        this.ClearEvents();
        
        base.Dispose();
    }

    public override void ClearEvents() {
        base.ClearEvents();

        FurballGame.Instance.WindowManager.OnFramebufferResize -= this.OnFramebufferResize;
    }

    public void SetFont(FontSystem font, int fontSize) {
        (FontSystem FontSystem, int fontSize) key = (font, fontSize);
        
        ContentManager.FSS_CACHE.TryGetValue(key, out WeakReference<DynamicSpriteFont> fontRef);

        DynamicSpriteFont f = null;

        bool valid = fontRef?.TryGetTarget(out f) ?? false;

        if (!valid)
            ContentManager.FSS_CACHE.Remove(key);
        
        if (fontRef == null || !valid) {
            this.Font = font.GetFont(fontSize);
            ContentManager.FSS_CACHE.Add((font, fontSize), new WeakReference<DynamicSpriteFont>(this.Font));
            Logger.Log($"Caching DynamicSpriteFont of size {fontSize}", LoggerLevelCacheEvent.Instance);
        } else {
            this.Font = f;
        }
        this.OnFramebufferResize(null, FurballGame.Instance.WindowManager.WindowSize);
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