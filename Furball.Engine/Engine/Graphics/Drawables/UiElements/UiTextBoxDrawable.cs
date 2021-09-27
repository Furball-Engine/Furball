using System;
using System.Linq;
using SpriteFontPlus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Input;
using Microsoft.Xna.Framework.Input;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    /// <summary>
    /// Creates a Basic Textbox
    /// </summary>
    public class UiTextBoxDrawable : TextDrawable {
        public float TextBoxWidth;
        public bool  Selected;

        public event EventHandler<char> OnLetterTyped;
        public event EventHandler<char> OnLetterRemoved; 

        public override Vector2 Size => new Vector2(this.TextBoxWidth, this.Font.MeasureString("|").Y) * this.Scale;
        /// <summary>
        /// Creates a Textbox
        /// </summary>
        /// <param name="position">Where to Draw</param>
        /// <param name="font">What font to use</param>
        /// <param name="text">Initial Text</param>
        /// <param name="size">Size of the text</param>
        /// <param name="width">Width/Length of the Textbox</param>
        /// <param name="range">SpriteFont characcter range</param>
        public UiTextBoxDrawable(Vector2 position, byte[] font, string text, float size, float width, CharacterRange[] range = null) : base(Vector2.Zero, font, text, size, range) {
            this.Position     = position;
            this.TextBoxWidth = width;
            this.RegisterHandlers();
        }
        /// <summary>
        /// Creates a Textbox
        /// </summary>
        /// <param name="position">Where to Draw</param>
        /// <param name="font">What font to use</param>
        /// <param name="text">Initial Text</param>>
        /// <param name="width">Width/Length of the Textbox</param>
        public UiTextBoxDrawable(Vector2 position, SpriteFont font, string text, float width) : base(font, text) {
            this.Position     = position;
            this.TextBoxWidth = width;
            this.RegisterHandlers();
        }

        private void RegisterHandlers() {
            FurballGame.Instance.Window.TextInput += this.OnTextInput;
            FurballGame.InputManager.OnMouseDown += this.OnMouseDown;
        }

        private void OnMouseDown(object? sender, (MouseButton, string) e) {
            Vector2   tempSize = this.Size;
            Rectangle sizeRect = new(new Point((int)this.Position.X, (int)this.Position.Y) - this.LastCalculatedOrigin.ToPoint(), new((int)tempSize.X, (int)tempSize.Y));

            Point mousePos = FurballGame.InputManager.CursorStates.First(state => state.Name == e.Item2).Position;
            if (sizeRect.Contains(mousePos) && this.Visible && this.Clickable) {
                this.Selected = true;
            } else {
                this.Selected = false;
            }
        }

        private void OnTextInput(object sender, TextInputEventArgs e) {
            if (!this.Selected) return;
            
            bool wasSpecial = false;

            switch (e.Key) {
                case Keys.Back:
                    if (this.Text.Length != 0) {
                        this.OnLetterRemoved?.Invoke(this, this.Text[^1]);
                        this.Text = this.Text[..^1];
                    }
                    wasSpecial = true;
                    break;
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
            batch.ShapeBatch.DrawRectangle(
                args.Position * FurballGame.VerticalRatio, 
                this.Size * FurballGame.VerticalRatio, 
                Color.Transparent, 
                this.Selected ? Color.White : Color.Gray
            );
            
            base.Draw(time, batch, args);
        }
    }
}
