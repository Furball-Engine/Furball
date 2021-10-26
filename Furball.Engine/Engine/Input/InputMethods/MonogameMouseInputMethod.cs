using Microsoft.Xna.Framework.Input;
using MouseStateXna=Microsoft.Xna.Framework.Input.MouseState;

namespace Furball.Engine.Engine.Input.InputMethods {
    public class MonogameMouseInputMethod : InputMethod {
        public override void Update() {
            MouseStateXna currentMouseState = Mouse.GetState(FurballGame.Instance.Window);
            
            MouseState state = new("MonogameMouse") {
                Position                   = (currentMouseState.Position.ToVector2() / FurballGame.VerticalRatio).ToPoint(),
                LeftButton                 = currentMouseState.LeftButton,
                MiddleButton               = currentMouseState.MiddleButton,
                RightButton                = currentMouseState.RightButton,
                XButton1                   = currentMouseState.XButton1,
                XButton2                   = currentMouseState.XButton2,
                ScrollWheelValue           = currentMouseState.ScrollWheelValue,
                HorizontalScrollWheelValue = currentMouseState.HorizontalScrollWheelValue
            };

            if (!FurballGame.Instance.IsActive) {
                state.LeftButton   = ButtonState.Released;
                state.MiddleButton = ButtonState.Released;
                state.RightButton  = ButtonState.Released;
                state.XButton1     = ButtonState.Released;
                state.XButton2     = ButtonState.Released;
            }

            this.CursorPositions[0] = state;
        }
        public override void Dispose() {}
        public override void Initialize() {
            MouseStateXna currentMouseState = Mouse.GetState(FurballGame.Instance.Window);
            
            MouseState state = new("MonogameMouse") {
                Position                   = (currentMouseState.Position.ToVector2() / FurballGame.VerticalRatio).ToPoint(),
                LeftButton                 = currentMouseState.LeftButton,
                MiddleButton               = currentMouseState.MiddleButton,
                RightButton                = currentMouseState.RightButton,
                XButton1                   = currentMouseState.XButton1,
                XButton2                   = currentMouseState.XButton2,
                ScrollWheelValue           = currentMouseState.ScrollWheelValue,
                HorizontalScrollWheelValue = currentMouseState.HorizontalScrollWheelValue
            };

            if (!FurballGame.Instance.IsActive) {
                state.LeftButton   = ButtonState.Released;
                state.MiddleButton = ButtonState.Released;
                state.RightButton  = ButtonState.Released;
                state.XButton1     = ButtonState.Released;
                state.XButton2     = ButtonState.Released;
            }

            this.CursorPositions.Add(state);
        }
    }
}
