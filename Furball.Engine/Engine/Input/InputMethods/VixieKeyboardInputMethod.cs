using System;
using System.Linq;
using Furball.Vixie.Input;
using Silk.NET.Input;
using Silk.NET.Input.Extensions;

namespace Furball.Engine.Engine.Input.InputMethods {
    public class VixieKeyboardInputMethod : InputMethod {
        public override void Update() {
            this.HeldKeys.Clear();

            for (int i = 0; i < this.Keyboards.Count; i++) {
                IKeyboard     keyboard = this.Keyboards[i];
                KeyboardState state    = keyboard.CaptureState();

                Span<Key> keys = state.GetPressedKeys();

                for (int i2 = 0; i2 < keys.Length; i2++) {
                    Key key = keys[i2];
                    this.HeldKeys.Add(key);
                }
            }
        }

        public override void Dispose() {
            this.Keyboards.Clear();
        }
        
        public override void Initialize() {
            this.Keyboards = Keyboard.GetKeyboards().ToList();
        }
    }
}
