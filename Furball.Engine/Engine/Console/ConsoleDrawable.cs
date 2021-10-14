using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Helpers.Logger;
using Microsoft.Xna.Framework.Input;

namespace Furball.Engine.Engine.Console {
    public class ConsoleDrawable : UiTextBoxDrawable {
        public ConsoleDrawable() : base(new(FurballGame.DEFAULT_WINDOW_WIDTH / 2f, FurballGame.DEFAULT_WINDOW_HEIGHT / 2f), FurballGame.DEFAULT_FONT, "", 30, 300) {
            this.OriginType = OriginType.Center;
            this.Visible    = false;

            this.OnCommit                      += this.OnTextCommit;
            FurballGame.InputManager.OnKeyDown += this.OnKeyDown;
        }

        private void OnKeyDown(object sender, Keys e) {
            if (this.Visible) return;

            if (e == Keys.OemTilde) {
                this.Visible  = true;
                this.Selected = true;

                this.Text = "";
            }
        }

        private void OnTextCommit(object sender, string text) {
            Logger.Log(Console.Run(text));
            this.Visible = false;
        }

        public override void Dispose(bool disposing) {
            this.OnCommit                      -= this.OnTextCommit;
            FurballGame.InputManager.OnKeyDown -= this.OnKeyDown;

            base.Dispose(disposing);
        }
    }
}
