using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Furball.Engine.Engine.Input;
using Silk.NET.Input;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements; 

/// <summary>
/// Creates a Basic Textbox
/// </summary>
public class DrawableTextBox : TextDrawable, ICharInputHandler {
    /// <summary>
    ///     The width of the text box
    /// </summary>
    public float TextBoxWidth;
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

    /// <summary>
    ///     Called when a letter is typed in the text box
    /// </summary>
    public event EventHandler<char> OnLetterTyped;
    /// <summary>
    ///     Called when a letter is removed from the text box
    /// </summary>
    public event EventHandler<char> OnLetterRemoved;
    /// <summary>
    ///     Called when the user "commits" the text in the text box, aka when they press enter
    /// </summary>
    public event EventHandler<string> OnCommit;
    /// <summary>
    ///     Called when focus changes on the textbox
    /// </summary>
    public event EventHandler<bool> OnFocusChange;
    /// <summary>
    /// The range of characters in the string that are selected
    /// </summary>
    public readonly Bindable<Range> SelectedRange = new(new Range(0, 0));


    public override Vector2 Size => new Vector2(this.TextBoxWidth, this.Font.MeasureString("|").Y) * this.Scale;
    /// <summary>
    /// Creates a Textbox
    /// </summary>
    /// <param name="position">Where to Draw</param>
    /// <param name="font">What font to use</param>
    /// <param name="fontSize">Size of the text</param>
    /// <param name="width">Width/Length of the Textbox</param>
    /// <param name="text">Initial Text</param>
    /// <param name="isInContainerDrawable"></param>
    public DrawableTextBox(Vector2 position, FontSystem font, int fontSize, float width, string text) : base(
    Vector2.Zero,
    font,
    text,
    fontSize
    ) {
        this.Position     = position;
        this.TextBoxWidth = width;

        this.RegisterHandlers();
            
        this.SelectedRange.OnChange += OnSelectedChange;
    }
        
    private void OnSelectedChange(object sender, Range e) {
        // throw new NotImplementedException();
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
                    
                this.OnLetterTyped?.Invoke(this, 'v');
                break;
            }
            case Key.Backspace: {
                if (this.Text.Length != 0 && this.SelectedRange.Value.Start != 0) {
                    char lastLetter = this.Text[this.SelectedRange.Value.Start - 1];
                        
                    this.Text = this.Text.Remove(this.SelectedRange.Value.Start - 1, 1);
                    this.OnLetterRemoved?.Invoke(this, lastLetter);

                    this.SelectedRange.Value = new Range(this.SelectedRange.Value.End - 1, this.SelectedRange.Value.End - 1);
                }
                break;
            }
            case Key.Enter: {
                this.OnCommit?.Invoke(this, this.Text);

                if (this.DeselectOnCommit)
                    FurballGame.InputManager.ReleaseTextFocus(this);
                //wasSpecial    = true;

                if (this.ClearOnCommit)
                    this.Text = string.Empty;
                break;
            }
            case Key.Left: {
                if (this.SelectedRange.Value.End == 0) {
                    this.SelectedRange.Value = new Range(0, 0);
                    break;
                }
                    
                this.SelectedRange.Value = new Range(this.SelectedRange.Value.End - 1, this.SelectedRange.Value.End - 1);
                break;
            }
            case Key.Right: {
                if (this.SelectedRange.Value.End == this.Text.Length) {
                    this.SelectedRange.Value = new Range(this.Text.Length, this.Text.Length);
                    break;
                }
                    
                this.SelectedRange.Value = new Range(this.SelectedRange.Value.End + 1, this.SelectedRange.Value.End + 1);
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
                return;
            }
                
            if (e.args.position.X > rects.Last().Right + this.RealPosition.X) {
                this.SelectedRange.Value = new Range(this.Text.Length, this.Text.Length);
                return;
            }

            for (int i = rects.Count - 1; i >= 0; i--) {
                Rectangle rect = rects[i];
                if (e.args.position.X > ((rect.Left - (rect.Width / 2f)) + this.RealPosition.X)) {
                    this.SelectedRange.Value = new Range(i, i);
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
    }

    public override void Dispose() {
        this.UnregisterHandlers();

        this.SelectedRange.OnChange -= OnSelectedChange;

        this.OnLetterTyped   = null;
        this.OnLetterRemoved = null;
            
        base.Dispose();
    }

    public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
        batch.DrawRectangle(
        args.Position,
        this.Size * args.Scale,
        1f,
        this.Selected ? Color.LightGray : Color.DarkGray
        );

        if(this.Selected) {
            List<Rectangle> rects = this.TextRectangles;
                
            if (this.SelectedRange.Value.Start == this.SelectedRange.Value.End) {
                Rectangle rect;
                Vector2   position;
                if (this.Text.Length == 0) {
                    position = args.Position;
                }
                else if (this.SelectedRange.Value.End == this.Text.Length) {
                    rect     = rects[rects.Count - 1];
                    position = args.Position + new Vector2(rect.Right, rect.Y);
                } 
                else {
                    rect     = rects[this.SelectedRange.Value.Start];
                    position = args.Position + new Vector2(rect.Left, rect.Y);
                }
                position.Y =  args.Position.Y;//this will have to do for now :(
                position.X -= 1;

                if (this.Text.EndsWith(" "))
                    position.X += this.Font.MeasureString(" ").X;

                batch.Draw(FurballGame.WhitePixel, position, new Vector2(1.5f, this.Font.LineHeight));
            }
        }

        base.Draw(time, batch, args);
    }
    
    public void HandleChar(char c) {
        if (this.SelectedRange.Value.Start > this.Text.Length)
            this.SelectedRange.Value = new Range(0, 0);
            
        //If it was a control character, dont concat the character to the typed string
        if (char.IsControl(c)) return;

        this.Text = this.Text.Insert(this.SelectedRange.Value.Start, c.ToString());
        this.OnLetterTyped?.Invoke(this, c);
            
        this.SelectedRange.Value = new Range(this.SelectedRange.Value.End + 1, this.SelectedRange.Value.End + 1);
    }
    
    public void HandleFocus() {
        this.Selected = true;
    }
    
    public void HandleDefocus() {
        this.Selected = false;
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