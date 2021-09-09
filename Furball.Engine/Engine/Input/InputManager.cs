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

				for (int i = 0; i < this.registeredInputMethods.Count; i++)
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

				for (int i = 0; i < this.registeredInputMethods.Count; i++)
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
		public event EventHandler<(MouseButton, string)> OnMouseDown;
		/// <summary>
		/// Called when a mouse button is released
		/// </summary>
		public event EventHandler<(MouseButton, string)> OnMouseUp;
		/// <summary>
		/// Called when a cursor moves
		/// </summary>
		public event EventHandler<(Point, string)>       OnMouseMove;
		/// <summary>
		/// Called when the cursor scrolls
		/// </summary>
		public event EventHandler<(int, string)>         OnMouseScroll;
		/// <summary>
		/// Updates all registered InputMethods and calls the necessary events
		/// </summary>
		public void Update() {
			List<MouseState> oldCursorStates = this.CursorStates.ToList();
			List<Keys>       oldKeys         = this.HeldKeys.ToList();

			for (int i = 0; i < this.registeredInputMethods.Count; i++) {
				InputMethod method = this.registeredInputMethods[i];

				method.Update();
			}

			#region OnKeyUp/Down

			List<Keys> diffKeysPressed  = this.HeldKeys.Except(oldKeys).ToList();
			List<Keys> diffKeysReleased = oldKeys.Except(this.HeldKeys).ToList();

			for (int i = 0; i < diffKeysPressed.Count; i++)
				this.OnKeyDown?.Invoke(this, diffKeysPressed[i]);

			for (int i = 0; i < diffKeysReleased.Count; i++)
				this.OnKeyUp?.Invoke(this, diffKeysReleased[i]);

			#endregion

			#region OnMouseUp/Down/Move/Scroll

			for (int i = 0; i < oldCursorStates.Count; i++) {
				MouseState oldState = oldCursorStates[i];

				List<MouseState> filteredStates = new();

				//Filtering States of the same name
				for(int k = 0; k != this.CursorStates.Count; k++)
					if(oldState.Name == this.CursorStates[k].Name)
						filteredStates.Add(this.CursorStates[k]);

				for (int j = 0; j < filteredStates.Count; j++) {
					MouseState newState = filteredStates[i];
					//Handling Mouse Movement by comparing to the last Input Frame
					if (oldState.State.Position != newState.State.Position)
						this.OnMouseMove?.Invoke(this, (newState.State.Position, newState.Name));


					//Handling The Left Mouse Button by comparing to the last Input Frame
					if (oldState.State.LeftButton != newState.State.LeftButton)
						if (oldState.State.LeftButton == ButtonState.Released)
							this.OnMouseDown?.Invoke(this, (MouseButton.LeftButton, newState.Name));
						else
							this.OnMouseUp?.Invoke(this, (MouseButton.LeftButton, newState.Name));


					//Handling The Right Mouse Button by comparing to the last Input Frame
					if (oldState.State.RightButton != newState.State.RightButton)
						if (oldState.State.RightButton == ButtonState.Released)
							this.OnMouseDown?.Invoke(this, (MouseButton.RightButton, newState.Name));
						else
							this.OnMouseUp?.Invoke(this, (MouseButton.RightButton, newState.Name));


					//Handling the Middle Mouse Button by comparing to the last Input Frame
					if (oldState.State.MiddleButton != newState.State.MiddleButton)
						if (oldState.State.MiddleButton == ButtonState.Released)
							this.OnMouseDown?.Invoke(this, (MouseButton.MiddleButton, newState.Name));
						else
							this.OnMouseUp?.Invoke(this, (MouseButton.MiddleButton, newState.Name));


					//Handling Scrolling by comparing to the last Input Frame
					if (oldState.State.ScrollWheelValue != newState.State.ScrollWheelValue)
						this.OnMouseScroll?.Invoke(this, (newState.State.ScrollWheelValue - oldState.State.ScrollWheelValue, newState.Name));
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
