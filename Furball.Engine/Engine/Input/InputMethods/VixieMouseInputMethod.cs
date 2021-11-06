using Furball.Vixie.Input;
using Silk.NET.Input.Extensions;

namespace Furball.Engine.Engine.Input.InputMethods {
    public class VixieMouseInputMethod : InputMethod {
        public MouseState CurrentState { get; private set; }
        public override void Update() {
            this.CurrentState = Mouse.GetState();
        }
        public override void Dispose() {

        }
        public override void Initialize() {

        }
    }
}
