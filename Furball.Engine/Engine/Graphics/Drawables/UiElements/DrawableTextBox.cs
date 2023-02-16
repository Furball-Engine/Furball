using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Input;
using Furball.Engine.Engine.Input.Events;
using Silk.NET.Input;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements; 

/// <summary>
/// Creates a Basic Textbox
/// </summary>
public partial class DrawableTextBox : CompositeDrawable, ICharInputHandler {
    /// <summary>
    ///     The width of the text box
    /// </summary>
    public float TextBoxWidth;

    public string Text {
        get => this._textDrawable.Text;
        set => this._textDrawable.Text = value;
    }

    public int LineCount {
        get => this._lineCount;
        set {
            this._lineCount = value;
            this.RecalcOutline();
        }
    }

    public DynamicSpriteFont Font => this._textDrawable.Font;
    
    public List<Glyph> TextRectangles => this._textDrawable.TextRectangles;

    private bool _selected;
    /// <summary>
    ///     Whether the text box was selected
    /// </summary>
    public bool Selected {
        get => this._selected;
        private set {
            if (value == this._selected) return;

            this._selected = value;
            this.OnFocusChange?.Invoke(this, value);
        }
    }
    public bool ClearOnCommit    = false;
    public bool DeselectOnCommit = true;

    private TextDrawable               _textDrawable;
    private RectanglePrimitiveDrawable _outline;
    private TexturedDrawable           _caret;

    /// <summary>
    /// The range of characters in the string that are selected
    /// </summary>
    public readonly Bindable<Range> SelectedRange = new(new Range(0, 0));
    private int _lineCount;

    public override Vector2 Size => new Vector2(this.TextBoxWidth, this._textDrawable.Font.LineHeight * this.LineCount) * this.Scale;
    /// <summary>
    /// Creates a Textbox
    /// </summary>
    /// <param name="position">Where to Draw</param>
    /// <param name="font">What font to use</param>
    /// <param name="fontSize">Size of the text</param>
    /// <param name="width">Width/Length of the Textbox</param>
    /// <param name="text">Initial Text</param>
    /// <param name="lineCount">Amount of lines that are able to fit in the text box</param>
    public DrawableTextBox(Vector2 position, FontSystem font, int fontSize, float width, string text = "", int lineCount = 1) {
        if (lineCount < 1)
            throw new Exception("Invalid line count!");
        
        this._lineCount = lineCount;

        this.Position      = position;
        this.TextBoxWidth  = width;
        
        this._textDrawable = new TextDrawable(Vector2.Zero, font, text, fontSize);
        this._outline = new RectanglePrimitiveDrawable(Vector2.Zero, this._textDrawable.Size, 1f, false) {
            ColorOverride = Color.DarkGray
        };
        this._caret = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
            Visible       = true,
            Scale         = new Vector2(2f, this._textDrawable.Font.LineHeight),
            ColorOverride = new Color(1, 1, 1, 0f),
            Clickable     = false,
            CoverClicks   = false,
            Hoverable     = false,
            CoverHovers   = false,
        };

        this.Children!.Add(this._textDrawable);
        this.Children.Add(this._outline);
        this.Children.Add(this._caret);

        this.RegisterHandlers();
        this.RecalcOutline();
        this.RegisterForInput();
    }
        
    private void UpdateCaretPosition(bool instant) {
        List<Glyph> rects = this.TextRectangles;
            
        if (this.SelectedRange.Value.Start == this.SelectedRange.Value.End) {
            Rectangle rect;
            Vector2   position;
            if (this.Text.Length == 0) {
                position = Vector2.Zero;
            }
            else if (this.SelectedRange.Value.End == this.Text.Length) {
                rect     = rects[rects.Count - 1].Bounds;
                position = new Vector2(rect.Right, rect.Y);
            } 
            else {
                rect     = rects[this.SelectedRange.Value.Start].Bounds;
                position = new Vector2(rect.Left, rect.Bottom);
            }
            
            position.Y = this.Text.Take(this.SelectedRange.Value.Start).Count(c => c == '\n') * this.Font.LineHeight;

            if (this.SelectedRange.Value.Start != 0 && this.Text[this.SelectedRange.Value.Start - 1] == '\n')
                position.X = 0;
            
            // position.X -= 2;
        
            if (this.Text.EndsWith(" "))
                position.X += this.Font.MeasureString(" ").X;

            this._caret.MoveTo(position, instant ? 0 : 35);
        }
    }

    private void RegisterHandlers() {
        FurballGame.InputManager.OnMouseDown += this.OnMouseDown;
        FurballGame.InputManager.OnKeyDown   += this.OnKeyDown;
    }

    private void OnKeyDown(object sender, KeyEventArgs e) {
        if (!this.Selected || !this.Visible) return;

        if (this.SelectedRange.Value.Start > this.Text.Length)
            this.SelectedRange.Value = new Range(0, 0);
            
        switch(e.Key) {
            case Key.V when FurballGame.InputManager.ControlHeld: {
                string clipboard = e.Keyboard.GetClipboard();
            
                this.Text                = this.Text.Insert(this.SelectedRange.Value.End, clipboard);
                this.SelectedRange.Value = new Range(this.SelectedRange.Value.End + clipboard.Length, this.SelectedRange.Value.End + clipboard.Length);
                this.UpdateCaretPosition(false);
            
                this.OnLetterTyped?.Invoke(this, 'v');
                this.RecalcOutline();
                break;
            }
            case Key.Backspace: {
                if (this.Text.Length != 0 && this.SelectedRange.Value.Start != 0) {
                    char lastLetter = this.Text[this.SelectedRange.Value.Start - 1];
                        
                    this.Text = this.Text.Remove(this.SelectedRange.Value.Start - 1, 1);
                    this.OnLetterRemoved?.Invoke(this, lastLetter);

                    this.SelectedRange.Value = new Range(this.SelectedRange.Value.End - 1, this.SelectedRange.Value.End - 1);
                    this.UpdateCaretPosition(false);
                }
                this.RecalcOutline();
                break;
            }
            case Key.Enter: {
                if (this.LineCount == 1 || (this.LineCount > 1 && e.Keyboard.IsKeyPressed(Key.ShiftLeft))) {
                    this.OnCommit?.Invoke(this, this.Text);
                
                    if (this.DeselectOnCommit)
                        FurballGame.InputManager.ReleaseTextFocus(this);
                
                    if (this.ClearOnCommit) {
                        this.Text = string.Empty;
                        this.RecalcOutline();
                    }
                } else {
                    this.HandleChar(new CharInputEvent('\n', e.Keyboard));
                }
                break;
            }
            case Key.Left: {
                if (this.SelectedRange.Value.End == 0) {
                    this.SelectedRange.Value = new Range(0, 0);
                    break;
                }
                    
                this.SelectedRange.Value = new Range(this.SelectedRange.Value.End - 1, this.SelectedRange.Value.End - 1);
                this.UpdateCaretPosition(false);
                break;
            }
            case Key.Right: {
                if (this.SelectedRange.Value.End == this.Text.Length) {
                    this.SelectedRange.Value = new Range(this.Text.Length, this.Text.Length);
                    this.UpdateCaretPosition(false);
                    break;
                }
                    
                this.SelectedRange.Value = new Range(this.SelectedRange.Value.End + 1, this.SelectedRange.Value.End + 1);
                this.UpdateCaretPosition(false);
                break;
            }
        }
    }

    public void OnMouseDown(object sender, MouseButtonEventArgs e) {
        if (this.RealContains(e.Mouse.Position) && this.Visible && this.Clickable) {
            FurballGame.InputManager.TakeTextFocus(this);

            this.Text ??= "";
                
            if (this.Text.Length == 0) {
                this.SelectedRange.Value = new Range(0, 0);
                this.UpdateCaretPosition(true);
                return;
            }
            
            //Get the rectangles of all characters
            List<Glyph> glyphs = this.TextRectangles;

            List<int> newLinePoints = new();
            for (int i = 0; i < this.Text.Length; i++) {
                if(this.Text[i] == '\n')
                    newLinePoints.Add(i);
            }
            //Fake point at end of text
            newLinePoints.Add(this.Text.Length - 1);

            //Get the real number of used lines
            int numberOfLines = this.Text.Count(x => x == '\n') + 1;

            //Iterate over the number of used lines
            for (int lineNumber = numberOfLines - 1; lineNumber >= 0; lineNumber--) {
                Rectangle lastRect = lineNumber == numberOfLines - 1 ? glyphs.Last().Bounds : glyphs[newLinePoints[lineNumber] - 1].Bounds;
                
                int startOfLine = lineNumber == 0 ? 0 : newLinePoints[lineNumber - 1];
                int endOfLine   = newLinePoints[lineNumber];

                float yStart = this.Font.LineHeight * lineNumber;
                float yEnd   = this.Font.LineHeight * (lineNumber + 1);

                if (e.Mouse.Position.Y < yStart + this.RealPosition.Y || e.Mouse.Position.Y > yEnd + this.RealPosition.Y)
                    continue;
                
                if (e.Mouse.Position.X > lastRect.Right + this.RealPosition.X) {
                    this.SelectedRange.Value = new Range(endOfLine + 1, endOfLine + 1);
                    this.UpdateCaretPosition(true);
                    return;
                }

                for (int i = endOfLine; i >= startOfLine; i--) {
                    Rectangle rect = glyphs[i].Bounds;
                    if (e.Mouse.Position.X > rect.Left - rect.Width / 2f + this.RealPosition.X) {
                        this.SelectedRange.Value = new Range(i, i);
                        this.UpdateCaretPosition(true);
                        return;
                    }
                }
            }
        }
        else {
            FurballGame.InputManager.ReleaseTextFocus(this);
        }
    }

    private void UnregisterHandlers() {
        FurballGame.InputManager.OnMouseDown -= this.OnMouseDown;
        FurballGame.InputManager.OnKeyDown   -= this.OnKeyDown;
        
        FurballGame.InputManager.ReleaseTextFocus(this);
    }

    public override void Dispose() {
        this.UnregisterHandlers();

        this.OnLetterTyped   = null;
        this.OnLetterRemoved = null;
            
        base.Dispose();
    }

    public bool SaveInStack {
        get;
        set;
    } = false;
    
    public void HandleChar(CharInputEvent ev) {
        if (this.SelectedRange.Value.Start > this.Text.Length)
            this.SelectedRange.Value = new Range(0, 0);
            
        //If it was a control character, dont concat the character to the typed string
        if (char.IsControl(ev.Char) && ev.Char != '\n') return;

        this.Text = this.Text.Insert(this.SelectedRange.Value.Start, ev.Char.ToString());
        this.OnLetterTyped?.Invoke(this, ev.Char);
            
        this.SelectedRange.Value = new Range(this.SelectedRange.Value.End + 1, this.SelectedRange.Value.End + 1);
        this.UpdateCaretPosition(false);
        
        this.RecalcOutline();
    }

    private void RecalcOutline() {
        this._outline.RectSize = new Vector2(this.TextBoxWidth, this._textDrawable.Font.LineHeight * this._lineCount);
    }

    public void HandleFocus() {
        this.Selected = true;

        this._caret.FadeColor(new Color(1, 1, 1, 0.5f), 100);
        
        this._outline.FadeColor(Color.LightGray, 100);
    }
    
    public void HandleDefocus() {
        this.Selected = false;
        
        this._caret.FadeColor(new Color(1, 1, 1, 0f), 100);

        this._outline.FadeColor(Color.DarkGray, 100);
    }
}

public struct Range {
    public int Start;
    public int End;

    public int Length => Math.Abs(this.End - this.Start);

    public Range(int start, int end) {
        this.Start = start;
        this.End   = end;
    }
}