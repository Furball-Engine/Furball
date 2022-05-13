using System;
using System.Numerics;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Helpers;
using Furball.Vixie.Backends.Shared;
using Silk.NET.Input;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    /// <summary>
    /// Creates a Basic Textbox
    /// </summary>
    public class UiTextBoxDrawable : TextDrawable {
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
        

        public override Vector2 Size => new Vector2(this.TextBoxWidth, this.Font.MeasureString("|").Y) * this.Scale;
        /// <summary>
        /// Creates a Textbox
        /// </summary>
        /// <param name="position">Where to Draw</param>
        /// <param name="font">What font to use</param>
        /// <param name="text">Initial Text</param>
        /// <param name="size">Size of the text</param>
        /// <param name="width">Width/Length of the Textbox</param>
        public UiTextBoxDrawable(Vector2 position, FontSystem font, string text, int size, float width, bool isInContainerDrawable = false) : base(
            Vector2.Zero,
            font,
            text,
            size
        ) {
            this.Position     = position;
            this.TextBoxWidth = width;

            this._isInContainerDrawable = isInContainerDrawable;

            this.RegisterHandlers(this._isInContainerDrawable);
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

                    this.Text += clipboard;
                    this.OnLetterTyped?.Invoke(this, 'v');
                    break;
                }
                case Key.Backspace: {
                    if (this.Text.Length != 0) {
                        char lastLetter = this.Text[^1];
                        this.Text = this.Text[..^1];
                        this.OnLetterRemoved?.Invoke(this, lastLetter);
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
            }
        }

        public void OnMouseDown(object sender, ((MouseButton mouseButton, Vector2 position) args, string cursorName) e) {
            if (this.RealContains(e.args.position.ToPoint()) && this.Visible && this.Clickable)
                this.Selected = true;
            else
                this.Selected = false;
        }

        private void OnTextInput(object sender, (IKeyboard keyboard, char @char)e) {
            if (!this.Selected) return;

            //If it was a control character, dont concat the character to the typed string
            if (char.IsControl(e.@char)) return;

            this.Text += e.@char;
            this.OnLetterTyped?.Invoke(this, e.@char);
        }

        private void UnregisterHandlers() {
            FurballGame.InputManager.OnCharInput -= this.OnTextInput;
        }

        public override void Dispose() {
            this.UnregisterHandlers();

            this.OnLetterTyped = null;
            this.OnLetterRemoved = null;
            
            base.Dispose();
        }

        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.DrawRectangle(
                args.Position * FurballGame.VerticalRatio,
                this.Size * FurballGame.VerticalRatio,
                1f,
                this.Selected ? Color.LightGray : Color.DarkGray
             );
            
            base.Draw(time, batch, args);
        }
    }
}
