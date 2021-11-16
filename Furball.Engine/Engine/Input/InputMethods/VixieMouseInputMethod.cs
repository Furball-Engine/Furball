using Furball.Vixie.Input;
using Silk.NET.Input.Extensions;

namespace Furball.Engine.Engine.Input.InputMethods {
    public class VixieMouseInputMethod : InputMethod {
        public override void Update() {
            this.CursorPositions[0] = Mouse.GetState();
        }
        public override void Dispose() {

        }
        public override void Initialize() {
            this.CursorPositions.Add(Mouse.GetState());
        }
    }
}
