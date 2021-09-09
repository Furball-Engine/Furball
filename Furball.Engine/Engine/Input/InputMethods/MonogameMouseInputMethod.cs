using Microsoft.Xna.Framework.Input;
using MouseStateXna=Microsoft.Xna.Framework.Input.MouseState;

namespace Furball.Engine.Engine.Input.InputMethods {
	public class MonogameMouseInputMethod : InputMethod {
		public override void Update() {
			MouseStateXna currentMouseState = Mouse.GetState(FurballGame.Instance.Window);
			this.CursorPositions[0].State = currentMouseState;
		}
		public override void Dispose() { }
		public override void Initialize() {
			MouseStateXna currentMouseState = Mouse.GetState(FurballGame.Instance.Window);
			this.CursorPositions.Add(new MouseState(currentMouseState, "MonogameMouse"));
		}
	}
}
