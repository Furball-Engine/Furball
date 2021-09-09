using Microsoft.Xna.Framework.Input;

namespace Furball.Engine.Engine.Input.InputMethods {
	public class MonogameMouseInputMethod : InputMethod {
		public override void Update() {
			this.CursorPositions[0] = new(Mouse.GetState(FurballGame.Instance.Window), "MonogameMouse");
		}
		public override void Dispose() { }
		public override void Initialize() {
			this.CursorPositions.Add(new(Mouse.GetState(FurballGame.Instance.Window), "MonogameMouse"));
		}
	}
}
