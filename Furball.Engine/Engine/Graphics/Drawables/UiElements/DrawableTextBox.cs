using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Graphics.Drawables.Primitives;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Input;
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

    public DynamicSpriteFont Font => this._textDrawable.Font;
    
    public List<Rectangle> TextRectangles => this._textDrawable.TextRectangles;

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

    public override Vector2 Size => new Vector2(this.TextBoxWidth, this._textDrawable.Font.LineHeight) * this.Scale;
    /// <summary>
    /// Creates a Textbox
    /// </summary>
    /// <param name="position">Where to Draw</param>
    /// <param name="font">What font to use</param>
    /// <param name="fontSize">Size of the text</param>
    /// <param name="width">Width/Length of the Textbox</param>
    /// <param name="text">Initial Text</param>
    public DrawableTextBox(Vector2 position, FontSystem font, int fontSize, float width, string text) {
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
            CoverHovers   = false
        };

        this.Drawables.Add(this._textDrawable);
        this.Drawables.Add(this._outline);
        this.Drawables.Add(this._caret);

        this.RegisterHandlers();
        this.RecalcOutline();
    }
        
    private void UpdateCaretPosition(bool instant) {
        List<Rectangle> rects = this.TextRectangles;
            
        if (this.SelectedRange.Value.Start == this.SelectedRange.Value.End) {
            Rectangle rect;
            Vector2   position;
            if (this.Text.Length == 0) {
                position = Vector2.Zero;
            }
            else if (this.SelectedRange.Value.End == this.Text.Length) {
                rect     = rects[rects.Count - 1];
                position = new Vector2(rect.Right, rect.Y);
            } 
            else {
                rect     = rects[this.SelectedRange.Value.Start];
                position = new Vector2(rect.Left, rect.Y);
            }
            position.Y =  0; //this will have to do for now :(
            position.X -= 1;
        
            if (this.Text.EndsWith(" "))
                position.X += this.Font.MeasureString(" ").X;

            this._caret.MoveTo(position, instant ? 0 : 35);
        }
    }

    private void RegisterHandlers() {
        FurballGame.InputManager.OnMouseDown += this.OnMouseDown;
        FurballGame.InputManager.OnKeyDown   += this.OnKeyDown;
    }

    private void OnKeyDown(object sender, Key e) {
        if (!this.Selected || !this.Visible) return;
            
        switch(e) {
            case Key.V when FurballGame.InputManager.HeldKeys.Contains(Key.ControlLeft): {
                string clipboard = FurballGame.InputManager.Clipboard;

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
                this.OnCommit?.Invoke(this, this.Text);

                if (this.DeselectOnCommit)
                    FurballGame.InputManager.ReleaseTextFocus(this);
                //wasSpecial    = true;

                if (this.ClearOnCommit) {
                    this.Text = string.Empty;
                    this.RecalcOutline();
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

    public void OnMouseDown(object sender, ((MouseButton mouseButton, Vector2 position) args, string cursorName) e) {
        if (this.RealContains(e.args.position.ToPoint()) && this.Visible && this.Clickable) {
            FurballGame.InputManager.TakeTextFocus(this);

            List<Rectangle> rects = this.TextRectangles;
            

            if (rects.Count == 0) {
                this.SelectedRange.Value = new Range(0, 0);
                this.UpdateCaretPosition(true);
                return;
            }
                
            if (e.args.position.X > rects.Last().Right + this.RealPosition.X) {
                this.SelectedRange.Value = new Range(this.Text.Length, this.Text.Length);
                this.UpdateCaretPosition(true);
                return;
            }

            for (int i = rects.Count - 1; i >= 0; i--) {
                Rectangle rect = rects[i];
                if (e.args.position.X > ((rect.Left - (rect.Width / 2f)) + this.RealPosition.X)) {
                    this.SelectedRange.Value = new Range(i, i);
                    this.UpdateCaretPosition(true);
                    return;
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

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        // batch.DrawRectangle(
        // args.Position,
        // this.Size * args.Scale,
        // 1f,
        // this.Selected ? Color.LightGray : Color.DarkGray
        // );

        // if(this.Selected) {

        // }

        base.Draw(time, batch, args);
    }

    public bool SaveInStack {
        get;
        set;
    } = false;
    
    public void HandleChar(char c) {
        if (this.SelectedRange.Value.Start > this.Text.Length)
            this.SelectedRange.Value = new Range(0, 0);
            
        //If it was a control character, dont concat the character to the typed string
        if (char.IsControl(c)) return;

        this.Text = this.Text.Insert(this.SelectedRange.Value.Start, c.ToString());
        this.OnLetterTyped?.Invoke(this, c);
            
        this.SelectedRange.Value = new Range(this.SelectedRange.Value.End + 1, this.SelectedRange.Value.End + 1);
        this.UpdateCaretPosition(false);
        
        this.RecalcOutline();
    }

    private void RecalcOutline() {
        this._outline.RectSize = new Vector2(this.TextBoxWidth, this._textDrawable.Font.LineHeight);
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