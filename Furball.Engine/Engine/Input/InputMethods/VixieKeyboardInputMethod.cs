using Furball.Vixie.Input;
using Silk.NET.Input.Extensions;

namespace Furball.Engine.Engine.Input.InputMethods {
    public class VixieKeyboardInputMethod : InputMethod {
        public KeyboardState KeyboardState { get; private set; }
        public override void Update() {
            KeyboardState = Keyboard.GetState();
        }
        public override void Dispose() {

        }
        public override void Initialize() {

        }
    }
}
