using System.Linq;
using Furball.Vixie.Input;
using Silk.NET.Input;
using Silk.NET.Input.Extensions;

namespace Furball.Engine.Engine.Input.InputMethods {
    public class VixieMouseInputMethod : InputMethod {
        public override void Update() {
            for (int i = 0; i < this.MouseStates.Count; i++) {
                FurballMouseState state = this.MouseStates[i];
                IMouse            mouse = this.Mice[i];
                
                MouseState tempState = mouse.CaptureState();

                state.Position       = tempState.Position / FurballGame.VerticalRatio;
                state.PressedButtons = tempState.GetPressedButtons().ToArray();
                state.ScrollWheels   = tempState.GetScrollWheels().ToArray();

                this.MouseStates[i] = state;
            }
        }
        
        public override void Dispose() {
            this.Mice.Clear();
        }
        
        public override void Initialize() {
            this.Mice = Mouse.GetMice().ToList();

            for (int i = 0; i < this.Mice.Count; i++) {
                IMouse mouse = this.Mice[i];
                
                this.MouseStates.Add(GetState(mouse));
            }
        }

        public static FurballMouseState GetState(IMouse mouse) {
            FurballMouseState state = new();

            MouseState tempState = mouse.CaptureState();

            state.Position       = tempState.Position / FurballGame.VerticalRatio;
            state.PressedButtons = tempState.GetPressedButtons().ToArray();
            state.ScrollWheels   = tempState.GetScrollWheels().ToArray();
            
            state.Name = mouse.Name;
            
            return state;
        }
    }
}
