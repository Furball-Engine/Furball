using System.Linq;
using Furball.Vixie.Input;
using Silk.NET.Input;
using Silk.NET.Input.Extensions;

namespace Furball.Engine.Engine.Input.InputMethods {
    public class VixieMouseInputMethod : InputMethod {
        public override void Update() {
            this.MouseStates.Clear();

            foreach (IMouse mouse in this.Mice) {
                this.MouseStates.Add(mouse.CaptureState());
            }
        }
        
        public override void Dispose() {
            this.Mice.Clear();
        }
        
        public override void Initialize() {
            this.Mice = Mouse.GetMice().ToList();
        }
    }
}
