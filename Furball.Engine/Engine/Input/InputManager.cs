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
        public event EventHandler<Keys> OnKeyDown;
        /// <summary>
        /// Called when a key is released
        /// </summary>
        public event EventHandler<Keys> OnKeyUp;
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
        public event EventHandler<(Point, string)> OnMouseMove;
        /// <summary>
        /// Called when a cursor moves
        /// </summary>
        public event EventHandler<((Point, Point), string)> OnMouseDrag;
        /// <summary>
        /// Called when the cursor scrolls
        /// </summary>
        public event EventHandler<(int, string)> OnMouseScroll;

        private List<Keys> _diffKeysPressed  = new();
        private List<Keys> _diffKeysReleased = new();

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

            this._diffKeysPressed  = this.HeldKeys.Except(oldKeys).ToList();
            this._diffKeysReleased = oldKeys.Except(this.HeldKeys).ToList();

            for (int i = 0; i < this._diffKeysPressed.Count; i++)
                this.OnKeyDown?.Invoke(this, this._diffKeysPressed[i]);

            for (int i = 0; i < this._diffKeysReleased.Count; i++)
                this.OnKeyUp?.Invoke(this, this._diffKeysReleased[i]);

            #endregion

            #region OnMouseUp/Down/Move/Scroll

            for (int i = 0; i < oldCursorStates.Count; i++) {
                MouseState oldState = oldCursorStates[i];

                List<MouseState> filteredStates = new();

                int cursorStateSize = this.CursorStates.Count;
                //Filtering States of the same name
                for (int k = 0; k < cursorStateSize; k++)
                    if (oldState.Name == this.CursorStates[k].Name)
                        filteredStates.Add(this.CursorStates[k]);

                for (int j = 0; j < filteredStates.Count; j++) {
                    MouseState newState = filteredStates[i];

                    //Handling Mouse Movement by comparing to the last Input Frame
                    if (oldState.Position != newState.Position) {
                        this.OnMouseMove?.Invoke(this, (newState.Position, newState.Name));

                        //We only are going to handle drags with M1
                        if (oldState.LeftButton == ButtonState.Pressed && newState.LeftButton == ButtonState.Pressed)
                            this.OnMouseDrag?.Invoke(this, ((oldState.Position, newState.Position), newState.Name));
                    }


                    //Handling The Left Mouse Button by comparing to the last Input Frame
                    if (oldState.LeftButton != newState.LeftButton)
                        if (oldState.LeftButton == ButtonState.Released)
                            this.OnMouseDown?.Invoke(this, (MouseButton.LeftButton, newState.Name));
                        else
                            this.OnMouseUp?.Invoke(this, (MouseButton.LeftButton, newState.Name));


                    //Handling The Right Mouse Button by comparing to the last Input Frame
                    if (oldState.RightButton != newState.RightButton)
                        if (oldState.RightButton == ButtonState.Released)
                            this.OnMouseDown?.Invoke(this, (MouseButton.RightButton, newState.Name));
                        else
                            this.OnMouseUp?.Invoke(this, (MouseButton.RightButton, newState.Name));


                    //Handling the Middle Mouse Button by comparing to the last Input Frame
                    if (oldState.MiddleButton != newState.MiddleButton)
                        if (oldState.MiddleButton == ButtonState.Released)
                            this.OnMouseDown?.Invoke(this, (MouseButton.MiddleButton, newState.Name));
                        else
                            this.OnMouseUp?.Invoke(this, (MouseButton.MiddleButton, newState.Name));


                    //Handling Scrolling by comparing to the last Input Frame
                    if (oldState.ScrollWheelValue != newState.ScrollWheelValue)
                        this.OnMouseScroll?.Invoke(this, (newState.ScrollWheelValue - oldState.ScrollWheelValue, newState.Name));
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
