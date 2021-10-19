using Furball.Engine.Engine.Input;

namespace Furball.Engine.Engine.DevConsole.ConFuncs.Input {
    /// <summary>
    /// im_input_methods
    /// Lists all available and Registered Input Methods
    /// </summary>
    public class AvailableInputMethods : ConFunc {
        public AvailableInputMethods() : base("im_input_methods") {}

        public override ConsoleResult Run(string[] consoleInput) {
            string result = "";

            for (int i = 0; i != FurballGame.InputManager.RegisteredInputMethods.Count; i++) {
                InputMethod currentMethod = FurballGame.InputManager.RegisteredInputMethods[i];

                string typeName = currentMethod.GetType().Name;

                result += $"[{i}] {typeName}\n";
            }

            return new ConsoleResult(ExecutionResult.Success, result);
        }
    }
}
