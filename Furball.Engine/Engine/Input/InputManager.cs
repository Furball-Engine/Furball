using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Furball.Engine.Engine.Input {
	public enum MouseButton {
		LeftButton,
		MiddleButton,
		RightButton
	}

	public class InputManager {
		/// <summary>
		/// The positions of all cursors and their states
		/// </summary>
		public List<MouseState> CursorStates {
			get {
				List<MouseState> temp = new();

				for (var i = 0; i < this.registeredInputMethods.Count; i++)
					temp.AddRange(this.registeredInputMethods[i].CursorPositions);

				return temp;
			}
		}

		/// <summary>
		/// The currently held Keyboard keys
		/// </summary>
		public List<Keys> HeldKeys {
			get {
				List<Keys> temp = new();

				for (var i = 0; i < this.registeredInputMethods.Count; i++)
					temp.AddRange(this.registeredInputMethods[i].HeldKeys);

				return temp;
			}
		}

		/// <summary>
		/// The currently registered InputMethods
		/// </summary>
		private List<InputMethod> registeredInputMethods = new();

		/// <summary>
		/// Called when a key is pressed
		/// </summary>
		public event EventHandler<Keys>                       OnKeyDown;
		/// <summary>
		/// Called when a key is released
		/// </summary>
		public event EventHandler<Keys>                       OnKeyUp;
		/// <summary>
		/// Called when a mouse button is pressed
		/// </summary>
		public event EventHandler<Tuple<MouseButton, string>> OnMouseDown;
		/// <summary>
		/// Called when a mouse button is released
		/// </summary>
		public event EventHandler<Tuple<MouseButton, string>> OnMouseUp;
		/// <summary>
		/// Called when a cursor moves
		/// </summary>
		public event EventHandler<Tuple<Point, string>>       OnMouseMove;
		/// <summary>
		/// Called when the cursor scrolls
		/// </summary>
		public event EventHandler<Tuple<int, string>>         OnMouseScroll;

		/// <summary>
		/// Updates all registered InputMethods and calls the necessary events
		/// </summary>
		public void Update() {
			List<MouseState> oldCursorStates = this.CursorStates.ToList();
			List<Keys>       oldKeys         = this.HeldKeys.ToList();

			for (var i = 0; i < this.registeredInputMethods.Count; i++) {
				InputMethod method = this.registeredInputMethods[i];

				method.Update();
			}

			#region OnKeyUp/Down

			IEnumerable<Keys> diffKeysPressed  = this.HeldKeys.Except(oldKeys);
			IEnumerable<Keys> diffKeysReleased = oldKeys.Except(this.HeldKeys);

			foreach (Keys key in diffKeysPressed)
				this.OnKeyDown?.Invoke(this, key);

			foreach (Keys key in diffKeysReleased)
				this.OnKeyUp?.Invoke(this, key);

			#endregion

			#region OnMouseUp/Down/Move/Scroll

			foreach (MouseState oldState in oldCursorStates) {
				foreach (MouseState newState in this.CursorStates.Where(newState => oldState.Name == newState.Name)) {
					if (oldState.State.Position != newState.State.Position)
						this.OnMouseMove?.Invoke(this, new(newState.State.Position, newState.Name));

					if (oldState.State.LeftButton != newState.State.LeftButton)
						if (oldState.State.LeftButton == ButtonState.Released)
							this.OnMouseDown?.Invoke(this, new Tuple<MouseButton, string>(MouseButton.LeftButton, newState.Name));
						else
							this.OnMouseUp?.Invoke(this, new Tuple<MouseButton, string>(MouseButton.LeftButton, newState.Name));
					if (oldState.State.RightButton != newState.State.RightButton)
						if (oldState.State.RightButton == ButtonState.Released)
							this.OnMouseDown?.Invoke(this, new Tuple<MouseButton, string>(MouseButton.RightButton, newState.Name));
						else
							this.OnMouseUp?.Invoke(this, new Tuple<MouseButton, string>(MouseButton.RightButton, newState.Name));
					if (oldState.State.MiddleButton != newState.State.MiddleButton)
						if (oldState.State.MiddleButton == ButtonState.Released)
							this.OnMouseDown?.Invoke(this, new Tuple<MouseButton, string>(MouseButton.MiddleButton, newState.Name));
						else
							this.OnMouseUp?.Invoke(this, new Tuple<MouseButton, string>(MouseButton.MiddleButton, newState.Name));
					if (oldState.State.ScrollWheelValue != newState.State.ScrollWheelValue)
						this.OnMouseScroll?.Invoke(this, new Tuple<int, string>(newState.State.ScrollWheelValue - oldState.State.ScrollWheelValue, newState.Name));
				}
			}

			#endregion
		}

		/// <summary>
		/// Registers a new input method and calls its Initialize method
		/// </summary>
		/// <param name="method">The InputMethod to add</param>
		public void RegisterInputMethod(InputMethod method) {
			this.registeredInputMethods.Add(method);
			method.Initialize();
		}
		/// <summary>
		/// Removes an input method and calls its Dispose method
		/// </summary>
		/// <param name="method">The InputMethod to remove</param>
		public void RemoveInputMethod(InputMethod method) {
			method.Dispose();
			this.registeredInputMethods.Remove(method);
		}
	}
}
