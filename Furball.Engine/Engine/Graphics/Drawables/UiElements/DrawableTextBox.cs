using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Silk.NET.Input;
using Color=Furball.Vixie.Backends.Shared.Color;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    /// <summary>
    /// Creates a Basic Textbox
    /// </summary>
    public class DrawableTextBox : TextDrawable {
        /// <summary>
        ///     The width of the text box
        /// </summary>
        public float TextBoxWidth;
        private bool _selected;
        private bool _isInContainerDrawable;
        /// <summary>
        ///     Whether the text box was selected
        /// </summary>
        public bool Selected {
            get => this._selected;
            set {
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
        public DrawableTextBox(Vector2 position, FontSystem font, int fontSize, float width, string text, bool isInContainerDrawable = false) : base(
            Vector2.Zero,
            font,
            text,
            fontSize
        ) {
            this.Position     = position;
            this.TextBoxWidth = width;

            this._isInContainerDrawable = isInContainerDrawable;

            this.RegisterHandlers(this._isInContainerDrawable);
            
            this.SelectedRange.OnChange += OnSelectedChange;
        }
        
        private void OnSelectedChange(object sender, Range e) {
            // throw new NotImplementedException();
        }

        private void RegisterHandlers(bool isInContainerDrawable) {
            FurballGame.InputManager.OnCharInput += this.OnTextInput;
            if (!isInContainerDrawable)
                FurballGame.InputManager.OnMouseDown += this.OnMouseDown;
            
            FurballGame.InputManager.OnKeyDown += this.OnKeyDown;
        }

        private void OnKeyDown(object sender, Key e) {
            if (!this.Selected || !this.Visible) return;
            
            switch(e) {
                case Key.V when FurballGame.InputManager.HeldKeys.Contains(Key.ControlLeft): {
                    string clipboard = FurballGame.InputManager.Clipboard;

                    this.Text                = this.Text.Insert(this.SelectedRange.Value.End.Value, clipboard);
                    this.SelectedRange.Value = new Range(this.SelectedRange.Value.End.Value + clipboard.Length, this.SelectedRange.Value.End.Value + clipboard.Length);
                    
                    this.OnLetterTyped?.Invoke(this, 'v');
                    break;
                }
                case Key.Backspace: {
                    if (this.Text.Length != 0 && this.SelectedRange.Value.Start.Value != 0) {
                        char lastLetter = this.Text[this.SelectedRange.Value.Start.Value - 1];
                        
                        this.Text = this.Text.Remove(this.SelectedRange.Value.Start.Value - 1, 1);
                        this.OnLetterRemoved?.Invoke(this, lastLetter);

                        this.SelectedRange.Value = new Range(this.SelectedRange.Value.End.Value - 1, this.SelectedRange.Value.End.Value - 1);
                    }
                    break;
                }
                case Key.Enter: {
                    this.OnCommit?.Invoke(this, this.Text);

                    if (this.DeselectOnCommit)
                        this.Selected = false;
                    //wasSpecial    = true;

                    if (this.ClearOnCommit)
                        this.Text = string.Empty;
                    break;
                }
                case Key.Left: {
                    if (this.SelectedRange.Value.End.Value == 0) {
                        this.SelectedRange.Value = new Range(0, 0);
                        break;
                    }
                    
                    this.SelectedRange.Value = new Range(this.SelectedRange.Value.End.Value - 1, this.SelectedRange.Value.End.Value - 1);
                    break;
                }
                case Key.Right: {
                    if (this.SelectedRange.Value.End.Value == this.Text.Length) {
                        this.SelectedRange.Value = new Range(this.Text.Length, this.Text.Length);
                        break;
                    }
                    
                    this.SelectedRange.Value = new Range(this.SelectedRange.Value.End.Value + 1, this.SelectedRange.Value.End.Value + 1);
                    break;
                }
            }
        }

        public void OnMouseDown(object sender, ((MouseButton mouseButton, Vector2 position) args, string cursorName) e) {
            if (this.RealContains(e.args.position.ToPoint()) && this.Visible && this.Clickable) {
                this.Selected = true;

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
                this.Selected = false;
            }
        }

        private void OnTextInput(object sender, (IKeyboard keyboard, char @char)e) {
            if (!this.Selected) return;

            //If it was a control character, dont concat the character to the typed string
            if (char.IsControl(e.@char)) return;

            this.Text = this.Text.Insert(this.SelectedRange.Value.Start.Value, e.@char.ToString());
            this.OnLetterTyped?.Invoke(this, e.@char);
            
            this.SelectedRange.Value = new Range(this.SelectedRange.Value.End.Value + 1, this.SelectedRange.Value.End.Value + 1);
        }

        private void UnregisterHandlers() {
            FurballGame.InputManager.OnCharInput -= this.OnTextInput;
        }

        public override void Dispose() {
            this.UnregisterHandlers();

            this.SelectedRange.OnChange -= OnSelectedChange;

            this.OnLetterTyped = null;
            this.OnLetterRemoved = null;
            
            base.Dispose();
        }

        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
            args.Position *= FurballGame.VerticalRatio;
            args.Scale    *= FurballGame.VerticalRatio;
            
            batch.DrawRectangle(
                args.Position,
                this.Size * args.Scale,
                1f,
                this.Selected ? Color.LightGray : Color.DarkGray
            );

            if(this.Selected) {
                List<Rectangle> rects = this.TextRectangles;
                
                if (this.SelectedRange.Value.Start.Value == this.SelectedRange.Value.End.Value) {
                    Rectangle rect;
                    Vector2 position;
                    if (this.Text.Length == 0) {
                        position = args.Position;
                    }
                    else if (this.SelectedRange.Value.End.Value == this.Text.Length) {
                        rect     = rects[^1];
                        position = args.Position + (new Vector2(rect.Right, rect.Y) * FurballGame.VerticalRatio);
                    } 
                    else {
                        rect = rects[this.SelectedRange.Value.Start];
                        position = args.Position + (new Vector2(rect.Left, rect.Y) * FurballGame.VerticalRatio);
                    }
                    position.Y =  args.Position.Y; //this will have to do for now :(
                    position.X -= 1;

                    if (this.Text.EndsWith(" "))
                        position.X += this.Font.MeasureString(" ").X;
                    
                    batch.Draw(FurballGame.WhitePixel, position, new Vector2(1.5f * FurballGame.VerticalRatio, this.Font.LineHeight * FurballGame.VerticalRatio));
                }
            }

            args.Position /= FurballGame.VerticalRatio;
            args.Scale    /= FurballGame.VerticalRatio;
            
            base.Draw(time, batch, args);
        }
    }
}
