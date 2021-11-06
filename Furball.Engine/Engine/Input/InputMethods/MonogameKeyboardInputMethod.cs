using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

namespace Furball.Engine.Engine.Input.InputMethods {
    public class MonogameKeyboardInputMethod : InputMethod {
        private List<Keys> tempList = new();

        public override void Update() {
            this.HeldKeys = this.tempList.ToList();
        }

        public override void Dispose() {
            FurballGame.Instance.Window.KeyDown -= this.OnKeyDown;
            FurballGame.Instance.Window.KeyUp   -= this.OnKeyUp;
        }

        public override void Initialize() {
            //The reason we dont just use Keyboard.GetState() is because that does not retain the order of the keys being pressed

            FurballGame.Instance.Window.KeyDown += this.OnKeyDown;
            FurballGame.Instance.Window.KeyUp   += this.OnKeyUp;
        }

        private void OnKeyDown(object sender, InputKeyEventArgs args) {
            //OnKeyDown will repeat like a text box if a key is held so we make sure we arent adding duplicates

            if (!this.tempList.Contains(args.Key))
                this.tempList.Add(args.Key);
        }

        private void OnKeyUp(object sender, InputKeyEventArgs args) {
            this.tempList.Remove(args.Key);
        }
    }
}
