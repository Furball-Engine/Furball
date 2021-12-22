using System;
using System.Numerics;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Graphics.Drawables.UiElements;
using Furball.Engine.Engine.Input;
using Furball.Vixie.Input;
using JetBrains.Annotations;
using Silk.NET.Input;

namespace Furball.Engine.Engine.DevConsole {
    public class ConsoleDrawable : UiTextBoxDrawable {
        public event EventHandler<ConsoleResult> OnCommandFinished;
    
        public ConsoleDrawable() : base(new Vector2(FurballGame.DEFAULT_WINDOW_WIDTH / 2f, FurballGame.DEFAULT_WINDOW_HEIGHT / 2f), FurballGame.DEFAULT_FONT, "", 30, 300) {
            this.OriginType = OriginType.Center;
            this.Visible    = false;

            this.OnCommit                      += this.OnTextCommit;
            FurballGame.InputManager.OnKeyDown += this.OnKeyDown;
        }

        private void OnKeyDown(object? sender, Key e) {
            if (e == Key.F12) {
                this.Visible  = !this.Visible;
                this.Selected = !this.Selected;

                this.Text = "";
            }
        }

        private void OnTextCommit(object sender, string text) {
            ConsoleResult result = DevConsole.Run(text);

            this.OnCommandFinished?.Invoke(this, result);

            this.Visible  = false;
            this.Selected = false;
        }

        public override void Dispose() {
            this.OnCommit                      -= this.OnTextCommit;
            FurballGame.InputManager.OnKeyDown -= this.OnKeyDown;

            base.Dispose();
        }
    }
}
