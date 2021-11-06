namespace Furball.Engine.Engine.DevConsole.ConFuncs.Standard {
    /// <summary>
    /// `quit`
    /// Exits the Game
    /// </summary>
    public class Quit: ConFunc {
        public Quit() : base("quit") {}

        public override ConsoleResult Run(string[] consoleInput) {
            FurballGame.Instance.WindowManager.

            return new ConsoleResult(ExecutionResult.Success, "Exiting game.");
        }
    }
}
