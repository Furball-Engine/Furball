using System;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Input;
using TextCopy;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    /// <summary>
    /// Creates a Basic Textbox
    /// </summary>
    public class UiTextBoxDrawable : TextDrawable {
        /// <summary>
        ///     The width of the text box
        /// </summary>
        public float TextBoxWidth;
        /// <summary>
        ///     Whether the text box was selected
        /// </summary>
        public bool  Selected;
        public bool ClearOnCommit = false;

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
            this.RegisterHandlers(isInContainerDrawable);
        }

        private void RegisterHandlers(bool isInContainerDrawable) {
            FurballGame.Instance.Window.TextInput += this.OnTextInput;
            if (!isInContainerDrawable)
                FurballGame.InputManager.OnMouseDown += this.OnMouseDown;
            FurballGame.InputManager.OnKeyDown += this.OnKeyDown;
        }

        private void OnKeyDown(object sender, Keys e) {
            if (!this.Selected && !this.Visible) return;
            
            switch(e) {
                case Keys.V when FurballGame.InputManager.HeldKeys.Contains(Keys.LeftControl): {
                    string clipboard = ClipboardService.GetText();

                    this.Text += clipboard;
                    this.OnLetterTyped?.Invoke(this, 'v');
                    break;
                }
            }
        }

        public void OnMouseDown(object sender, ((MouseButton heldButton, Point position) args, string name) e) {
            if (this.Rectangle.Contains(e.args.position) && this.Visible && this.Clickable)
                this.Selected = true;
            else
                this.Selected = false;
        }

        private void OnTextInput(object sender, TextInputEventArgs e) {
            if (!this.Selected) return;
            
            bool wasSpecial = false;

            switch (e.Key) {
                case Keys.Back: {
                    if (this.Text.Length != 0) {
                        char lastLetter = this.Text[^1];
                        this.Text = this.Text[..^1];
                        this.OnLetterRemoved?.Invoke(this, lastLetter);
                    }
                    wasSpecial = true;
                    break;
                }
                case Keys.Enter: {
                    this.OnCommit?.Invoke(this, this.Text);
                    this.Selected = false;
                    wasSpecial    = true;

                    if (this.ClearOnCommit)
                        this.Text = string.Empty;
                    break;
                }
            }

            //If it was a special character or the character ia control character, dont concat the character to the typed string
            if (wasSpecial || char.IsControl(e.Character)) return;

            this.Text += e.Character;
            this.OnLetterTyped?.Invoke(this, e.Character);
        }

        private void UnregisterHandlers() {
            FurballGame.Instance.Window.TextInput -= this.OnTextInput;
        }

        public override void Dispose(bool disposing) {
            this.UnregisterHandlers();

            this.OnLetterTyped = null;
            this.OnLetterRemoved = null;
            
            base.Dispose(disposing);
        }

        public override void Draw(double time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.SpriteBatch.DrawRectangle(
                args.Position * FurballGame.VerticalRatio, 
                this.Size * FurballGame.VerticalRatio, 
                this.Selected ? Color.LightGray : Color.DarkGray, 
                0f
            );
            
            base.Draw(time, batch, args);
        }
    }
}
