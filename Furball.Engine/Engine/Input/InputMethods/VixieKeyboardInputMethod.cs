using System.Linq;
using Furball.Vixie.Input;
using Silk.NET.Input.Extensions;

namespace Furball.Engine.Engine.Input.InputMethods {
    public class VixieKeyboardInputMethod : InputMethod {
        private KeyboardState KeyboardState;
        public override void Update() {
            this.HeldKeys = this.KeyboardState.GetPressedKeys().ToArray().ToList();
        }
        public override void Dispose() {

        }
        public override void Initialize() {
            this.KeyboardState = Keyboard.GetState();
        }
    }
}
