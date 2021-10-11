using System;
using FontStashSharp;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TextCopy;
using Xssp.MonoGame.Primitives2D;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    /// <summary>
    /// Creates a Basic Textbox
    /// </summary>
    public class UiTextBoxDrawable : TextDrawable {
        public float TextBoxWidth;
        public bool  Selected;

        public event EventHandler<char> OnLetterTyped;
        public event EventHandler<char> OnLetterRemoved; 
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
        public UiTextBoxDrawable(Vector2 position, FontSystem font, string text, int size, float width) : base(Vector2.Zero, font, text, size) {
            this.Position     = position;
            this.TextBoxWidth = width;
            this.RegisterHandlers();
        }

        private void RegisterHandlers() {
            FurballGame.Instance.Window.TextInput += this.OnTextInput;
            FurballGame.InputManager.OnMouseDown += this.OnMouseDown;
            FurballGame.InputManager.OnKeyDown += this.OnKeyDown;
        }

        private void OnKeyDown(object sender, Keys e) {
            if (!this.Selected) return;
            
            switch(e) {
                case Keys.V when FurballGame.InputManager.HeldKeys.Contains(Keys.LeftControl): {
                    string clipboard = ClipboardService.GetText();

                    this.Text += clipboard;
                    this.OnLetterTyped?.Invoke(this, 'v');
                    break;
                }
            }
        }

        private void OnMouseDown(object sender, ((MouseButton heldButton, Point position) args, string name) e) {
            Vector2   tempSize = this.Size;
            Rectangle sizeRect = new(new Point((int)this.Position.X, (int)this.Position.Y) - this.LastCalculatedOrigin.ToPoint(), new((int)tempSize.X, (int)tempSize.Y));
            
            if (sizeRect.Contains(e.args.position) && this.Visible && this.Clickable) {
                this.Selected = true;
            } else {
                this.Selected = false;
            }
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
                    break;
                }
            }

            if (wasSpecial) return;

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

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.SpriteBatch.DrawRectangle(
                args.Position * FurballGame.VerticalRatio, 
                this.Size * FurballGame.VerticalRatio, 
                this.Selected ? Color.LightGray : Color.DarkGray, 
                args.LayerDepth
            );
            
            base.Draw(time, batch, args);
        }
    }
}
