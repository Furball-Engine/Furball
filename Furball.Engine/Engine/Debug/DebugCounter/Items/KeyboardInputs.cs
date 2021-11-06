

namespace Furball.Engine.Engine.Debug.DebugCounter.Items {
    public class KeyboardInputs : DebugCounterItem {
        public override bool ForceNewLine { get; } = true;

        public override string GetAsString(double time) {
            string endString = "keyboard: {";

            for (int i = 0; i != FurballGame.InputManager.HeldKeys.Count; i++) {
                endString += $"{FurballGame.InputManager.HeldKeys[i].ToString()}, ";
            }

            return endString + "}";
        }
    }
}
