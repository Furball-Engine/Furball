using System;
using System.Linq;
using SpriteFontPlus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Input;
using Microsoft.Xna.Framework.Input;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    public class UiTextBoxDrawable : TextDrawable {
        public float TextBoxWidth;
        public bool  Selected;

        public event EventHandler<char> OnLetterTyped;
        public event EventHandler<char> OnLetterRemoved; 

        public override Vector2 Size => new(this.TextBoxWidth, this.Font.MeasureString("|").Y);

        public UiTextBoxDrawable(byte[] font, string text, float size, float width, CharacterRange[] range = null) : base(font, text, size, range) {
            this.TextBoxWidth = width;
            this.RegisterHandlers();
        }
        public UiTextBoxDrawable(SpriteFont font, string text, float width) : base(font, text) {
            this.TextBoxWidth = width;
            this.RegisterHandlers();
        }

        private void RegisterHandlers() {
            FurballGame.Instance.Window.TextInput += this.OnTextInput;
            FurballGame.InputManager.OnMouseDown += this.OnMouseDown;
        }
        private void OnMouseDown(object? sender, (MouseButton, string) e) {
            Vector2   tempSize = this.Size;
            Rectangle sizeRect = new(new((int)this.Position.X, (int)this.Position.Y), new((int)tempSize.X, (int)tempSize.Y));

            Point mousePos = FurballGame.InputManager.CursorStates.Where(state => state.Name == e.Item2).First().State.Position;
            if (sizeRect.Contains(mousePos)) {
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
            
            base.Dispose(disposing);
        }

        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            batch.ShapeBatch.DrawRectangle(args.Position - args.Origin, this.Size, Color.Transparent, this.Selected ? Color.White : Color.Gray);
            
            base.Draw(time, batch, args);
        }
    }
}
